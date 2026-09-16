using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ERP.Reporting.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERP.Reporting.Documents
{
    /// <summary>
    /// Code-first QuestPDF document implementing 4x6 inch (100mm x 150mm) professional shipping parcel tags.
    /// </summary>
    public class ShipmentTagDocument : IDocument
    {
        private readonly List<ShipmentTagItem> _tags;

        public ShipmentTagDocument(List<ShipmentTagItem> tags)
        {
            _tags = tags ?? new List<ShipmentTagItem>();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            if (_tags.Count == 0)
            {
                container.Page(p =>
                {
                    p.ContinuousSize(100, Unit.Millimetre);
                    p.Content().Text("No shipment tags to render.");
                });
                return;
            }

            for (int i = 0; i < _tags.Count; i++)
            {
                var tag = _tags[i];
                var barcodeBytes = BarcodeDataService.GenerateBarcodeBytes(tag.TrackingNo ?? "TRK-001", 200, 52);

                container.Page(page =>
                {
                    // Standard 4 x 6 inch shipping label: 101.6 mm x 152.4 mm
                    page.Size(102, 152, Unit.Millimetre);
                    page.Margin(16, Unit.Point);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Segoe UI").FontColor(QuestPDF.Helpers.Colors.Grey.Darken4));

                    page.Content().Column(col =>
                    {
                        // 1. TOP CARRIER & TRACKING BAR
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(trackCol =>
                            {
                                trackCol.Item().Text(tag.CourierService ?? "Express Cargo Service")
                                    .FontSize(9f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                                trackCol.Item().PaddingTop(1).Text(string.Format("Date: {0:dd-MMM-yyyy}", tag.DispatchDate))
                                    .FontSize(7.5f)
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                            });

                            row.ConstantItem(120).AlignRight().Border(1.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Darken4)
                                .Background(QuestPDF.Helpers.Colors.Grey.Lighten4).PaddingVertical(3).PaddingHorizontal(6).AlignCenter()
                                .Text(tag.PackageTag).FontSize(9.5f).Bold().FontColor(QuestPDF.Helpers.Colors.Black);
                        });

                        col.Item().PaddingTop(4).AlignCenter().Height(16, Unit.Millimetre).Image(barcodeBytes);

                        col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                        // 2. SHIP TO / CONSIGNEE BOX (High Contrast)
                        col.Item().PaddingTop(8).Border(2f).BorderColor(QuestPDF.Helpers.Colors.Grey.Darken4)
                            .Background(QuestPDF.Helpers.Colors.Grey.Lighten5).Padding(10).Column(destCol =>
                            {
                                destCol.Item().Text("SHIP TO / CONSIGNEE:")
                                    .FontSize(8f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                                destCol.Item().PaddingTop(3).Text(tag.ConsigneeName ?? "Valued Customer")
                                    .FontSize(13f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Black);

                                if (!string.IsNullOrWhiteSpace(tag.ConsigneeCompany) && tag.ConsigneeCompany != tag.ConsigneeName)
                                {
                                    destCol.Item().PaddingTop(2).Text(tag.ConsigneeCompany)
                                        .FontSize(10.5f)
                                        .SemiBold()
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                                }

                                if (!string.IsNullOrWhiteSpace(tag.ConsigneeAddress))
                                {
                                    destCol.Item().PaddingTop(3).Text(tag.ConsigneeAddress)
                                        .FontSize(9.5f)
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);
                                }

                                if (!string.IsNullOrWhiteSpace(tag.ConsigneeCity))
                                {
                                    destCol.Item().PaddingTop(2).Text(tag.ConsigneeCity)
                                        .FontSize(11f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Black);
                                }

                                if (!string.IsNullOrWhiteSpace(tag.ConsigneePhone))
                                {
                                    destCol.Item().PaddingTop(4).Text("📞 Phone: " + tag.ConsigneePhone)
                                        .FontSize(10.5f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);
                                }
                            });

                        col.Item().PaddingTop(8);

                        // 3. FROM / SHIPPER BOX
                        col.Item().Border(1f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                            .Background(QuestPDF.Helpers.Colors.White).Padding(8).Column(fromCol =>
                            {
                                fromCol.Item().Text("FROM / SENDER:")
                                    .FontSize(7.5f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                                string shipperName = !string.IsNullOrWhiteSpace(tag.ShipperName)
                                    ? tag.ShipperName
                                    : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                                fromCol.Item().PaddingTop(2).Text(shipperName)
                                    .FontSize(9.5f)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                                if (!string.IsNullOrWhiteSpace(tag.ShipperAddress))
                                {
                                    fromCol.Item().PaddingTop(1).Text(tag.ShipperAddress)
                                        .FontSize(8f)
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                }

                                if (!string.IsNullOrWhiteSpace(tag.ShipperPhone))
                                {
                                    fromCol.Item().PaddingTop(1).Text("Sender Tel: " + tag.ShipperPhone)
                                        .FontSize(8f)
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                }
                            });

                        col.Item().PaddingTop(6);

                        // 4. PARCEL METRICS STRIP
                        col.Item().Row(metricRow =>
                        {
                            metricRow.RelativeItem().Text(x =>
                            {
                                x.Span("Inv #: ").FontSize(8f).Bold();
                                x.Span(tag.InvoiceNo ?? "N/A").FontSize(8f);
                            });

                            if (tag.WeightKg > 0)
                            {
                                metricRow.RelativeItem().AlignRight().Text(x =>
                                {
                                    x.Span("Weight: ").FontSize(8f).Bold();
                                    x.Span(tag.WeightKg.ToString("0.0") + " KG").FontSize(8f);
                                });
                            }
                        });

                        col.Item().PaddingTop(6);

                        // 5. CAUTION WARNING BANNER
                        col.Item().Border(1f).BorderColor(QuestPDF.Helpers.Colors.Red.Lighten2)
                            .Background(QuestPDF.Helpers.Colors.Red.Lighten5).PaddingVertical(4).PaddingHorizontal(6)
                            .AlignCenter().Text("⚠ " + (tag.Remarks ?? "HANDLE WITH CARE • FRAGILE • KEEP DRY"))
                            .FontSize(7.5f)
                            .Bold()
                            .FontColor(QuestPDF.Helpers.Colors.Red.Darken2);
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

                string filePath = Path.Combine(tempDir, string.Format("ShipmentTag_{0:yyyyMMdd_HHmmss}_{1}.pdf",
                    DateTime.Now,
                    Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }

        public static void PrintDirectToPrinter(IDocument document, string printerName)
        {
            var images = document.GenerateImages().ToList();
            if (images.Count == 0) return;

            int pageIndex = 0;
            using (var pd = new PrintDocument())
            {
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    pd.PrinterSettings.PrinterName = printerName;
                }

                pd.PrintController = new StandardPrintController();
                pd.PrintPage += (s, ev) =>
                {
                    if (pageIndex < images.Count)
                    {
                        using (var ms = new MemoryStream(images[pageIndex]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        {
                            System.Drawing.Rectangle bounds = ev.MarginBounds;
                            if (bounds.Width <= 0 || bounds.Height <= 0) bounds = ev.PageBounds;
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
