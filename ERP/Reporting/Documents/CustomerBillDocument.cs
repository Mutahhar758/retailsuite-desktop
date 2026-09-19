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
    public enum CustomerBillPrintLayout
    {
        A4Sheet,
        Thermal80mm
    }

    /// <summary>
    /// Code-first QuestPDF document implementing the Customer Bill / Sale Receipt Statement.
    /// Supports corporate A4 invoice presentation as well as direct silent printing to physical/thermal printers.
    /// </summary>
    public class CustomerBillDocument : IDocument
    {
        private readonly CustomerBillSummary _summary;
        private readonly List<CustomerBillLineItem> _lines;
        private readonly CustomerBillPrintLayout _layout;

        public CustomerBillDocument(CustomerBillSummary summary, List<CustomerBillLineItem> lines, CustomerBillPrintLayout layout = CustomerBillPrintLayout.A4Sheet)
        {
            _summary = summary ?? new CustomerBillSummary();
            _lines = lines ?? new List<CustomerBillLineItem>();
            _layout = layout;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            if (_layout == CustomerBillPrintLayout.Thermal80mm)
            {
                ComposeThermal80(container);
            }
            else
            {
                ComposeA4(container);
            }
        }

        private void ComposeA4(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32, Unit.Point);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Segoe UI").FontColor(QuestPDF.Helpers.Colors.Grey.Darken4));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeThermal80(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(80, Unit.Millimetre);
                page.MarginVertical(2, Unit.Millimetre);
                page.MarginHorizontal(4, Unit.Millimetre);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8f).FontFamily("Arial").FontColor(QuestPDF.Helpers.Colors.Black));

                page.Content().Column(col =>
                {
                    string compName = !string.IsNullOrWhiteSpace(_summary?.CompanyName)
                        ? _summary.CompanyName
                        : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                    // 1. Store Header (Bold for emphasis)
                    col.Item().AlignCenter().Text(compName).FontSize(12f).Bold();

                    if (!string.IsNullOrWhiteSpace(_summary?.CompanyAddress))
                    {
                        col.Item().AlignCenter().PaddingTop(1).Text(_summary.CompanyAddress).FontSize(8f).SemiBold();
                    }

                    if (!string.IsNullOrWhiteSpace(_summary?.CompanyPhone))
                    {
                        col.Item().AlignCenter().PaddingTop(1).Text("Tel: " + _summary.CompanyPhone).FontSize(8f).SemiBold();
                    }

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // 2. Receipt Title
                    col.Item().AlignCenter().Text("CUSTOMER BILL / RECEIPT").FontSize(9f).Bold();

                    // 3. Customer & Meta
                    col.Item().PaddingTop(2).Row(r =>
                    {
                        r.AutoItem().Text("Customer: ").Bold().FontSize(8.5f);
                        r.RelativeItem().Text(_summary?.CustomerName ?? "Customer").Bold().FontSize(8.5f);
                    });

                    col.Item().Row(r =>
                    {
                        r.AutoItem().Text("Period: ").FontSize(8f).SemiBold();
                        r.RelativeItem().Text(string.Format("{0:dd/MM/yy} to {1:dd/MM/yy}", _summary.FromDate, _summary.ToDate)).FontSize(8f).SemiBold();
                    });

                    col.Item().Row(r =>
                    {
                        r.AutoItem().Text("Printed: ").FontSize(8f).SemiBold();
                        r.RelativeItem().Text(DateTime.Now.ToString("dd-MMM-yy HH:mm")).FontSize(8f).SemiBold();
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // 4. Line Items Table with Adj Column (Clean, sharp SemiBold weight)
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2.6f); // Item & Date
                            columns.RelativeColumn(0.9f); // Qty
                            columns.RelativeColumn(1.0f); // Rate
                            columns.RelativeColumn(1.0f); // Adj
                            columns.RelativeColumn(1.3f); // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ITEM").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("QTY").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("RATE").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("ADJ").Bold().FontSize(8f);
                            header.Cell().AlignRight().Text("TOTAL").Bold().FontSize(8.5f);

                            header.Cell().ColumnSpan(5).PaddingVertical(1).LineHorizontal(0.75f).LineColor(QuestPDF.Helpers.Colors.Black);
                        });

                        if (_lines != null && _lines.Count > 0)
                        {
                            for (int i = 0; i < _lines.Count; i++)
                            {
                                var line = _lines[i];

                                table.Cell().PaddingVertical(1.2f).Text(t =>
                                {
                                    t.Span(line.Date.ToString("dd/MM") + " ").FontSize(8.5f).SemiBold().FontColor(QuestPDF.Helpers.Colors.Black);
                                    t.Span(line.Item).FontSize(8.5f).SemiBold();
                                });
                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedQty).FontSize(9f).SemiBold();
                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedRate).FontSize(9f).SemiBold();
                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedAddLess).FontSize(9f).SemiBold();
                                table.Cell().AlignRight().PaddingVertical(1.2f).Text(line.FormattedAmount).FontSize(9.5f).SemiBold();
                            }
                        }
                        else
                        {
                            table.Cell().ColumnSpan(5).AlignCenter().PaddingVertical(3).Text("No line transactions in period.").FontSize(8.5f).SemiBold();
                        }
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // 5. Financial Summary
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Previous Balance:").FontSize(9f).SemiBold();
                        r.AutoItem().Text(_summary.PreviousBalance.ToString("#,##0")).FontSize(9.5f).SemiBold();
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Current Invoiced:").FontSize(9f).SemiBold();
                        r.AutoItem().Text(_summary.CurrentBillTotal.ToString("#,##0")).FontSize(9.5f).SemiBold();
                    });

                    if (_summary.PaymentsReceived != 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Payment / Recovery:").FontSize(9f).SemiBold();
                            r.AutoItem().Text(string.Format("({0:#,##0})", _summary.PaymentsReceived)).FontSize(9.5f).SemiBold();
                        });
                    }

                    col.Item().PaddingTop(1).LineHorizontal(0.75f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // Major focal point: NET DUE BALANCE in Bold
                    col.Item().PaddingTop(2).Row(r =>
                    {
                        r.RelativeItem().Text("NET DUE BALANCE:").Bold().FontSize(11f);
                        r.AutoItem().Text(_summary.NetBalance.ToString("#,##0")).Bold().FontSize(12f);
                    });

                    if (_summary.ShowQrPayment)
                    {
                        col.Item().PaddingTop(2).LineHorizontal(0.75f).LineColor(QuestPDF.Helpers.Colors.Black);
                        col.Item().AlignCenter().Text("SCAN TO PAY (ALL BANKS / RAAST)").FontSize(8f).Bold();
                        try
                        {
                            var qrBytes = ERP.Classes.QrCodeHelper.GeneratePng(_summary.QrPayment.BuildEmvCoPayload(_summary.NetBalance), 4);
                            if (qrBytes != null && qrBytes.Length > 0)
                            {
                                col.Item().AlignCenter().Width(100).Image(qrBytes);
                            }
                        }
                        catch { }
                        if (!string.IsNullOrWhiteSpace(_summary.QrPayment.BankName))
                            col.Item().AlignCenter().Text(_summary.QrPayment.BankName).FontSize(7.5f).Bold();
                        if (!string.IsNullOrWhiteSpace(_summary.QrPayment.AccountTitle))
                            col.Item().AlignCenter().Text(_summary.QrPayment.AccountTitle).FontSize(7.5f).SemiBold();
                        if (!string.IsNullOrWhiteSpace(_summary.QrPayment.AccountNumber))
                        {
                            string dispIban = ERP.Classes.QrPaymentInfo.FormatIban(ERP.Classes.QrPaymentInfo.NormalizeToIban(_summary.QrPayment.AccountNumber, _summary.QrPayment.BankName));
                            col.Item().AlignCenter().Text(dispIban).FontSize(7.5f).Bold();
                        }
                        col.Item().AlignCenter().Text("Amount: PKR " + _summary.NetBalance.ToString("#,##0")).FontSize(9.5f).Bold();
                    }

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // 6. Thankyou & Signatures
                    string thankLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine)
                        ? ConfigInfo.ThankyouLine
                        : "Thank you for your valued business!";

                    col.Item().AlignCenter().PaddingTop(2).Text(thankLine).Italic().FontSize(8f);
                    col.Item().AlignCenter().PaddingTop(1).Text("Software powered by Bizgrip Solutions (Contact: 03228258734)").FontSize(5.8f).SemiBold().FontColor(QuestPDF.Helpers.Colors.Black);
                });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                // Top Brand & Invoice Title
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(brandCol =>
                    {
                        string compName = !string.IsNullOrWhiteSpace(_summary?.CompanyName)
                            ? _summary.CompanyName
                            : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                        brandCol.Item().Text(compName)
                            .FontSize(18)
                            .Bold()
                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                        if (!string.IsNullOrWhiteSpace(_summary.CompanyAddress))
                        {
                            brandCol.Item().PaddingTop(2).Text(_summary.CompanyAddress)
                                .FontSize(8f)
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        }

                        if (!string.IsNullOrWhiteSpace(_summary.CompanyPhone))
                        {
                            brandCol.Item().PaddingTop(1).Text("Contact: " + _summary.CompanyPhone)
                                .FontSize(8f)
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        }
                    });

                    row.ConstantItem(260).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Text("CUSTOMER BILL / INVOICE")
                            .FontSize(13)
                            .Bold()
                            .FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                        metaCol.Item().PaddingTop(3).Text(string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _summary.FromDate, _summary.ToDate))
                            .FontSize(8.5f)
                            .SemiBold()
                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                        metaCol.Item().PaddingTop(2).Text(string.Format("Issue Date: {0:dd MMM yyyy, HH:mm}", _summary.GeneratedAt))
                            .FontSize(7.5f)
                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                        metaCol.Item().PaddingTop(1).Text(string.Format("Generated By: {0}", _summary.GeneratedBy ?? "System"))
                            .FontSize(7.5f)
                            .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(1f).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);

                // Customer Info Card
                col.Item().PaddingTop(8).PaddingBottom(8).Border(1f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                    .Background(QuestPDF.Helpers.Colors.Grey.Lighten5).Padding(8).Row(custRow =>
                    {
                        custRow.RelativeItem(3).Column(infoCol =>
                        {
                            infoCol.Item().Text("BILL TO CUSTOMER:")
                                .FontSize(7.5f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                            infoCol.Item().PaddingTop(2).Text(_summary.CustomerName ?? "Valued Customer")
                                .FontSize(11f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);

                            if (!string.IsNullOrWhiteSpace(_summary.CustomerAddress))
                            {
                                infoCol.Item().PaddingTop(2).Text(_summary.CustomerAddress)
                                    .FontSize(8f)
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            }

                            if (!string.IsNullOrWhiteSpace(_summary.CustomerPhone))
                            {
                                infoCol.Item().PaddingTop(1).Text("Phone: " + _summary.CustomerPhone)
                                    .FontSize(8f)
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            }
                        });

                        custRow.ConstantItem(180).AlignRight().Column(codeCol =>
                        {
                            codeCol.Item().Text("Previous Balance (B/F):")
                                .FontSize(7.5f)
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                            codeCol.Item().Text(_summary.PreviousBalance.ToString("#,##0.00"))
                                .FontSize(10f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);
                        });
                    });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(4).Column(col =>
            {
                // Line Items Table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(24);   // #
                        columns.ConstantColumn(68);   // Date
                        columns.ConstantColumn(72);   // Voucher #
                        columns.RelativeColumn();     // Item Description
                        columns.ConstantColumn(40);   // Unit
                        columns.ConstantColumn(44);   // Qty
                        columns.ConstantColumn(65);   // Rate
                        columns.ConstantColumn(58);   // Add / Less
                        columns.ConstantColumn(75);   // Amount
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).AlignCenter().Text("#");
                        header.Cell().Element(HeaderCell).Text("Date");
                        header.Cell().Element(HeaderCell).Text("Voucher #");
                        header.Cell().Element(HeaderCell).Text("Item Description");
                        header.Cell().Element(HeaderCell).AlignCenter().Text("Unit");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Qty");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Rate");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Add / Less");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Amount");
                    });

                    if (_lines.Count == 0)
                    {
                        table.Cell().ColumnSpan(9).Element(c => BodyCell(c, QuestPDF.Helpers.Colors.White))
                            .AlignCenter().PaddingVertical(14).Text("No billing transactions recorded in the selected period.").Italic().FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                    }
                    else
                    {
                        for (int i = 0; i < _lines.Count; i++)
                        {
                            var line = _lines[i];
                            var bg = (i % 2 == 0) ? QuestPDF.Helpers.Colors.White : QuestPDF.Helpers.Colors.Grey.Lighten5;

                            table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text((i + 1).ToString()).FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                            table.Cell().Element(c => BodyCell(c, bg)).Text(line.FormattedDate).FontSize(7.5f);
                            table.Cell().Element(c => BodyCell(c, bg)).Text(line.VNo ?? string.Empty).FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            table.Cell().Element(c => BodyCell(c, bg)).Text(line.Item ?? string.Empty).SemiBold();
                            table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text(line.Unit ?? string.Empty);
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedQty);
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedRate);
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedAddLess);
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedAmount).SemiBold();
                        }
                    }

                    // Total Current Bill Subtotal
                    table.Cell().ColumnSpan(8).Element(SubtotalCell).Text("CURRENT PERIOD BILL TOTAL:").Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                    table.Cell().Element(SubtotalCell).AlignRight().Text(_summary.CurrentBillTotal.ToString("#,##0")).Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);
                });

                col.Item().PaddingTop(12);

                // Financial Summary Breakdown & Net Balance Block
                col.Item().Row(summaryRow =>
                {
                    summaryRow.RelativeItem(3).Column(notesCol =>
                    {
                        if (!string.IsNullOrWhiteSpace(_summary.ThankyouLine))
                        {
                            notesCol.Item().PaddingTop(4).Border(1f).BorderColor(QuestPDF.Helpers.Colors.Blue.Lighten4)
                                .Background(QuestPDF.Helpers.Colors.Blue.Lighten5).Padding(8).Column(msgCol =>
                                {
                                    msgCol.Item().Text(_summary.ThankyouLine).FontSize(8.5f).Italic().FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                                    msgCol.Item().PaddingTop(2).Text("Please clear outstanding balances within the agreed credit terms.").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                });
                        }

                        if (_summary.ShowQrPayment)
                        {
                            notesCol.Item().PaddingTop(6).Element(ComposeQrPaymentSection);
                        }
                    });

                    summaryRow.ConstantItem(260).AlignRight().Column(recCol =>
                    {
                        recCol.Item().Table(recTable =>
                        {
                            recTable.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1.8f);
                                cols.RelativeColumn(1.4f);
                            });

                            // Previous Balance
                            recTable.Cell().Element(RecLabelCell).Text("Previous Balance:");
                            recTable.Cell().Element(RecValueCell).Text(_summary.PreviousBalance.ToString("#,##0"));

                            // Current Bill
                            recTable.Cell().Element(RecLabelCell).Text("Current Period Bill:");
                            recTable.Cell().Element(RecValueCell).Text(_summary.CurrentBillTotal.ToString("#,##0"));

                            // Gross Total
                            recTable.Cell().Element(RecLabelBoldCell).Text("Gross Total Payable:");
                            recTable.Cell().Element(RecValueBoldCell).Text(_summary.GrossTotal.ToString("#,##0"));

                            // Payments
                            recTable.Cell().Element(RecLabelCell).Text("Less Payments Received:");
                            recTable.Cell().Element(RecValueCell).Text("(" + _summary.PaymentsReceived.ToString("#,##0") + ")").FontColor(QuestPDF.Helpers.Colors.Green.Darken3);

                            // Net Balance Due
                            recTable.Cell().Element(GrandNetLabelCell).Text("NET BALANCE DUE:").Bold();
                            recTable.Cell().Element(GrandNetValueCell).Text(_summary.NetBalance.ToString("#,##0")).Bold();
                        });
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("Software powered by Bizgrip Solutions (Contact: 03228258734)")
                        .FontSize(7.5f)
                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        x.CurrentPageNumber().FontSize(7.5f).SemiBold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                        x.Span(" of ").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        x.TotalPages().FontSize(7.5f).SemiBold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                    });
                });
            });
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container
                .Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                .BorderBottom(1f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken2)
                .PaddingVertical(4)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(7.5f).Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken3));
        }

        private static IContainer BodyCell(IContainer container, string backgroundColor)
        {
            return container
                .Background(backgroundColor)
                .BorderBottom(0.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten3)
                .PaddingVertical(3.5f)
                .PaddingHorizontal(4);
        }

        private static IContainer SubtotalCell(IContainer container)
        {
            return container
                .BorderTop(1f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken2)
                .BorderBottom(1f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken2)
                .Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                .PaddingVertical(4)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(8f));
        }

        private static IContainer RecLabelCell(IContainer container)
        {
            return container
                .PaddingVertical(2.5f)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(8f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2));
        }

        private static IContainer RecValueCell(IContainer container)
        {
            return container
                .AlignRight()
                .PaddingVertical(2.5f)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(8f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken4));
        }

        private static IContainer RecLabelBoldCell(IContainer container)
        {
            return container
                .BorderTop(0.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                .PaddingVertical(3f)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(8f).Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken3));
        }

        private static IContainer RecValueBoldCell(IContainer container)
        {
            return container
                .AlignRight()
                .BorderTop(0.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                .PaddingVertical(3f)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(8f).Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken4));
        }

        private static IContainer GrandNetLabelCell(IContainer container)
        {
            return container
                .BorderTop(1.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken3)
                .BorderBottom(2.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken3)
                .Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                .PaddingVertical(5)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(9f).Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken4));
        }

        private static IContainer GrandNetValueCell(IContainer container)
        {
            return container
                .AlignRight()
                .BorderTop(1.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken3)
                .BorderBottom(2.5f)
                .BorderColor(QuestPDF.Helpers.Colors.Grey.Darken3)
                .Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                .PaddingVertical(5)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(9.5f).Bold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken3));
        }

        private void ComposeQrPaymentSection(IContainer container)
        {
            byte[] qrBytes = null;
            try
            {
                qrBytes = ERP.Classes.QrCodeHelper.GeneratePng(_summary.QrPayment.BuildEmvCoPayload(_summary.NetBalance), 4);
            }
            catch
            {
                qrBytes = null;
            }

            container.Border(1f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                .Background(QuestPDF.Helpers.Colors.Grey.Lighten5)
                .Padding(8)
                .Row(row =>
                {
                    if (qrBytes != null && qrBytes.Length > 0)
                    {
                        row.ConstantItem(90).AlignCenter().Image(qrBytes);
                    }

                    row.RelativeItem().PaddingLeft(10).Column(infoCol =>
                    {
                        infoCol.Item().Text("SCAN TO PAY VIA ANY BANK APP").FontSize(8.5f).Bold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                        if (!string.IsNullOrWhiteSpace(_summary.QrPayment.BankName))
                        {
                            infoCol.Item().PaddingTop(2).Text(t =>
                            {
                                t.Span("Bank: ").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                                t.Span(_summary.QrPayment.BankName).FontSize(7.5f).Bold();
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(_summary.QrPayment.AccountTitle))
                        {
                            infoCol.Item().Text(t =>
                            {
                                t.Span("Title: ").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                                t.Span(_summary.QrPayment.AccountTitle).FontSize(7.5f).SemiBold();
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(_summary.QrPayment.AccountNumber))
                        {
                            string dispIban = ERP.Classes.QrPaymentInfo.FormatIban(ERP.Classes.QrPaymentInfo.NormalizeToIban(_summary.QrPayment.AccountNumber, _summary.QrPayment.BankName));
                            infoCol.Item().Text(t =>
                            {
                                t.Span("A/C or IBAN: ").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                                t.Span(dispIban).FontSize(7.5f).Bold();
                            });
                        }

                        infoCol.Item().PaddingTop(1).Text(t =>
                        {
                            t.Span("Amount Pre-filled: ").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                            t.Span("PKR " + _summary.NetBalance.ToString("#,##0")).FontSize(8f).Bold().FontColor(QuestPDF.Helpers.Colors.Green.Darken3);
                        });

                        infoCol.Item().PaddingTop(3).Text("Works with Meezan, HBL, Alfalah, Easypaisa, JazzCash & all Raast banks")
                            .FontSize(6.5f).Italic().FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                    });
                });
        }

        /// <summary>
        /// Asynchronously renders the document to a temporary PDF file and returns its path.
        /// </summary>
        public async Task<string> GeneratePdfToTempFileAsync()
        {
            return await Task.Run(() =>
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "RetailSuite", "Reports");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                string filePath = Path.Combine(tempDir, string.Format("CustomerBill_{0}_{1:yyyyMMdd_HHmmss}_{2}.pdf", 
                    !string.IsNullOrWhiteSpace(_summary.CustomerCode) ? _summary.CustomerCode : "Batch",
                    DateTime.Now, 
                    Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }

        /// <summary>
        /// Direct-to-printer silent printing engine using GDI+ PrintDocument.
        /// Completely suppresses Windows popups and dialogs via StandardPrintController.
        /// </summary>
        public static void PrintDirectToPrinter(IDocument document, string printerName)
        {
            var settings = new ImageGenerationSettings { RasterDpi = 300 };
            var pageImages = document.GenerateImages(settings).ToList();
            if (pageImages.Count == 0) return;

            int pageIndex = 0;
            using (var pd = new PrintDocument())
            {
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    pd.PrinterSettings.PrinterName = printerName;
                }

                // Explicitly zero out GDI+ margins to prevent Windows from applying default 1-inch (100-unit) borders
                pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                pd.OriginAtMargins = false;

                // If thermal receipt roll, auto-detect and select 80mm paper size from printer driver
                foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
                {
                    if (ps.Width >= 270 && ps.Width <= 325)
                    {
                        pd.DefaultPageSettings.PaperSize = ps;
                        break;
                    }
                }

                pd.PrintController = new StandardPrintController(); // Silent mode: suppresses "Printing page X..." pop-up
                pd.PrintPage += (sender, ev) =>
                {
                    if (pageIndex < pageImages.Count)
                    {
                        using (var ms = new MemoryStream(pageImages[pageIndex]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        {
                            ev.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            ev.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            ev.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            ev.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                            // If continuous/tall roll layout (e.g. 80mm thermal receipt)
                            if (img.Height > img.Width * 1.3f)
                            {
                                // Physical printhead on 80mm thermal printers is 72mm (~283 hundredths of an inch).
                                // Total roll width is 80mm (~315 hundredths of an inch).
                                // Using targetWidth = 275 hundredths of an inch (~70mm):
                                // 1. Guarantees it never shrinks to 1 inch (we never check ev.MarginBounds).
                                // 2. Prevents the right-side columns (ADJ and TOTAL) from falling outside the 72mm thermal printhead.
                                float targetWidth = 275f;
                                float targetHeight = img.Width > 0 ? (float)img.Height * (targetWidth / (float)img.Width) : ev.PageBounds.Height;
                                ev.Graphics.DrawImage(img, new System.Drawing.RectangleF(0, 0, targetWidth, targetHeight));
                            }
                            else
                            {
                                System.Drawing.Rectangle bounds = ev.PageBounds;
                                if (bounds.Width <= 0 || bounds.Height <= 0)
                                {
                                    bounds = ev.MarginBounds;
                                }
                                ev.Graphics.DrawImage(img, bounds);
                            }
                        }
                        pageIndex++;
                        ev.HasMorePages = pageIndex < pageImages.Count;
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

    /// <summary>
    /// Combined multi-customer batch document for rendering all selected customer bills together.
    /// Each customer bill begins on a fresh page.
    /// </summary>
    public class CustomerBillBatchDocument : IDocument
    {
        private readonly List<CustomerBillDataResult> _batch;
        private readonly CustomerBillPrintLayout _layout;

        public CustomerBillBatchDocument(List<CustomerBillDataResult> batch, CustomerBillPrintLayout layout = CustomerBillPrintLayout.A4Sheet)
        {
            _batch = batch ?? new List<CustomerBillDataResult>();
            _layout = layout;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            for (int i = 0; i < _batch.Count; i++)
            {
                var bill = _batch[i];
                var singleDoc = new CustomerBillDocument(bill.Summary, bill.Lines, _layout);
                singleDoc.Compose(container);
            }
        }

        public async Task<string> GeneratePdfToTempFileAsync()
        {
            return await Task.Run(() =>
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "RetailSuite", "Reports");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                string filePath = Path.Combine(tempDir, string.Format("CustomerBillBatch_{0:yyyyMMdd_HHmmss}_{1}.pdf",
                    DateTime.Now,
                    Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }
    }
}
