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

        public BarcodeDocument(List<BarcodeLabelItem> items, BarcodePrintLayout layout = BarcodePrintLayout.Roll4Inch3Up)
        {
            _items = items ?? new List<BarcodeLabelItem>();
            _layout = layout;
            _companyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite";
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

            for (int r = 0; r < totalRows; r++)
            {
                var rowItems = expanded.Skip(r * pageSize).Take(pageSize).ToList();

                container.Page(page =>
                {
                    page.Size(100f, 22.86f, Unit.Millimetre);
                    page.MarginVertical(0.3f, Unit.Millimetre);
                    page.MarginHorizontal(1.0f, Unit.Millimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    page.Content().Row(row =>
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            if (c > 0)
                            {
                                row.ConstantItem(2, Unit.Millimetre); // Gap between stickers
                            }

                            if (c < rowItems.Count)
                            {
                                var item = rowItems[c];
                                var slices = BarcodeDataService.GenerateSlices(item.Barcode, 3);

                                string comp = _companyName ?? string.Empty;
                                if (comp.Length > 24) comp = comp.Substring(0, 22) + "..";

                                string title = item.Title ?? string.Empty;
                                if (title.Length > 22) title = title.Substring(0, 20) + "..";

                                row.RelativeItem().PaddingHorizontal(1.5f, Unit.Millimetre).Column(col =>
                                {
                                    col.Item().AlignCenter().Text(comp)
                                        .FontSize(5.5f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                                    col.Item().AlignCenter().Text(title)
                                        .FontSize(6.5f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Black);

                                    col.Item()
                                        .PaddingVertical(0.3f, Unit.Millimetre)
                                        .AlignCenter()
                                        .Height(item.IsVariableWeight ? 8.5f : 9.5f, Unit.Millimetre)
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
                                        .FontSize(item.Barcode.Length > 18 ? 3.8f : (item.Barcode.Length > 13 ? 4.5f : 5.5f))
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                                    if (item.IsVariableWeight && item.Qty.HasValue)
                                    {
                                        decimal tot = Math.Round(item.TotalAmount ?? (item.Qty.Value * item.Rate));
                                        decimal rate = Math.Round(item.Rate);
                                        string u = !string.IsNullOrWhiteSpace(item.Unit) && !decimal.TryParse(item.Unit, out _) ? item.Unit : "kg";
                                        col.Item().AlignCenter().Text(string.Format("Qty: {0:0.###} {1} • Rs. {2:#,##0}", item.Qty.Value, u, tot))
                                            .FontSize(6.5f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Black);

                                        col.Item().AlignCenter().Text(string.Format("Rate: Rs. {0:#,##0}/{1}", rate, u))
                                            .FontSize(5f)
                                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                    }
                                    else
                                    {
                                        col.Item().AlignCenter().Text(item.FormattedRate)
                                            .FontSize(7.5f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Black);
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
                        col.Item().AlignCenter().Text(_companyName)
                            .FontSize(6.5f)
                            .Bold()
                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                        col.Item().AlignCenter().Text(item.Title ?? string.Empty)
                            .FontSize(7.5f)
                            .Bold()
                            .FontColor(QuestPDF.Helpers.Colors.Black);

                        col.Item().PaddingVertical(1).AlignCenter().Height(item.IsVariableWeight ? 12 : 14, Unit.Millimetre).Image(barcodeBytes);

                        if (item.IsVariableWeight && item.Qty.HasValue)
                        {
                            decimal tot = Math.Round(item.TotalAmount ?? (item.Qty.Value * item.Rate));
                            decimal rate = Math.Round(item.Rate);
                            string u = !string.IsNullOrWhiteSpace(item.Unit) && !decimal.TryParse(item.Unit, out _) ? item.Unit : "kg";
                            col.Item().AlignCenter().Text(string.Format("Qty: {0:0.###} {1} • Rs. {2:#,##0}", item.Qty.Value, u, tot))
                                .FontSize(8f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Black);

                            col.Item().AlignCenter().Text(string.Format("Rate: Rs. {0:#,##0}/{1}", rate, u))
                                .FontSize(6.5f)
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                        }
                        else
                        {
                            col.Item().AlignCenter().Text(item.FormattedRate)
                                .FontSize(9f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Black);
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
                                    sticker.Item().AlignCenter().Text(_companyName)
                                        .FontSize(6.5f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                                    sticker.Item().AlignCenter().Text(item.Title ?? string.Empty)
                                        .FontSize(8f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);

                                    sticker.Item().PaddingVertical(2).AlignCenter().Height(item.IsVariableWeight ? 13 : 16, Unit.Millimetre).Image(barcodeBytes);

                                    if (item.IsVariableWeight && item.Qty.HasValue)
                                    {
                                        decimal tot = Math.Round(item.TotalAmount ?? (item.Qty.Value * item.Rate));
                                        decimal rate = Math.Round(item.Rate);
                                        string u = !string.IsNullOrWhiteSpace(item.Unit) && !decimal.TryParse(item.Unit, out _) ? item.Unit : "kg";
                                        sticker.Item().AlignCenter().Text(string.Format("Qty: {0:0.###} {1} • Rs. {2:#,##0}", item.Qty.Value, u, tot))
                                            .FontSize(8.5f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                                        sticker.Item().AlignCenter().Text(string.Format("Rate: Rs. {0:#,##0}/{1}", rate, u))
                                            .FontSize(7f)
                                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                    }
                                    else
                                    {
                                        sticker.Item().AlignCenter().Text(item.FormattedRate)
                                            .FontSize(9.5f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
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

        public static void PrintDirectToPrinter(IDocument document, string printerName)
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
                pd.PrintController = new StandardPrintController();
                pd.PrintPage += (s, ev) =>
                {
                    if (pageIndex < images.Count)
                    {
                        using (var ms = new MemoryStream(images[pageIndex]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        {
                            System.Drawing.Rectangle bounds = ev.PageBounds;
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
