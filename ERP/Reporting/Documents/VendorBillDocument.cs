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
    public enum VendorBillPrintLayout
    {
        A4Sheet,
        Thermal80mm
    }

    /// <summary>
    /// Code-first QuestPDF document implementing the Vendor Bill / Purchase Statement.
    /// Supports corporate A4 invoice presentation as well as direct silent printing to physical/thermal printers.
    /// </summary>
    public class VendorBillDocument : IDocument
    {
        private readonly VendorBillSummary _summary;
        private readonly List<VendorBillLineItem> _lines;
        private readonly VendorBillPrintLayout _layout;

        public VendorBillDocument(VendorBillSummary summary, List<VendorBillLineItem> lines, VendorBillPrintLayout layout = VendorBillPrintLayout.A4Sheet)
        {
            _summary = summary ?? new VendorBillSummary();
            _lines = lines ?? new List<VendorBillLineItem>();
            _layout = layout;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            if (_summary.IsWandaLayout)
            {
                if (_layout == VendorBillPrintLayout.Thermal80mm)
                {
                    ComposeWandaThermal80(container);
                }
                else
                {
                    ComposeWandaA4(container);
                }
                return;
            }

            if (_layout == VendorBillPrintLayout.Thermal80mm)
            {
                ComposeThermal80(container);
            }
            else
            {
                ComposeA4(container);
            }
        }

        #region Normal Layout - A4

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

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                // Top Brand & Report Title
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
                        metaCol.Item().Text("VENDOR BILL / STATEMENT")
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

                // Vendor Info Card
                col.Item().PaddingTop(8).PaddingBottom(8).Border(1f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                    .Background(QuestPDF.Helpers.Colors.Grey.Lighten5).Padding(8).Row(custRow =>
                    {
                        custRow.RelativeItem(3).Column(infoCol =>
                        {
                            infoCol.Item().Text("VENDOR / SUPPLIER:")
                                .FontSize(7.5f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                            infoCol.Item().PaddingTop(2).Text(_summary.VendorName ?? "Valued Supplier")
                                .FontSize(11f)
                                .Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);

                            if (!string.IsNullOrWhiteSpace(_summary.VendorAddress))
                            {
                                infoCol.Item().PaddingTop(2).Text(_summary.VendorAddress)
                                    .FontSize(8f)
                                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            }

                            if (!string.IsNullOrWhiteSpace(_summary.VendorPhone))
                            {
                                infoCol.Item().PaddingTop(1).Text("Phone: " + _summary.VendorPhone)
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
                        if (_summary.EnableCarriage)
                            columns.ConstantColumn(58);   // Carriage
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
                        if (_summary.EnableCarriage)
                            header.Cell().Element(HeaderCell).AlignRight().Text("Carriage");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Add / Less");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Amount");
                    });

                    if (_lines.Count == 0)
                    {
                        table.Cell().ColumnSpan((uint)(_summary.EnableCarriage ? 10 : 9)).Element(c => BodyCell(c, QuestPDF.Helpers.Colors.White))
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
                            if (_summary.EnableCarriage)
                                table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedCarriage);
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedAddLess);
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(line.FormattedAmount).SemiBold();
                        }
                    }

                    // Total Current Bill Subtotal
                    table.Cell().ColumnSpan((uint)(_summary.EnableCarriage ? 9 : 8)).Element(SubtotalCell).Text("CURRENT PERIOD PURCHASES TOTAL:").Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
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
                                    msgCol.Item().Text(_summary.ThankyouLine).FontSize(8.5f).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                                    msgCol.Item().PaddingTop(2).Text("Verified vendor billing and purchase ledger statement.").FontSize(7.5f).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                });
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
                            recTable.Cell().Element(RecLabelCell).Text("Current Period Purchases:");
                            recTable.Cell().Element(RecValueCell).Text(_summary.CurrentBillTotal.ToString("#,##0"));

                            // Gross Total
                            recTable.Cell().Element(RecLabelBoldCell).Text("Gross Total Payable:");
                            recTable.Cell().Element(RecValueBoldCell).Text(_summary.GrossTotal.ToString("#,##0"));

                            // Payments Made
                            recTable.Cell().Element(RecLabelCell).Text("Less Payments Made:");
                            recTable.Cell().Element(RecValueCell).Text("(" + _summary.PaymentsMade.ToString("#,##0") + ")").FontColor(QuestPDF.Helpers.Colors.Green.Darken3);

                            // Net Balance Payable
                            recTable.Cell().Element(GrandNetLabelCell).Text("NET BALANCE PAYABLE:").Bold();
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

        #endregion

        #region Normal Layout - 80mm Thermal Receipt

        private void ComposeThermal80(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(80, Unit.Millimetre);
                page.MarginVertical(2, Unit.Millimetre);
                page.MarginLeft(2.5f, Unit.Millimetre);
                page.MarginRight(8.5f, Unit.Millimetre);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8f).FontFamily("Arial").FontColor(QuestPDF.Helpers.Colors.Black));

                page.Content().Column(col =>
                {
                    string compName = !string.IsNullOrWhiteSpace(_summary?.CompanyName)
                        ? _summary.CompanyName
                        : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                    // 1. Store Header
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
                    col.Item().AlignCenter().Text("VENDOR BILL / STATEMENT").FontSize(9f).Bold();

                    // 3. Vendor & Meta
                    col.Item().PaddingTop(2).Row(r =>
                    {
                        r.AutoItem().Text("Vendor: ").Bold().FontSize(8.5f);
                        r.RelativeItem().Text(_summary?.VendorName ?? "Supplier").Bold().FontSize(8.5f);
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

                    // 4. Line Items Table with Adj Column
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2.5f); // Item & Date
                            columns.RelativeColumn(0.9f); // Qty
                            columns.RelativeColumn(1.0f); // Rate
                            columns.RelativeColumn(0.9f); // Adj
                            columns.RelativeColumn(1.4f); // Total
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
                        r.RelativeItem().Text("Purchases Total:").FontSize(9f).SemiBold();
                        r.AutoItem().Text(_summary.CurrentBillTotal.ToString("#,##0")).FontSize(9.5f).SemiBold();
                    });

                    if (_summary.PaymentsMade != 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Payments Made:").FontSize(9f).SemiBold();
                            r.AutoItem().Text(string.Format("({0:#,##0})", _summary.PaymentsMade)).FontSize(9.5f).SemiBold();
                        });
                    }

                    col.Item().PaddingTop(1).LineHorizontal(0.75f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // NET PAYABLE BALANCE in Bold
                    col.Item().PaddingTop(2).Row(r =>
                    {
                        r.RelativeItem().Text("NET PAYABLE BALANCE:").Bold().FontSize(11f);
                        r.AutoItem().Text(_summary.NetBalance.ToString("#,##0")).Bold().FontSize(12f);
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(QuestPDF.Helpers.Colors.Black);

                    // 6. Thankyou & Signatures
                    string thankLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine)
                        ? ConfigInfo.ThankyouLine
                        : "Thank you for your business!";

                    col.Item().AlignCenter().PaddingTop(2).Text(thankLine).FontSize(8.5f).SemiBold();
                    col.Item().AlignCenter().PaddingTop(1).Text("Software powered by Bizgrip Solutions (Contact: 03228258734)").FontSize(5.8f).SemiBold().FontColor(QuestPDF.Helpers.Colors.Black);
                });
            });
        }

        #endregion

        #region Wanda / Feed Mill Layout

        private void ComposeWandaA4(IDocumentContainer container)
        {
            decimal totalPurchKg = _lines.Where(x => x.TransactionType == "Purchase" || string.IsNullOrEmpty(x.TransactionType)).Sum(x => x.Qty);
            decimal totalPurchBags = _lines.Where(x => x.TransactionType == "Purchase" || string.IsNullOrEmpty(x.TransactionType)).Sum(x => x.SecQty ?? (x.QtyInPack.HasValue && x.QtyInPack.Value > 0 ? Math.Round(x.Qty / x.QtyInPack.Value) : 0));
            decimal totalPurchAmount = _summary.TotalPurchases > 0 ? _summary.TotalPurchases : _lines.Where(x => x.TransactionType == "Purchase" || string.IsNullOrEmpty(x.TransactionType)).Sum(x => Math.Abs(x.Amount));

            decimal totalBill = _summary.CurrentBillTotal;
            decimal previousBal = _summary.PreviousBalance;
            decimal grandTotal = previousBal + totalBill;
            decimal payments = _summary.PaymentsMade;
            decimal netBalance = _summary.NetBalance;

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(14, Unit.Point);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Arial").FontColor(Colors.Black));

                page.Content().Border(1f).BorderColor(Colors.Black).CornerRadius(6).Padding(10).Column(col =>
                {
                    // 1. Company Name
                    col.Item().AlignCenter().Text((_summary.CompanyName ?? "COMPANY").ToUpper())
                        .FontSize(18).Bold().FontColor(Colors.Black);

                    // 2. Report Subtitle
                    col.Item().AlignCenter().PaddingTop(2).Text("VENDOR BILL & STATEMENT")
                        .FontSize(11).Bold().FontColor(Colors.Black);

                    // 3. Vendor Info, Date Range, Previous Balance
                    col.Item().PaddingTop(10).Row(metaRow =>
                    {
                        metaRow.RelativeItem().Column(infoCol =>
                        {
                            infoCol.Item().Text(t =>
                            {
                                t.Span("Vendor / Supplier: ").Bold();
                                t.Span(string.Format("{0} ({1})", _summary.VendorName, _summary.VendorCode)).SemiBold();
                            });
                            infoCol.Item().PaddingTop(2).Text(t =>
                            {
                                t.Span("Date: ").Bold();
                                t.Span(string.Format("{0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _summary.FromDate, _summary.ToDate)).SemiBold();
                            });
                        });

                        metaRow.AutoItem().AlignRight().Text(t =>
                        {
                            t.Span("Previous Balance: ").Bold();
                            t.Span(string.Format("Rs. {0:N2}", previousBal)).SemiBold();
                        });
                    });

                    col.Item().PaddingTop(8);

                    // 4. 11/12-column Table matching exact layout
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(38);   // Date
                            columns.ConstantColumn(46);   // Voucher No
                            columns.RelativeColumn();     // Description
                            columns.ConstantColumn(46);   // Weight (Kg)
                            columns.ConstantColumn(30);   // Bags
                            columns.ConstantColumn(38);   // Kg Rate
                            columns.ConstantColumn(38);   // Bag Rate
                            if (_summary.EnableCarriage)
                                columns.ConstantColumn(36); // Carriage
                            columns.ConstantColumn(36);   // Add/Less
                            columns.ConstantColumn(50);   // Amount
                            columns.ConstantColumn(44);   // Payment Date
                            columns.ConstantColumn(48);   // Payment Amount
                        });

                        table.Header(header =>
                        {
                            IContainer WandaHeaderCell(IContainer c) =>
                                c.Border(0.75f).BorderColor(Colors.Black).PaddingVertical(3).PaddingHorizontal(1.5f);

                            header.Cell().Element(WandaHeaderCell).AlignCenter().Text("Date").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignCenter().Text("Voucher No").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).Text("Description").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Weight (Kg)").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Bags").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Kg Rate").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Bag Rate").Bold().FontSize(7.5f);
                            if (_summary.EnableCarriage)
                                header.Cell().Element(WandaHeaderCell).AlignRight().Text("Carriage").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Add/Less").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Amount").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignCenter().Text("Pay Date").Bold().FontSize(7.5f);
                            header.Cell().Element(WandaHeaderCell).AlignRight().Text("Payment").Bold().FontSize(7.5f);
                        });

                        IContainer WandaBodyCell(IContainer c) =>
                            c.Border(0.5f).BorderColor(Colors.Black).PaddingVertical(2).PaddingHorizontal(2f);

                        if (_lines.Count == 0)
                        {
                            uint totalCols = (uint)(_summary.EnableCarriage ? 12 : 11);
                            table.Cell().ColumnSpan(totalCols).Element(WandaBodyCell)
                                .AlignCenter().PaddingVertical(10).Text("No billing transactions recorded in the selected period.").Italic();
                        }
                        else
                        {
                            for (int i = 0; i < _lines.Count; i++)
                            {
                                var line = _lines[i];

                                decimal bagQty = line.SecQty ?? (line.QtyInPack.HasValue && line.QtyInPack.Value > 0 ? Math.Round(line.Qty / line.QtyInPack.Value) : 0);
                                decimal bagRate = line.SecRate ?? (line.QtyInPack.HasValue && line.QtyInPack.Value > 0 ? (line.Rate * line.QtyInPack.Value) : 0);

                                table.Cell().Element(WandaBodyCell).AlignCenter().Text(line.Date != DateTime.MinValue ? line.Date.ToString("dd-MMM-yy") : string.Empty).FontSize(7f);
                                table.Cell().Element(WandaBodyCell).AlignCenter().Text(line.VNo ?? string.Empty).FontSize(7f);
                                table.Cell().Element(WandaBodyCell).Text(line.Item ?? string.Empty).FontSize(7.5f);

                                table.Cell().Element(WandaBodyCell).AlignRight().Text(line.Qty != 0 ? line.Qty.ToString("#,##0") : "-").FontSize(7.5f);
                                table.Cell().Element(WandaBodyCell).AlignRight().Text(bagQty != 0 ? bagQty.ToString("#,##0") : "-").FontSize(7.5f);
                                table.Cell().Element(WandaBodyCell).AlignRight().Text(line.Rate != 0 ? line.Rate.ToString("#,##0.00") : "-").FontSize(7.5f);
                                table.Cell().Element(WandaBodyCell).AlignRight().Text(bagRate != 0 ? bagRate.ToString("#,##0.00") : "-").FontSize(7.5f);

                                if (_summary.EnableCarriage)
                                    table.Cell().Element(WandaBodyCell).AlignRight().Text(line.Carriage != 0 ? line.Carriage.ToString("#,##0") : "-").FontSize(7.5f);

                                table.Cell().Element(WandaBodyCell).AlignRight().Text(line.AddLess != 0 ? line.AddLess.ToString("#,##0") : "-").FontSize(7.5f);
                                table.Cell().Element(WandaBodyCell).AlignRight().Text(line.Amount.ToString("#,##0;(#,##0);0")).FontSize(7.5f).Bold();

                                table.Cell().Element(WandaBodyCell).AlignCenter().Text(line.ReceiptDate.HasValue && line.ReceiptDate.Value != DateTime.MinValue ? line.ReceiptDate.Value.ToString("dd-MMM-yy") : "-").FontSize(7f);
                                table.Cell().Element(WandaBodyCell).AlignRight().Text(line.ReceiptAmount.HasValue && line.ReceiptAmount.Value != 0 ? line.ReceiptAmount.Value.ToString("#,##0") : "-").FontSize(7.5f);
                            }
                        }

                        // Subtotal Row
                        IContainer WandaSubtotalCell(IContainer c) =>
                            c.Border(0.75f).BorderColor(Colors.Black).Background(Colors.Grey.Lighten4).PaddingVertical(3).PaddingHorizontal(2f);

                        table.Cell().ColumnSpan(3).Element(WandaSubtotalCell).AlignRight().Text("TOTAL:").Bold().FontSize(7.5f);
                        table.Cell().Element(WandaSubtotalCell).AlignRight().Text(_lines.Sum(x => x.Qty).ToString("#,##0")).Bold().FontSize(7.5f);
                        table.Cell().Element(WandaSubtotalCell).AlignRight().Text(_lines.Sum(x => x.SecQty ?? (x.QtyInPack.HasValue && x.QtyInPack.Value > 0 ? Math.Round(x.Qty / x.QtyInPack.Value) : 0)).ToString("#,##0")).Bold().FontSize(7.5f);
                        table.Cell().Element(WandaSubtotalCell).Text("");
                        table.Cell().Element(WandaSubtotalCell).Text("");

                        if (_summary.EnableCarriage)
                            table.Cell().Element(WandaSubtotalCell).AlignRight().Text(_lines.Sum(x => x.Carriage).ToString("#,##0")).Bold().FontSize(7.5f);

                        table.Cell().Element(WandaSubtotalCell).AlignRight().Text(_lines.Sum(x => x.AddLess).ToString("#,##0")).Bold().FontSize(7.5f);
                        table.Cell().Element(WandaSubtotalCell).AlignRight().Text(_summary.CurrentBillTotal.ToString("#,##0")).Bold().FontSize(8f);
                        table.Cell().Element(WandaSubtotalCell).Text("");
                        table.Cell().Element(WandaSubtotalCell).AlignRight().Text(_lines.Where(x => x.ReceiptAmount.HasValue).Sum(x => x.ReceiptAmount.Value).ToString("#,##0")).Bold().FontSize(7.5f);
                    });

                    col.Item().PaddingTop(12);

                    // 5. 3-Column Summary Cards at the bottom
                    col.Item().Row(summaryRow =>
                    {
                        summaryRow.Spacing(10);

                        // Card 1: Purchase Summary
                        summaryRow.RelativeItem().Border(1f).BorderColor(Colors.Black).Padding(6).Column(card1 =>
                        {
                            card1.Item().AlignCenter().Text("PURCHASE SUMMARY").Bold().FontSize(8.5f);
                            card1.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Black);

                            card1.Item().PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Total Purch Kg:").FontSize(8f);
                                r.AutoItem().Text(totalPurchKg.ToString("#,##0")).Bold().FontSize(8f);
                            });

                            card1.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Total Purch Bags:").FontSize(8f);
                                r.AutoItem().Text(totalPurchBags.ToString("#,##0")).Bold().FontSize(8f);
                            });

                            card1.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Total Purchases:").FontSize(8f);
                                r.AutoItem().Text(string.Format("Rs. {0:N0}", totalPurchAmount)).Bold().FontSize(8f);
                            });
                        });

                        // Card 2: Period Billing Reconciliation
                        summaryRow.RelativeItem().Border(1f).BorderColor(Colors.Black).Padding(6).Column(card2 =>
                        {
                            card2.Item().AlignCenter().Text("BILLING RECONCILIATION").Bold().FontSize(8.5f);
                            card2.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Black);

                            card2.Item().PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Purchases:").FontSize(8f);
                                r.AutoItem().Text(string.Format("{0:N0}", totalPurchAmount)).FontSize(8f);
                            });

                            if (_summary.TotalPurchaseReturns > 0)
                            {
                                card2.Item().PaddingTop(2).Row(r =>
                                {
                                    r.RelativeItem().Text("Less Purch Returns:").FontSize(8f);
                                    r.AutoItem().Text(string.Format("({0:N0})", _summary.TotalPurchaseReturns)).FontSize(8f);
                                });
                            }

                            if (_summary.TotalSales > 0)
                            {
                                card2.Item().PaddingTop(2).Row(r =>
                                {
                                    r.RelativeItem().Text("Less Sales:").FontSize(8f);
                                    r.AutoItem().Text(string.Format("({0:N0})", _summary.TotalSales)).FontSize(8f);
                                });
                            }

                            if (_summary.TotalSaleReturns > 0)
                            {
                                card2.Item().PaddingTop(2).Row(r =>
                                {
                                    r.RelativeItem().Text("Add Sale Returns:").FontSize(8f);
                                    r.AutoItem().Text(string.Format("{0:N0}", _summary.TotalSaleReturns)).FontSize(8f);
                                });
                            }

                            card2.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Black);
                            card2.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Net Period Bill:").Bold().FontSize(8f);
                                r.AutoItem().Text(string.Format("Rs. {0:N0}", totalBill)).Bold().FontSize(8f);
                            });
                        });

                        // Card 3: Final Payable Balance
                        summaryRow.RelativeItem().Border(1f).BorderColor(Colors.Black).Padding(6).Column(card3 =>
                        {
                            card3.Item().AlignCenter().Text("BALANCE STATEMENT").Bold().FontSize(8.5f);
                            card3.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Black);

                            card3.Item().PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Previous Balance:").FontSize(8f);
                                r.AutoItem().Text(string.Format("{0:N0}", previousBal)).FontSize(8f);
                            });

                            card3.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Add Net Period Bill:").FontSize(8f);
                                r.AutoItem().Text(string.Format("{0:N0}", totalBill)).FontSize(8f);
                            });

                            card3.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Gross Balance:").FontSize(8f).SemiBold();
                                r.AutoItem().Text(string.Format("{0:N0}", grandTotal)).FontSize(8f).SemiBold();
                            });

                            card3.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("Less Payments Made:").FontSize(8f);
                                r.AutoItem().Text(string.Format("({0:N0})", payments)).FontSize(8f);
                            });

                            card3.Item().PaddingTop(3).LineHorizontal(1f).LineColor(Colors.Black);
                            card3.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeItem().Text("NET PAYABLE:").Bold().FontSize(9f);
                                r.AutoItem().Text(string.Format("Rs. {0:N0}", netBalance)).Bold().FontSize(9.5f);
                            });
                        });
                    });

                    // 6. Signature Block
                    col.Item().PaddingTop(20).Row(sigRow =>
                    {
                        sigRow.RelativeItem().Column(c =>
                        {
                            c.Item().LineHorizontal(0.75f).LineColor(Colors.Black);
                            c.Item().PaddingTop(2).AlignCenter().Text("Prepared By").FontSize(7.5f).SemiBold();
                        });

                        sigRow.ConstantItem(40);

                        sigRow.RelativeItem().Column(c =>
                        {
                            c.Item().LineHorizontal(0.75f).LineColor(Colors.Black);
                            c.Item().PaddingTop(2).AlignCenter().Text("Checked By").FontSize(7.5f).SemiBold();
                        });

                        sigRow.ConstantItem(40);

                        sigRow.RelativeItem().Column(c =>
                        {
                            c.Item().LineHorizontal(0.75f).LineColor(Colors.Black);
                            c.Item().PaddingTop(2).AlignCenter().Text("Vendor Signature").FontSize(7.5f).SemiBold();
                        });
                    });

                    // 7. Footer
                    col.Item().PaddingTop(8).AlignCenter().Text("Software powered by Bizgrip Solutions (Contact: 03228258734)")
                        .FontSize(6.5f).SemiBold().FontColor(Colors.Grey.Darken1);
                });
            });
        }

        private void ComposeWandaThermal80(IDocumentContainer container)
        {
            decimal totalPurchKg = _lines.Where(x => x.TransactionType == "Purchase" || string.IsNullOrEmpty(x.TransactionType)).Sum(x => x.Qty);
            decimal totalPurchBags = _lines.Where(x => x.TransactionType == "Purchase" || string.IsNullOrEmpty(x.TransactionType)).Sum(x => x.SecQty ?? (x.QtyInPack.HasValue && x.QtyInPack.Value > 0 ? Math.Round(x.Qty / x.QtyInPack.Value) : 0));
            decimal totalPurchAmount = _summary.TotalPurchases > 0 ? _summary.TotalPurchases : _lines.Where(x => x.TransactionType == "Purchase" || string.IsNullOrEmpty(x.TransactionType)).Sum(x => Math.Abs(x.Amount));

            decimal totalBill = _summary.CurrentBillTotal;
            decimal previousBal = _summary.PreviousBalance;
            decimal grandTotal = previousBal + totalBill;
            decimal payments = _summary.PaymentsMade;
            decimal netBalance = _summary.NetBalance;

            container.Page(page =>
            {
                page.ContinuousSize(80, Unit.Millimetre);
                page.MarginVertical(2, Unit.Millimetre);
                page.MarginLeft(2.5f, Unit.Millimetre);
                page.MarginRight(8.5f, Unit.Millimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8f).FontFamily("Arial").FontColor(Colors.Black));

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().AlignCenter().Text(_summary.CompanyName ?? "COMPANY").FontSize(12f).Bold();
                    col.Item().AlignCenter().PaddingTop(1).Text("WANDA / FEED VENDOR BILL").FontSize(9f).Bold();

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    col.Item().Row(r =>
                    {
                        r.AutoItem().Text("Vendor: ").Bold().FontSize(8.5f);
                        r.RelativeItem().Text(_summary.VendorName).Bold().FontSize(8.5f);
                    });
                    col.Item().Row(r =>
                    {
                        r.AutoItem().Text("Period: ").FontSize(8f).SemiBold();
                        r.RelativeItem().Text(string.Format("{0:dd/MM/yy} to {1:dd/MM/yy}", _summary.FromDate, _summary.ToDate)).FontSize(8f).SemiBold();
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    // Items table (Item, Weight, Bags, Rate, Amount)
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2.0f); // Item & Date
                            cols.RelativeColumn(1.0f); // Kg
                            cols.RelativeColumn(0.7f); // Bags
                            cols.RelativeColumn(0.9f); // Rate
                            cols.RelativeColumn(1.3f); // Total
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("ITEM").Bold().FontSize(7.5f);
                            h.Cell().AlignRight().Text("KG").Bold().FontSize(7.5f);
                            h.Cell().AlignRight().Text("BAG").Bold().FontSize(7.5f);
                            h.Cell().AlignRight().Text("RATE").Bold().FontSize(7.5f);
                            h.Cell().AlignRight().Text("TOTAL").Bold().FontSize(8f);
                            h.Cell().ColumnSpan(5).PaddingVertical(1).LineHorizontal(0.75f).LineColor(Colors.Black);
                        });

                        foreach (var line in _lines)
                        {
                            decimal bagQty = line.SecQty ?? (line.QtyInPack.HasValue && line.QtyInPack.Value > 0 ? Math.Round(line.Qty / line.QtyInPack.Value) : 0);
                            table.Cell().PaddingVertical(1).Text(t =>
                            {
                                t.Span(string.Format("{0:dd/MM} ", line.Date)).FontSize(7.5f).SemiBold();
                                t.Span(line.Item).FontSize(8f).SemiBold();
                            });
                            table.Cell().AlignRight().PaddingVertical(1).Text(line.Qty.ToString("#,##0")).FontSize(8f).SemiBold();
                            table.Cell().AlignRight().PaddingVertical(1).Text(bagQty > 0 ? bagQty.ToString("#,##0") : "-").FontSize(8f).SemiBold();
                            table.Cell().AlignRight().PaddingVertical(1).Text(string.Format("{0:N0}", line.Rate)).FontSize(8f).SemiBold();
                            table.Cell().AlignRight().PaddingVertical(1).Text(line.Amount.ToString("#,##0;(#,##0);0")).FontSize(8.5f).Bold();
                        }
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    // Summary
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Total Purch Kg:").FontSize(8.5f).SemiBold();
                        r.AutoItem().Text(totalPurchKg.ToString("#,##0")).FontSize(8.5f).Bold();
                    });
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Total Purch Bags:").FontSize(8.5f).SemiBold();
                        r.AutoItem().Text(totalPurchBags.ToString("#,##0")).FontSize(8.5f).Bold();
                    });
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Previous Balance:").FontSize(8.5f).SemiBold();
                        r.AutoItem().Text(string.Format("{0:N0}", previousBal)).FontSize(8.5f).SemiBold();
                    });
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Total Purchases:").FontSize(8.5f).SemiBold();
                        r.AutoItem().Text(string.Format("{0:N0}", totalPurchAmount)).FontSize(8.5f).SemiBold();
                    });
                    if (_summary.TotalPurchaseReturns > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Less Purch Returns:").FontSize(8f);
                            r.AutoItem().Text(string.Format("({0:N0})", _summary.TotalPurchaseReturns)).FontSize(8f);
                        });
                    }
                    if (_summary.TotalSales > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Less Sales:").FontSize(8f);
                            r.AutoItem().Text(string.Format("({0:N0})", _summary.TotalSales)).FontSize(8f);
                        });
                    }
                    if (_summary.TotalSaleReturns > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Add Sale Returns:").FontSize(8f);
                            r.AutoItem().Text(string.Format("{0:N0}", _summary.TotalSaleReturns)).FontSize(8f);
                        });
                    }
                    if (_summary.TotalPurchaseReturns > 0 || _summary.TotalSales > 0 || _summary.TotalSaleReturns > 0)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Net Bill:").FontSize(8.5f).SemiBold();
                            r.AutoItem().Text(string.Format("{0:N0}", totalBill)).FontSize(8.5f).SemiBold();
                        });
                    }
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Payments:").FontSize(8.5f).SemiBold();
                        r.AutoItem().Text(string.Format("{0:N0}", payments)).FontSize(8.5f).SemiBold();
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(1f).LineColor(Colors.Black);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("NET PAYABLE:").FontSize(10f).Bold();
                        r.AutoItem().Text(string.Format("Rs. {0:N0}", netBalance)).FontSize(11f).Bold();
                    });

                    col.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Black);
                    col.Item().AlignCenter().Text("Software powered by Bizgrip Solutions").FontSize(6f).SemiBold();
                });
            });
        }

        #endregion

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

                string filePath = Path.Combine(tempDir, string.Format("VendorBill_{0}_{1:yyyyMMdd_HHmmss}_{2}.pdf", 
                    !string.IsNullOrWhiteSpace(_summary.VendorCode) ? _summary.VendorCode : "Batch",
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
            var settings = new ImageGenerationSettings { RasterDpi = 203 };
            var pageImages = document.GenerateImages(settings).ToList();
            if (pageImages.Count == 0) return;

            int pageIndex = 0;
            using (var pd = new PrintDocument())
            {
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    pd.PrinterSettings.PrinterName = printerName;
                }

                pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                pd.OriginAtMargins = false;

                System.Drawing.Printing.PaperSize maxRollSize = null;
                foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
                {
                    if (ps.Width >= 270 && ps.Width <= 325)
                    {
                        if (maxRollSize == null || ps.Height > maxRollSize.Height)
                        {
                            maxRollSize = ps;
                        }
                    }
                }
                if (maxRollSize != null)
                {
                    pd.DefaultPageSettings.PaperSize = maxRollSize;
                }

                pd.PrintController = new StandardPrintController();
                pd.PrintPage += (sender, ev) =>
                {
                    if (pageIndex < pageImages.Count)
                    {
                        using (var ms = new MemoryStream(pageImages[pageIndex]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        using (var ia = new System.Drawing.Imaging.ImageAttributes())
                        {
                            float contrast = 1.8f;
                            float delta = 0.5f * (1.0f - contrast) - 0.08f;
                            var cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
                            {
                                new float[] { contrast, 0, 0, 0, 0 },
                                new float[] { 0, contrast, 0, 0, 0 },
                                new float[] { 0, 0, contrast, 0, 0 },
                                new float[] { 0, 0, 0, 1, 0 },
                                new float[] { delta, delta, delta, 0, 1 }
                            });
                            ia.SetColorMatrix(cm, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                            ev.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                            ev.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                            ev.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                            ev.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                            if (img.Height > img.Width * 1.3f)
                            {
                                float targetWidth = 275f;
                                float targetHeight = img.Width > 0 ? (float)img.Height * (targetWidth / (float)img.Width) : ev.PageBounds.Height;
                                var destRect = new System.Drawing.Rectangle(0, 0, (int)Math.Ceiling(targetWidth), (int)Math.Ceiling(targetHeight));
                                ev.Graphics.DrawImage(img, destRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
                            }
                            else
                            {
                                System.Drawing.Rectangle bounds = ev.PageBounds;
                                if (bounds.Width <= 0 || bounds.Height <= 0)
                                {
                                    bounds = ev.MarginBounds;
                                }
                                ev.Graphics.DrawImage(img, bounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
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
    /// Combined multi-vendor batch document for rendering all selected vendor bills together.
    /// Each vendor bill begins on a fresh page.
    /// </summary>
    public class VendorBillBatchDocument : IDocument
    {
        private readonly List<VendorBillDataResult> _batch;
        private readonly VendorBillPrintLayout _layout;

        public VendorBillBatchDocument(List<VendorBillDataResult> batch, VendorBillPrintLayout layout = VendorBillPrintLayout.A4Sheet)
        {
            _batch = batch ?? new List<VendorBillDataResult>();
            _layout = layout;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            for (int i = 0; i < _batch.Count; i++)
            {
                var bill = _batch[i];
                var singleDoc = new VendorBillDocument(bill.Summary, bill.Lines, _layout);
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

                string filePath = Path.Combine(tempDir, string.Format("VendorBillBatch_{0:yyyyMMdd_HHmmss}_{1}.pdf",
                    DateTime.Now,
                    Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }
    }
}
