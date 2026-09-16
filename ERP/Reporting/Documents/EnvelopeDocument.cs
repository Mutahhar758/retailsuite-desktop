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
    /// Code-first QuestPDF document implementing standard DL business mailing envelopes (220mm x 110mm).
    /// </summary>
    public class EnvelopeDocument : IDocument
    {
        private readonly List<EnvelopeItem> _envelopes;

        public EnvelopeDocument(List<EnvelopeItem> envelopes)
        {
            _envelopes = envelopes ?? new List<EnvelopeItem>();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            if (_envelopes.Count == 0)
            {
                container.Page(page =>
                {
                    page.ContinuousSize(220, Unit.Millimetre);
                    page.Margin(20, Unit.Point);
                    page.Content().AlignCenter().AlignMiddle().Text("No envelope recipients selected.").Italic();
                });
                return;
            }

            for (int i = 0; i < _envelopes.Count; i++)
            {
                var item = _envelopes[i];
                container.Page(page =>
                {
                    // DL Envelope size: 220 mm width, 110 mm height
                    page.Size(220, 110, Unit.Millimetre);
                    page.Margin(24, Unit.Point);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9f).FontFamily("Segoe UI").FontColor(QuestPDF.Helpers.Colors.Grey.Darken4));

                    page.Content().Column(col =>
                    {
                        // 1. SENDER BLOCK (Top Left)
                        col.Item().Width(240).Column(senderCol =>
                        {
                            senderCol.Item().Text("RETURN IF UNDELIVERED TO:")
                                .FontSize(6.5f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                            string senderComp = !string.IsNullOrWhiteSpace(item.SenderCompany)
                                ? item.SenderCompany
                                : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                            senderCol.Item().PaddingTop(2).Text(senderComp)
                                .FontSize(9.5f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                            if (!string.IsNullOrWhiteSpace(item.SenderAddress))
                            {
                                senderCol.Item().PaddingTop(1).Text(item.SenderAddress)
                                    .FontSize(7.5f)
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            }

                            if (!string.IsNullOrWhiteSpace(item.SenderPhone))
                            {
                                senderCol.Item().PaddingTop(1).Text("Tel: " + item.SenderPhone)
                                    .FontSize(7.5f)
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            }

                            senderCol.Item().PaddingTop(4).LineHorizontal(0.5f).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                        });

                        col.Item().PaddingTop(32);

                        // 2. RECIPIENT BLOCK (Center / Lower Right)
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(180); // Spacer pushing recipient to center-right

                            row.RelativeItem().Border(1f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                                .Background(QuestPDF.Helpers.Colors.Grey.Lighten5).Padding(14).Column(recCol =>
                                {
                                    recCol.Item().Text("DELIVER TO:")
                                        .FontSize(7.5f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                                    recCol.Item().PaddingTop(3).Text(item.RecipientName ?? "Valued Recipient")
                                        .FontSize(12f)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);

                                    if (!string.IsNullOrWhiteSpace(item.RecipientCompany) && item.RecipientCompany != item.RecipientName)
                                    {
                                        recCol.Item().PaddingTop(2).Text(item.RecipientCompany)
                                            .FontSize(10f)
                                            .SemiBold()
                                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                                    }

                                    if (!string.IsNullOrWhiteSpace(item.RecipientAddress))
                                    {
                                        recCol.Item().PaddingTop(3).Text(item.RecipientAddress)
                                            .FontSize(9f)
                                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                                    }

                                    if (!string.IsNullOrWhiteSpace(item.RecipientPhone))
                                    {
                                        recCol.Item().PaddingTop(2).Text("Contact: " + item.RecipientPhone)
                                            .FontSize(8.5f)
                                            .Bold()
                                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                                    }
                                });
                        });
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

                string filePath = Path.Combine(tempDir, string.Format("Envelope_{0:yyyyMMdd_HHmmss}_{1}.pdf",
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
