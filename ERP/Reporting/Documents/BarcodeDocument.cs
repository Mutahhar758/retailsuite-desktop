using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ERP.Classes;
using ERP.Reporting.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERP.Reporting.Documents
{
    public enum BarcodePrintLayout
    {
        Roll4Inch3Up, // 4-inch roll (101.6mm x 25.4mm) with 3 barcodes horizontally across
        ThermalRoll,  // 50mm x 30mm sticker roll
        SheetA4       // A4 3x8 = 24 stickers per sheet
    }

    /// <summary>
    /// Code-first QuestPDF document rendering barcode price stickers for thermal label printers or A4 sticker sheets.
    /// </summary>
    public class BarcodeDocument : IDocument
    {
        private readonly List<BarcodeLabelItem> _items;
        private readonly BarcodePrintLayout _layout;
        private readonly string _companyName;
        private readonly string _companySubHeader;
        private readonly float _sideStickersSpreadMm;
        private readonly bool _includeCompanyName;
        private readonly bool _includeCompanySubHeader;
        private readonly bool _includeTitle;
        private readonly bool _includePrice;
        private readonly bool _includeRate;

        public BarcodeDocument(
            List<BarcodeLabelItem> items,
            BarcodePrintLayout layout = BarcodePrintLayout.Roll4Inch3Up,
            float sideStickersSpreadMm = 2.0f,
            string companySubHeader = null,
            bool includeCompanyName = true,
            bool includeCompanySubHeader = true,
            bool includeTitle = true,
            bool includePrice = true,
            bool includeRate = false)
        {
            _items = items ?? new List<BarcodeLabelItem>();
            _layout = layout;
            _companyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite";
            _sideStickersSpreadMm = Math.Max(0f, sideStickersSpreadMm);
            _includeCompanyName = includeCompanyName;
            _includeCompanySubHeader = includeCompanySubHeader;
            _includeTitle = includeTitle;
            _includePrice = includePrice;
            _includeRate = includeRate;
            _companySubHeader = _includeCompanySubHeader && !string.IsNullOrWhiteSpace(companySubHeader)
                ? companySubHeader
                : (_includeCompanySubHeader && !string.IsNullOrWhiteSpace(CompanyInfo.Description) ? CompanyInfo.Description : (_includeCompanySubHeader ? CompanyInfo.Address : null));
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            if (_layout == BarcodePrintLayout.Roll4Inch3Up)
            {
                ComposeRoll4Inch3Up(container);
            }
            else if (_layout == BarcodePrintLayout.ThermalRoll)
            {
                ComposeThermalRoll(container);
            }
            else
            {
                ComposeSheetA4(container);
            }
        }

        /// <summary>
        /// 4-inch continuous thermal roll with 3 barcodes horizontally across per feed row (101.6mm x 25.4mm / 4" x 1").
        /// </summary>
        private void ComposeRoll4Inch3Up(IDocumentContainer container)
        {
            var expanded = new List<BarcodeLabelItem>();
            foreach (var item in _items)
            {
                int count = Math.Max(1, item.Copies);
                for (int i = 0; i < count; i++) expanded.Add(item);
            }

            if (expanded.Count == 0)
            {
                container.Page(p =>
                {
                    p.Size(101.6f, 25.4f, Unit.Millimetre);
                    p.Content().AlignCenter().AlignMiddle().Text("No barcode items.");
                });
                return;
            }

            int pageSize = 3; // 3 stickers across per row
            int totalRows = (int)Math.Ceiling(expanded.Count / (double)pageSize);

            float spread = _sideStickersSpreadMm;
            float pageMarginH = Math.Max(0.2f, 1.0f - (spread * 0.35f));
            float interStickerGap = 2.0f + (spread * 0.65f);

            for (int r = 0; r < totalRows; r++)
            {
                var rowItems = expanded.Skip(r * pageSize).Take(pageSize).ToList();

                container.Page(page =>
                {
                    // 3.94 in x 0.90 in (100mm x 22.86mm) 3-up continuous thermal sticker roll
                    page.Size(100f, 22.86f, Unit.Millimetre);
                    page.MarginVertical(0.4f, Unit.Millimetre);
                    page.MarginHorizontal(pageMarginH, Unit.Millimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    page.Content().Row(row =>
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            if (c > 0)
                            {
                                row.ConstantItem(interStickerGap, Unit.Millimetre); // Gap between stickers
                            }

                            if (c < rowItems.Count)
                            {
                                var item = rowItems[c];
                                var slices = BarcodeDataService.GenerateSlices(item.Barcode, 3);

                                string comp = _companyName ?? string.Empty;
                                if (comp.Length > 24) comp = comp.Substring(0, 22) + "..";

                                string title = item.Title ?? string.Empty;
                                if (title.Length > 22) title = title.Substring(0, 20) + "..";

                                string subHead = _includeCompanySubHeader
                                    ? (!string.IsNullOrWhiteSpace(item.CompanySubHeader) ? item.CompanySubHeader : _companySubHeader)
                                    : null;
                                bool hasSubHeader = _includeCompanyName && !string.IsNullOrWhiteSpace(subHead);
                                bool isVariable = item.IsVariableWeight && item.Qty.HasValue;
                                bool hasRate = _includeRate && item.Rate > 0;

                                float barcodeHeightMm;
                                float compFontSize;
                                float subFontSize = 4.0f;
                                float titleFontSize;
                                float barcodeNumFontSize;
                                float priceFontSize;
                                float rateFontSize = 4.8f;
                                float itemSpacing;

                                if (hasSubHeader && hasRate)
                                {
                                    barcodeHeightMm = 4.8f;
                                    compFontSize = 5.0f;
                                    titleFontSize = 5.5f;
                                    barcodeNumFontSize = 4.5f;
                                    priceFontSize = 6.2f;
                                    itemSpacing = 0f;
                                }
                                else if (hasSubHeader || hasRate)
                                {
                                    barcodeHeightMm = 5.2f;
                                    compFontSize = 5.0f;
                                    titleFontSize = 5.8f;
                                    barcodeNumFontSize = item.Barcode.Length > 18 ? 4.5f : (item.Barcode.Length > 13 ? 5.2f : 5.8f);
                                    priceFontSize = 6.8f;
                                    itemSpacing = 0f;
                                }
                                else
                                {
                                    // Standard 5-line sticker (Company, Title, Barcode, Barcode Number, Price) on 0.90 in sticker
                                    // Balanced height & spacing so it fills the sticker and is centered vertically without touching edges
                                    barcodeHeightMm = isVariable ? 6.8f : 7.5f;
                                    compFontSize = 5.5f;
                                    titleFontSize = 6.2f;
                                    barcodeNumFontSize = item.Barcode.Length > 18 ? 4.8f : (item.Barcode.Length > 13 ? 5.8f : 6.2f);
                                    priceFontSize = 7.8f;
                                    itemSpacing = 0.2f;
                                }

                                float leftPad = 1.0f;
                                float rightPad = 1.0f;
                                if (c == 0)
                                {
                                    // Shift leftmost sticker outward (left) to decrease outer left margin and prevent cutting on inner right side
                                    leftPad = Math.Max(0.2f, 1.0f - (spread * 0.4f));
                                    rightPad = 1.0f + (spread * 0.4f);
                                }
                                else if (c == 2)
                                {
                                    // Shift rightmost sticker outward (right) to decrease outer right margin and prevent cutting on inner left side
                                    leftPad = 1.0f + (spread * 0.4f);
                                    rightPad = Math.Max(0.2f, 1.0f - (spread * 0.4f));
                                }

                                row.RelativeItem()
                                    .PaddingLeft(leftPad, Unit.Millimetre)
                                    .PaddingRight(rightPad, Unit.Millimetre)
                                    .PaddingVertical(hasSubHeader || hasRate ? 0.2f : 0.5f, Unit.Millimetre)
                                    .Column(col =>
                                {
                                    col.Spacing(itemSpacing);

                                    if (_includeCompanyName)
                                    {
                                        col.Item().AlignCenter().Text(comp)
                                            .FontSize(compFontSize)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Black);

                                        if (hasSubHeader)
                                        {
                                            string displaySub = subHead.Length > 28 ? subHead.Substring(0, 26) + ".." : subHead;
                                            col.Item().AlignCenter().Text(displaySub)
                                                .FontSize(subFontSize)
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                    }

                                    if (_includeTitle)
                                    {
                                        col.Item().AlignCenter().Text(title)
                                            .FontSize(titleFontSize)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Black);
                                    }

                                    col.Item()
                                        .PaddingVertical(hasSubHeader || hasRate ? 0.1f : 0.25f, Unit.Millimetre)
                                        .AlignCenter()
                                        .Height(barcodeHeightMm, Unit.Millimetre)
                                        .Row(bRow =>
                                        {
                                            foreach (var slice in slices)
                                            {
                                                if (slice.IsBlack)
                                                    bRow.RelativeItem(slice.Width).Background(QuestPDF.Helpers.Colors.Black);
                                                else
                                                    bRow.RelativeItem(slice.Width);
                                            }
                                        });

                                    col.Item().AlignCenter().Text(item.Barcode)
                                        .FontSize(barcodeNumFontSize)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Black);

                                    if (isVariable)
                                    {
                                        decimal tot = Math.Round(item.TotalAmount ?? (item.Qty.Value * item.Rate));
                                        string u = !string.IsNullOrWhiteSpace(item.Unit) && !decimal.TryParse(item.Unit, out _) ? item.Unit : "kg";

                                        decimal dispQty = item.DisplayQty.HasValue ? item.DisplayQty.Value : item.Qty.Value;
                                        string dispUnit = !string.IsNullOrWhiteSpace(item.DisplayUnit) ? item.DisplayUnit : u;
                                        string formattedQty = dispQty % 1m == 0m ? dispQty.ToString("0") : dispQty.ToString("0.###");

                                        if (_includePrice)
                                        {
                                            string priceStr = tot > 0
                                                ? string.Format("Qty: {0} {1} • Rs. {2:#,##0}", formattedQty, dispUnit, tot)
                                                : string.Format("Qty: {0} {1}", formattedQty, dispUnit);

                                            col.Item().AlignCenter().Text(priceStr)
                                                .FontSize(priceFontSize)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                        else
                                        {
                                            col.Item().AlignCenter().Text(string.Format("Qty: {0} {1}", formattedQty, dispUnit))
                                                .FontSize(priceFontSize)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }

                                        if (hasRate)
                                        {
                                            decimal rate = Math.Round(item.Rate);
                                            col.Item().AlignCenter().Text(string.Format("Rate: Rs. {0:#,##0}/{1}", rate, u))
                                                .FontSize(rateFontSize)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                    }
                                    else
                                    {
                                        if (_includePrice)
                                        {
                                            col.Item().AlignCenter().Text(item.FormattedRate)
                                                .FontSize(priceFontSize)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                    }
                                });
                            }
                            else
                            {
                                row.RelativeItem(); // Empty space to preserve 3-column width
                            }
                        }
                    });
                });
            }
        }

        private void ComposeThermalRoll(IDocumentContainer container)
        {
            // Expand copies
            var expanded = new List<BarcodeLabelItem>();
            foreach (var item in _items)
            {
                int count = Math.Max(1, item.Copies);
                for (int i = 0; i < count; i++) expanded.Add(item);
            }

            if (expanded.Count == 0)
            {
                container.Page(p =>
                {
                    p.ContinuousSize(50, Unit.Millimetre);
                    p.Content().Text("No barcode items.");
                });
                return;
            }

            for (int i = 0; i < expanded.Count; i++)
            {
                var item = expanded[i];
                var barcodeBytes = BarcodeDataService.GenerateBarcodeBytes(item.Barcode, 160, 52);

                container.Page(page =>
                {
                    // Standard 50mm x 30mm thermal sticker
                    page.Size(50, 30, Unit.Millimetre);
                    page.Margin(4, Unit.Point);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    page.Content().Column(col =>
                    {
                        if (_includeCompanyName)
                        {
                            col.Item().AlignCenter().Text(_companyName)
                                .FontSize(6.5f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Black);

                            string subHead = _includeCompanySubHeader
                                ? (!string.IsNullOrWhiteSpace(item.CompanySubHeader) ? item.CompanySubHeader : _companySubHeader)
                                : null;
                            if (!string.IsNullOrWhiteSpace(subHead))
                            {
                                string displaySub = subHead.Length > 36 ? subHead.Substring(0, 34) + ".." : subHead;
                                col.Item().AlignCenter().Text(displaySub)
                                    .FontSize(5.5f)
                                    .FontColor(QuestPDF.Helpers.Colors.Black);
                            }
                        }

                        if (_includeTitle)
                        {
                            col.Item().AlignCenter().Text(item.Title ?? string.Empty)
                                .FontSize(8f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Black);
                        }

                        col.Item().PaddingVertical(1).AlignCenter().Height(item.IsVariableWeight ? 11 : 13, Unit.Millimetre).Image(barcodeBytes);

                        if (item.IsVariableWeight && item.Qty.HasValue)
                        {
                            decimal tot = Math.Round(item.TotalAmount ?? (item.Qty.Value * item.Rate));
                            string u = !string.IsNullOrWhiteSpace(item.Unit) && !decimal.TryParse(item.Unit, out _) ? item.Unit : "kg";

                            decimal dispQty = item.DisplayQty.HasValue ? item.DisplayQty.Value : item.Qty.Value;
                            string dispUnit = !string.IsNullOrWhiteSpace(item.DisplayUnit) ? item.DisplayUnit : u;
                            string formattedQty = dispQty % 1m == 0m ? dispQty.ToString("0") : dispQty.ToString("0.###");

                            if (_includePrice)
                            {
                                string priceStr = tot > 0
                                    ? string.Format("Qty: {0} {1} • Rs. {2:#,##0}", formattedQty, dispUnit, tot)
                                    : string.Format("Qty: {0} {1}", formattedQty, dispUnit);

                                col.Item().AlignCenter().Text(priceStr)
                                    .FontSize(8.5f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Black);
                            }
                            else
                            {
                                col.Item().AlignCenter().Text(string.Format("Qty: {0} {1}", formattedQty, dispUnit))
                                    .FontSize(8.5f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Black);
                            }

                            if (_includeRate && item.Rate > 0)
                            {
                                decimal rate = Math.Round(item.Rate);
                                col.Item().AlignCenter().Text(string.Format("Rate: Rs. {0:#,##0}/{1}", rate, u))
                                    .FontSize(7.2f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Black);
                            }
                        }
                        else
                        {
                            if (_includePrice)
                            {
                                col.Item().AlignCenter().Text(item.FormattedRate)
                                    .FontSize(10f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Black);
                            }
                        }
                    });
                });
            }
        }

        private void ComposeSheetA4(IDocumentContainer container)
        {
            var expanded = new List<BarcodeLabelItem>();
            foreach (var item in _items)
            {
                int count = Math.Max(1, item.Copies);
                for (int i = 0; i < count; i++) expanded.Add(item);
            }

            int pageSize = 24; // 3 cols x 8 rows
            int totalPages = (int)Math.Ceiling(expanded.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            for (int p = 0; p < totalPages; p++)
            {
                var pageItems = expanded.Skip(p * pageSize).Take(pageSize).ToList();

                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(18, Unit.Point);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    page.Content().Grid(grid =>
                    {
                        grid.Columns(3);
                        grid.Spacing(8);

                        for (int i = 0; i < pageItems.Count; i++)
                        {
                            var item = pageItems[i];
                            var barcodeBytes = BarcodeDataService.GenerateBarcodeBytes(item.Barcode, 150, 50);

                            grid.Item().Border(0.75f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                                .Background(QuestPDF.Helpers.Colors.White).Padding(6).Column(sticker =>
                                {
                                    if (_includeCompanyName)
                                    {
                                        sticker.Item().AlignCenter().Text(_companyName)
                                            .FontSize(6.5f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Black);

                                        string subHead = _includeCompanySubHeader
                                            ? (!string.IsNullOrWhiteSpace(item.CompanySubHeader) ? item.CompanySubHeader : _companySubHeader)
                                            : null;
                                        if (!string.IsNullOrWhiteSpace(subHead))
                                        {
                                            string displaySub = subHead.Length > 36 ? subHead.Substring(0, 34) + ".." : subHead;
                                            sticker.Item().AlignCenter().Text(displaySub)
                                                .FontSize(5.5f)
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                    }

                                    if (_includeTitle)
                                    {
                                        sticker.Item().AlignCenter().Text(item.Title ?? string.Empty)
                                            .FontSize(8f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Black);
                                    }

                                    sticker.Item().PaddingVertical(2).AlignCenter().Height(item.IsVariableWeight ? 13 : 16, Unit.Millimetre).Image(barcodeBytes);

                                    if (item.IsVariableWeight && item.Qty.HasValue)
                                    {
                                        decimal tot = Math.Round(item.TotalAmount ?? (item.Qty.Value * item.Rate));
                                        string u = !string.IsNullOrWhiteSpace(item.Unit) && !decimal.TryParse(item.Unit, out _) ? item.Unit : "kg";

                                        decimal dispQty = item.DisplayQty.HasValue ? item.DisplayQty.Value : item.Qty.Value;
                                        string dispUnit = !string.IsNullOrWhiteSpace(item.DisplayUnit) ? item.DisplayUnit : u;
                                        string formattedQty = dispQty % 1m == 0m ? dispQty.ToString("0") : dispQty.ToString("0.###");

                                        if (_includePrice)
                                        {
                                            string priceStr = tot > 0
                                                ? string.Format("Qty: {0} {1} • Rs. {2:#,##0}", formattedQty, dispUnit, tot)
                                                : string.Format("Qty: {0} {1}", formattedQty, dispUnit);

                                            sticker.Item().AlignCenter().Text(priceStr)
                                                .FontSize(8.5f)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                        else
                                        {
                                            sticker.Item().AlignCenter().Text(string.Format("Qty: {0} {1}", formattedQty, dispUnit))
                                                .FontSize(8.5f)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }

                                        if (_includeRate && item.Rate > 0)
                                        {
                                            decimal rate = Math.Round(item.Rate);
                                            sticker.Item().AlignCenter().Text(string.Format("Rate: Rs. {0:#,##0}/{1}", rate, u))
                                                .FontSize(7.5f)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                    }
                                    else
                                    {
                                        if (_includePrice)
                                        {
                                            sticker.Item().AlignCenter().Text(item.FormattedRate)
                                                .FontSize(9.5f)
                                                .Bold()
                                                .FontColor(QuestPDF.Helpers.Colors.Black);
                                        }
                                    }
                                });
                        }
                    });
                });
            }
        }

        public async Task<string> GeneratePdfToTempFileAsync()
        {
            return await Task.Run(() =>
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "RetailSuite", "Reports");
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                string filePath = Path.Combine(tempDir, string.Format("BarcodeLabels_{0:yyyyMMdd_HHmmss}_{1}.pdf",
                    DateTime.Now,
                    Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }

        public static void PrintDirectToPrinter(IDocument document, string printerName, BarcodePrintLayout layout = BarcodePrintLayout.Roll4Inch3Up)
        {
            var settings = new ImageGenerationSettings { RasterDpi = 300 };
            var images = document.GenerateImages(settings).ToList();
            if (images.Count == 0) return;

            int pageIndex = 0;
            using (var pd = new PrintDocument())
            {
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    pd.PrinterSettings.PrinterName = printerName;
                }

                pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                pd.OriginAtMargins = false;

                // Explicitly define target dimensions in hundredths of an inch (1/100 inch)
                int targetWidthHundredths;
                int targetHeightHundredths;
                if (layout == BarcodePrintLayout.Roll4Inch3Up)
                {
                    targetWidthHundredths = 394; // 3.94 inches (100mm)
                    targetHeightHundredths = 90; // 0.90 inch (22.86mm)
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Roll 3.94x0.90 in", targetWidthHundredths, targetHeightHundredths);
                }
                else if (layout == BarcodePrintLayout.ThermalRoll)
                {
                    targetWidthHundredths = 197; // 50mm
                    targetHeightHundredths = 118; // 30mm
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Roll 50x30mm", targetWidthHundredths, targetHeightHundredths);
                }
                else
                {
                    targetWidthHundredths = 827; // A4
                    targetHeightHundredths = 1169;
                    pd.DefaultPageSettings.PaperSize = new PaperSize("A4", targetWidthHundredths, targetHeightHundredths);
                }

                pd.PrintController = new StandardPrintController();
                pd.PrintPage += (s, ev) =>
                {
                    if (pageIndex < images.Count)
                    {
                        using (var ms = new MemoryStream(images[pageIndex]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        {
                            // Prevent thermal label drivers (e.g. Xprinter USER 409x709) from stretching stickers
                            int drawWidth = ev.PageBounds.Width > 0 ? ev.PageBounds.Width : targetWidthHundredths;
                            int drawHeight = targetHeightHundredths;
                            if (ev.PageBounds.Height > 0 && ev.PageBounds.Height <= 110)
                            {
                                drawHeight = ev.PageBounds.Height;
                            }

                            var bounds = new System.Drawing.Rectangle(0, 0, drawWidth, drawHeight);

                            // Clear solid white so thermal printers don't burn transparent/uninitialized areas black
                            ev.Graphics.Clear(System.Drawing.Color.White);
                            ev.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            ev.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            ev.Graphics.DrawImage(img, bounds);
                        }
                        pageIndex++;
                        ev.HasMorePages = pageIndex < images.Count;
                    }
                    else
                    {
                        ev.HasMorePages = false;
                    }
                };
                pd.Print();
            }
        }
    }
}
