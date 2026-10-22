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
    /// Code-first QuestPDF document implementing modern sale receipts and invoices.
    /// Supports 80mm Continuous Thermal Receipt, A5, and A4 invoice formats.
    /// </summary>
    public class SaleReceiptDocument : IDocument
    {
        private readonly SaleReceiptSummary _summary;
        private readonly List<SaleReceiptLineItem> _lines;
        private readonly SaleReceiptPrintLayout _layout;

        public SaleReceiptDocument(SaleReceiptSummary summary, List<SaleReceiptLineItem> lines, SaleReceiptPrintLayout layout = SaleReceiptPrintLayout.Thermal80mm)
        {
            _summary = summary ?? new SaleReceiptSummary();
            _lines = lines ?? new List<SaleReceiptLineItem>();
            _layout = layout;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            if (_layout == SaleReceiptPrintLayout.Thermal80mm)
            {
                ComposeThermal80(container);
            }
            else if (_layout == SaleReceiptPrintLayout.A5Sheet)
            {
                ComposeA5(container);
            }
            else
            {
                ComposeA4(container);
            }
        }

        #region Thermal 80mm Composition

        private void ComposeThermal80(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(80, Unit.Millimetre);
                page.MarginVertical(2, Unit.Millimetre);
                page.MarginLeft(2.5f, Unit.Millimetre);
                page.MarginRight(6.5f, Unit.Millimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8f).FontFamily("Arial").FontColor(Colors.Black));

                page.Content().Column(col =>
                {
                    string compName = !string.IsNullOrWhiteSpace(_summary?.CompanyName)
                        ? _summary.CompanyName
                        : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                    // 1. Company Header
                    col.Item().AlignCenter().Text(compName).FontSize(11.5f).Bold();

                    if (!string.IsNullOrWhiteSpace(_summary?.CompanyAddress))
                    {
                        col.Item().AlignCenter().PaddingTop(1).Text(_summary.CompanyAddress).FontSize(7.5f).SemiBold();
                    }

                    if (!string.IsNullOrWhiteSpace(_summary?.CompanyPhone))
                    {
                        col.Item().AlignCenter().PaddingTop(1).Text("Tel: " + _summary.CompanyPhone).FontSize(7.5f).SemiBold();
                    }

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    // 2. Receipt Title
                    string receiptTitle = _summary.IsDuplicate ? "SALE RECEIPT (DUPLICATE)" : "SALE RECEIPT";
                    col.Item().AlignCenter().Text(receiptTitle).FontSize(9f).Bold();

                    // 3. Metadata
                    col.Item().PaddingTop(1).Row(r =>
                    {
                        r.AutoItem().Text("Voucher: ").Bold().FontSize(8f);
                        r.RelativeItem().Text(_summary?.VoucherNo ?? "").Bold().FontSize(8f);
                        r.AutoItem().Text(string.Format("{0:dd-MMM-yy HH:mm}", _summary.Date)).FontSize(7.5f);
                    });

                    col.Item().Row(r =>
                    {
                        r.AutoItem().Text("Customer: ").Bold().FontSize(8f);
                        r.RelativeItem().Text(!string.IsNullOrWhiteSpace(_summary?.CustomerName) ? _summary.CustomerName : "Walk-in Customer").FontSize(8f).SemiBold();
                    });

                    if (!string.IsNullOrWhiteSpace(_summary?.Cashier))
                    {
                        col.Item().Row(r =>
                        {
                            r.AutoItem().Text("Cashier: ").FontSize(7.5f);
                            r.RelativeItem().Text(_summary.Cashier).FontSize(7.5f);
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(_summary?.Description))
                    {
                        col.Item().Row(r =>
                        {
                            r.AutoItem().Text("Remarks: ").FontSize(7.5f);
                            r.RelativeItem().Text(_summary.Description).FontSize(7.5f);
                        });
                    }

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    // 4. Line Items Table
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2.6f); // Item
                            columns.RelativeColumn(1.1f); // Qty (with unit title)
                            columns.RelativeColumn(1.0f); // Rate
                            columns.RelativeColumn(1.3f); // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ITEM").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("QTY").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("RATE").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("TOTAL").Bold().FontSize(8f);

                            header.Cell().ColumnSpan(4).PaddingVertical(1).LineHorizontal(0.75f).LineColor(Colors.Black);
                        });

                        if (_lines != null && _lines.Count > 0)
                        {
                            for (int i = 0; i < _lines.Count; i++)
                            {
                                var line = _lines[i];

                                table.Cell().PaddingVertical(1.2f).Text(t =>
                                {
                                    t.Span(line.Item).FontSize(8f).SemiBold();
                                    if (!string.IsNullOrWhiteSpace(line.Imei))
                                    {
                                        t.Element().PaddingTop(0.5f).Text("IMEI: " + line.Imei).FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                                    }
                                });

                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedQtyWithUnit).FontSize(8f).SemiBold();
                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedRate).FontSize(8f).SemiBold();
                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedAmount).FontSize(8.5f).Bold();
                            }
                        }
                        else
                        {
                            table.Cell().ColumnSpan(4).AlignCenter().PaddingVertical(3).Text("No line items.").FontSize(8f);
                        }
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    // 5. Financial Summary
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Gross Total:").FontSize(8f).SemiBold();
                        r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.GrossTotal)).FontSize(8.5f).SemiBold();
                    });

                    if (_summary.TotalDiscount > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Discount:").FontSize(8f).SemiBold();
                            r.AutoItem().Text(string.Format("-Rs. {0:N2}", _summary.TotalDiscount)).FontSize(8.5f).SemiBold();
                        });
                    }

                    col.Item().PaddingVertical(1).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("NET TOTAL:").FontSize(9.5f).Bold();
                        r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.NetAmount)).FontSize(10f).Bold();
                    });

                    col.Item().PaddingVertical(1).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);

                    if (_summary.CashReceipt > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Cash Received:").FontSize(8f).SemiBold();
                            r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.CashReceipt)).FontSize(8.5f).SemiBold();
                        });
                    }

                    if (_summary.CashBack > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Change Return:").FontSize(8f).SemiBold();
                            r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.CashBack)).FontSize(8.5f).SemiBold();
                        });
                    }

                    if (_summary.Balance > 0)
                    {
                        col.Item().PaddingTop(1).Row(r =>
                        {
                            r.RelativeItem().Text("Remaining Due:").FontSize(8.5f).Bold();
                            r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.Balance)).FontSize(9f).Bold();
                        });
                    }

                    // 6. Thank you & Footer
                    col.Item().PaddingTop(4).AlignCenter().Text(_summary.ThankyouLine ?? "Thank you for your business!").FontSize(8f).Italic();
                    col.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                    col.Item().AlignCenter().PaddingTop(2).Text("Powered by BizGrip Solutions (0322-8258734)").FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                });
            });
        }

        #endregion

        #region A5 / A4 Composition

        private void ComposeA5(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5.Landscape());
                page.Margin(16, Unit.Point);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Segoe UI").FontColor(Colors.Grey.Darken4));

                page.Header().Element(ComposeStandardHeader);
                page.Content().Element(ComposeStandardContent);
                page.Footer().Element(ComposeStandardFooter);
            });
        }

        private void ComposeA4(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24, Unit.Point);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9f).FontFamily("Segoe UI").FontColor(Colors.Grey.Darken4));

                page.Header().Element(ComposeStandardHeader);
                page.Content().Element(ComposeStandardContent);
                page.Footer().Element(ComposeStandardFooter);
            });
        }

        private void ComposeStandardHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(!string.IsNullOrWhiteSpace(_summary?.CompanyName) ? _summary.CompanyName : "Retail Suite Enterprise")
                            .FontSize(14f).Bold().FontColor(Colors.Blue.Darken3);
                        if (!string.IsNullOrWhiteSpace(_summary?.CompanyAddress))
                            c.Item().Text(_summary.CompanyAddress).FontSize(8.5f).FontColor(Colors.Grey.Darken2);
                        if (!string.IsNullOrWhiteSpace(_summary?.CompanyPhone))
                            c.Item().Text("Tel: " + _summary.CompanyPhone).FontSize(8.5f).FontColor(Colors.Grey.Darken2);
                    });

                    row.AutoItem().Column(c =>
                    {
                        string title = _summary.IsDuplicate ? "SALE INVOICE (DUPLICATE)" : "SALE INVOICE";
                        c.Item().AlignRight().Text(title).FontSize(13f).Bold().FontColor(Colors.Grey.Darken4);
                        c.Item().AlignRight().Text("Voucher #: " + (_summary.VoucherNo ?? "")).FontSize(9f).Bold();
                        c.Item().AlignRight().Text(string.Format("Date: {0:dd-MMM-yyyy HH:mm}", _summary.Date)).FontSize(8.5f);
                    });
                });

                col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(Colors.Blue.Darken3);

                // Customer Info Banner
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Billed To: ").Bold();
                        t.Span(!string.IsNullOrWhiteSpace(_summary?.CustomerName) ? _summary.CustomerName : "Walk-in Customer").SemiBold();
                        if (!string.IsNullOrWhiteSpace(_summary?.CustomerAddress))
                        {
                            t.Span(" | " + _summary.CustomerAddress);
                        }
                    });

                    if (!string.IsNullOrWhiteSpace(_summary?.Cashier))
                    {
                        row.AutoItem().Text(t =>
                        {
                            t.Span("Cashier: ").Bold();
                            t.Span(_summary.Cashier);
                        });
                    }
                });

                col.Item().PaddingVertical(4).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            });
        }

        private void ComposeStandardContent(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);  // Seq
                        columns.RelativeColumn(3.5f); // Item
                        columns.RelativeColumn(1.2f); // Unit
                        columns.RelativeColumn(1.2f); // Qty
                        columns.RelativeColumn(1.3f); // Rate
                        columns.RelativeColumn(1.1f); // Discount
                        columns.RelativeColumn(1.5f); // Amount
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).Text("#").Bold();
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).Text("Item Description").Bold();
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).AlignCenter().Text("Unit").Bold();
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).AlignRight().Text("Qty").Bold();
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).AlignRight().Text("Rate").Bold();
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).AlignRight().Text("Disc").Bold();
                        header.Cell().Background(Colors.Grey.Lighten4).Padding(4).AlignRight().Text("Amount").Bold();

                        header.Cell().ColumnSpan(7).LineHorizontal(1f).LineColor(Colors.Grey.Darken2);
                    });

                    for (int i = 0; i < _lines.Count; i++)
                    {
                        var line = _lines[i];
                        var bg = (i % 2 == 1) ? Colors.Grey.Lighten5 : Colors.White;

                        table.Cell().Background(bg).Padding(4).Text((i + 1).ToString());
                        table.Cell().Background(bg).Padding(4).Text(t =>
                        {
                            t.Span(line.Item).SemiBold();
                            if (!string.IsNullOrWhiteSpace(line.Imei))
                            {
                                t.Element().Text(" [IMEI: " + line.Imei + "]").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                            }
                        });
                        table.Cell().Background(bg).Padding(4).AlignCenter().Text(line.Unit ?? "-");
                        table.Cell().Background(bg).Padding(4).AlignRight().Text(line.FormattedQty);
                        table.Cell().Background(bg).Padding(4).AlignRight().Text(line.FormattedRate);
                        table.Cell().Background(bg).Padding(4).AlignRight().Text(line.FormattedDiscount);
                        table.Cell().Background(bg).Padding(4).AlignRight().Text(line.FormattedAmount).SemiBold();
                    }
                });

                col.Item().PaddingTop(6).LineHorizontal(1f).LineColor(Colors.Grey.Darken2);

                // Financial Summary Block
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem(2).Column(c =>
                    {
                        if (!string.IsNullOrWhiteSpace(_summary?.Description))
                        {
                            c.Item().Text("Remarks: " + _summary.Description).FontSize(8.5f).Italic();
                        }
                        c.Item().PaddingTop(8).Text(_summary.ThankyouLine ?? "Thank you for your business!").FontSize(9f).Bold().FontColor(Colors.Blue.Darken3);
                    });

                    row.RelativeItem(1.5f).Border(1f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten5).Padding(8).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Gross Total:").FontSize(8.5f);
                            r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.GrossTotal)).FontSize(8.5f).SemiBold();
                        });

                        if (_summary.TotalDiscount > 0)
                        {
                            c.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Discount:").FontSize(8.5f);
                                r.AutoItem().Text(string.Format("-Rs. {0:N2}", _summary.TotalDiscount)).FontSize(8.5f).SemiBold();
                            });
                        }

                        c.Item().PaddingVertical(3).LineHorizontal(1f).LineColor(Colors.Grey.Darken1);

                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Net Amount:").Bold().FontSize(9.5f);
                            r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.NetAmount)).Bold().FontSize(10f);
                        });

                        if (_summary.CashReceipt > 0)
                        {
                            c.Item().PaddingTop(3).Row(r =>
                            {
                                r.RelativeItem().Text("Cash Received:").FontSize(8.5f);
                                r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.CashReceipt)).FontSize(8.5f).SemiBold();
                            });
                        }

                        if (_summary.CashBack > 0)
                        {
                            c.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Change:").FontSize(8.5f);
                                r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.CashBack)).FontSize(8.5f).SemiBold();
                            });
                        }

                        if (_summary.Balance > 0)
                        {
                            c.Item().PaddingTop(3).Row(r =>
                            {
                                r.RelativeItem().Text("Balance Due:").Bold().FontSize(9f);
                                r.AutoItem().Text(string.Format("Rs. {0:N2}", _summary.Balance)).Bold().FontSize(9.5f);
                            });
                        }
                    });
                });
            });
        }

        private void ComposeStandardFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                col.Item().PaddingTop(2).Row(r =>
                {
                    r.RelativeItem().Text("Powered by RetailSuite Enterprise").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                    r.AutoItem().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });
        }

        #endregion

        #region Direct Printing & Helpers

        /// <summary>
        /// Sends the QuestPDF document directly to a thermal receipt or laser printer without user dialogs.
        /// </summary>
        public static void PrintDirectToPrinter(IDocument document, string printerName)
        {
            var settings = new ImageGenerationSettings { RasterDpi = 203 };
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
                pd.PrintController = new StandardPrintController(); // Silent printing without WinForms dialog
                pd.PrintPage += (s, ev) =>
                {
                    if (pageIndex < images.Count)
                    {
                        using (var ms = new MemoryStream(images[pageIndex]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        {
                            System.Drawing.Rectangle bounds = ev.PageBounds;
                            ev.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            ev.Graphics.DrawImage(img, 0, 0, bounds.Width, (int)((float)img.Height * ((float)bounds.Width / (float)img.Width)));
                        }
                        pageIndex++;
                        ev.HasMorePages = pageIndex < images.Count;
                    }
                };

                pd.Print();
            }
        }

        /// <summary>
        /// Generates the QuestPDF document to a temporary file on disk for preview in WebView2.
        /// </summary>
        public async Task<string> GeneratePdfToTempFileAsync()
        {
            return await Task.Run(() =>
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "RetailSuite", "Receipts");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                string fileName = string.Format("Receipt_{0}_{1:yyyyMMdd_HHmmss}.pdf",
                    _summary?.VoucherNo?.Replace("-", "_") ?? "Draft",
                    DateTime.Now);
                string filePath = Path.Combine(tempDir, fileName);
                this.GeneratePdf(filePath);
                return filePath;
            });
        }

        #endregion
    }
}
