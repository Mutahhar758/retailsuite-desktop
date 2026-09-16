using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using ERP.Reporting.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERP.Reporting.Documents
{
    public class CustomerBalanceRecoveryDocument : IDocument
    {
        private readonly CustomerBalanceRecoveryHeader _header;
        private readonly List<CustomerBalanceRecoveryLineItem> _lines;
        private readonly CustomerBalanceRecoverySummary _summary;

        public CustomerBalanceRecoveryDocument(
            CustomerBalanceRecoveryHeader header,
            List<CustomerBalanceRecoveryLineItem> lines,
            CustomerBalanceRecoverySummary summary)
        {
            _header = header ?? new CustomerBalanceRecoveryHeader();
            _lines = lines ?? new List<CustomerBalanceRecoveryLineItem>();
            _summary = summary ?? new CustomerBalanceRecoverySummary();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20, Unit.Point);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Segoe UI").FontColor(Colors.Grey.Darken4));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(titleCol =>
                    {
                        string compName = !string.IsNullOrWhiteSpace(_header.CompanyName) ? _header.CompanyName : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");
                        titleCol.Item().Text(compName.ToUpper())
                            .FontSize(15)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        titleCol.Item().Text("CUSTOMER BALANCE & RECOVERY RECONCILIATION")
                            .FontSize(11)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken2);

                        string basisText = string.Equals(_header.DateBasis, "ClearingDate", StringComparison.OrdinalIgnoreCase)
                            ? "Reconciled by Bank/Cash Clearing Date"
                            : "Standard Voucher Entry Date";

                        titleCol.Item().Text(string.Format("ACCOUNTING BASIS: {0} • FILTER: {1}", basisText.ToUpper(), _header.BalanceFilter.ToUpper()))
                            .FontSize(8.5f)
                            .Bold()
                            .FontColor(Colors.Teal.Darken2);
                    });

                    row.ConstantItem(270).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Background(Colors.Grey.Lighten4).Padding(6).Column(box =>
                        {
                            box.Item().Text(string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _header.FromDate, _header.ToDate))
                                .FontSize(8.5f)
                                .SemiBold();

                            box.Item().Text(string.Format("Target: {0}", _header.CustomerFilter))
                                .FontSize(8f)
                                .FontColor(Colors.Grey.Darken2);

                            box.Item().Text(string.Format("Generated: {0:dd-MMM-yyyy hh:mm tt}", _header.GeneratedAt))
                                .FontSize(7.5f)
                                .FontColor(Colors.Grey.Darken1);
                        });
                    });
                });

                col.Item().PaddingTop(6).Element(ComposeKpiStrip);
                col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        }

        private void ComposeKpiStrip(IContainer container)
        {
            container.Row(row =>
            {
                // KPI 1: Previous Balance
                row.RelativeItem().Background(Colors.Blue.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("PREV RECEIVABLES").FontSize(7.5f).Bold().FontColor(Colors.Blue.Darken2);
                    kpi.Item().Text(string.Format("Rs. {0:N0}", _summary.TotalPreviousBalance)).FontSize(11).Bold().FontColor(Colors.Blue.Darken3);
                    kpi.Item().Text(string.Format("{0} Customers", _summary.TotalCustomers)).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(8);

                // KPI 2: Current Billing
                row.RelativeItem().Background(Colors.Indigo.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("PERIOD BILLING").FontSize(7.5f).Bold().FontColor(Colors.Indigo.Darken2);
                    kpi.Item().Text(string.Format("Rs. {0:N0}", _summary.TotalCurrentBilling)).FontSize(11).Bold().FontColor(Colors.Indigo.Darken3);
                    kpi.Item().Text(string.Format("Total Due: Rs. {0:N0}", _summary.TotalDue)).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(8);

                // KPI 3: Recovery Collected
                row.RelativeItem().Background(Colors.Green.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("TOTAL RECOVERED").FontSize(7.5f).Bold().FontColor(Colors.Green.Darken2);
                    kpi.Item().Text(string.Format("Rs. {0:N0}", _summary.TotalRecovery)).FontSize(11).Bold().FontColor(Colors.Green.Darken3);
                    kpi.Item().Text(string.Format("Recovery Rate: {0:N1}%", _summary.OverallRecoveryRate)).FontSize(7.5f).Bold().FontColor(Colors.Green.Darken3);
                });

                row.ConstantItem(8);

                // KPI 4: Closing Outstanding
                row.RelativeItem().Background(Colors.Red.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("CLOSING RECEIVABLE").FontSize(7.5f).Bold().FontColor(Colors.Red.Darken2);
                    kpi.Item().Text(string.Format("Rs. {0:N0}", _summary.TotalClosingBalance)).FontSize(11).Bold().FontColor(Colors.Red.Darken3);
                    kpi.Item().Text(string.Format("Discount: Rs. {0:N0}", _summary.TotalDiscount)).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(26);  // #
                    cols.RelativeColumn(2.2f); // Customer Name & Code
                    cols.RelativeColumn(1.3f); // Phone & City
                    cols.RelativeColumn(1.1f); // Previous Bal
                    cols.RelativeColumn(1.1f); // Current Billing
                    cols.RelativeColumn(1.1f); // Total Due
                    cols.RelativeColumn(1.1f); // Recovery Amount
                    cols.RelativeColumn(1.1f); // Closing Balance
                    cols.ConstantColumn(58);  // Recovery %
                    cols.ConstantColumn(60);  // Status
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("#");
                    header.Cell().Element(HeaderCellStyle).Text("CUSTOMER ACCOUNT & TITLE");
                    header.Cell().Element(HeaderCellStyle).Text("CONTACT / PHONE");
                    header.Cell().Element(HeaderCellStyleRight).Text("PREV BALANCE");
                    header.Cell().Element(HeaderCellStyleRight).Text("CURRENT BILL");
                    header.Cell().Element(HeaderCellStyleRight).Text("TOTAL DUE");
                    header.Cell().Element(HeaderCellStyleRight).Text("RECOVERED");
                    header.Cell().Element(HeaderCellStyleRight).Text("CLOSING BAL");
                    header.Cell().Element(HeaderCellStyleCenter).Text("REC %");
                    header.Cell().Element(HeaderCellStyleCenter).Text("STATUS");
                });

                // Rows
                for (int i = 0; i < _lines.Count; i++)
                {
                    var item = _lines[i];
                    bool isEven = i % 2 == 0;
                    var rowBg = isEven ? Colors.White : Colors.Grey.Lighten5;

                    // #
                    table.Cell().Element(c => CellStyle(c, rowBg)).Text(string.Format("{0}", i + 1)).FontSize(8).FontColor(Colors.Grey.Darken1);

                    // Customer Name
                    table.Cell().Element(c => CellStyle(c, rowBg)).Column(cc =>
                    {
                        cc.Item().Text(item.CustomerTitle).Bold().FontSize(8.5f);
                        if (!string.IsNullOrWhiteSpace(item.CustomerAccountId))
                        {
                            cc.Item().Text(item.CustomerAccountId).FontSize(7f).FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // Phone & Address
                    table.Cell().Element(c => CellStyle(c, rowBg)).Column(cc =>
                    {
                        cc.Item().Text(item.Phone ?? "-").FontSize(8f);
                        if (!string.IsNullOrWhiteSpace(item.Address))
                        {
                            string shortAddr = item.Address.Length > 28 ? item.Address.Substring(0, 26) + "..." : item.Address;
                            cc.Item().Text(shortAddr).FontSize(7f).FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // Previous Balance
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(string.Format("{0:N0}", item.PreviousBalance));

                    // Current Billing
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(string.Format("{0:N0}", item.CurrentBilling)).FontColor(Colors.Blue.Darken2);

                    // Total Due
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(string.Format("{0:N0}", item.TotalDue)).Bold();

                    // Recovery Amount
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(string.Format("{0:N0}", item.RecoveryAmount)).Bold().FontColor(Colors.Green.Darken2);

                    // Closing Balance
                    string balColor = item.ClosingBalance > 0 ? Colors.Red.Darken2 : Colors.Grey.Darken3;
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(string.Format("{0:N0}", item.ClosingBalance)).Bold().FontColor(balColor);

                    // Recovery %
                    table.Cell().Element(c => CellStyleCenter(c, rowBg)).Text(string.Format("{0:N0}%", item.RecoveryPercentage)).Bold();

                    // Status Pill
                    table.Cell().Element(c => CellStyleCenter(c, rowBg)).Element(badgeCell =>
                    {
                        string badgeBg = Colors.Grey.Lighten3;
                        string badgeFg = Colors.Grey.Darken2;
                        string st = item.Status ?? "Unpaid";

                        if (string.Equals(st, "Cleared", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Green.Lighten4;
                            badgeFg = Colors.Green.Darken3;
                        }
                        else if (string.Equals(st, "Partial", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Amber.Lighten4;
                            badgeFg = Colors.Amber.Darken3;
                        }
                        else if (string.Equals(st, "Unpaid", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Red.Lighten4;
                            badgeFg = Colors.Red.Darken2;
                        }
                        else if (string.Equals(st, "Advance", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Blue.Lighten4;
                            badgeFg = Colors.Blue.Darken3;
                        }

                        badgeCell.Background(badgeBg).PaddingHorizontal(4).PaddingVertical(1).Text(st)
                            .FontSize(7f).Bold().FontColor(badgeFg);
                    });
                }

                // Grand Total Row
                table.Cell().Element(TotalCellStyle).Text("");
                table.Cell().Element(TotalCellStyle).Text(string.Format("GRAND TOTAL ({0} ACCOUNTS)", _lines.Count)).Bold();
                table.Cell().Element(TotalCellStyle).Text("");
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalPreviousBalance)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalCurrentBilling)).Bold().FontColor(Colors.Blue.Darken3);
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalDue)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalRecovery)).Bold().FontColor(Colors.Green.Darken3);
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalClosingBalance)).Bold().FontColor(Colors.Red.Darken3);
                table.Cell().Element(TotalCellStyleCenter).Text(string.Format("{0:N1}%", _summary.OverallRecoveryRate)).Bold().FontColor(Colors.Green.Darken3);
                table.Cell().Element(TotalCellStyleCenter).Text("-");
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text(string.Format("Retail Suite Financial Audit Core • Customer Recovery Schedule • {0:yyyy}", DateTime.Today))
                    .FontSize(7.5f)
                    .FontColor(Colors.Grey.Medium);

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.CurrentPageNumber().FontSize(8).Bold();
                    text.Span(" / ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        }

        private IContainer HeaderCellStyle(IContainer container) =>
            container.Background(Colors.Blue.Darken3).Padding(5).AlignLeft().AlignMiddle()
                .DefaultTextStyle(x => x.Bold().FontSize(7.5f).FontColor(Colors.White));

        private IContainer HeaderCellStyleRight(IContainer container) =>
            container.Background(Colors.Blue.Darken3).Padding(5).AlignRight().AlignMiddle()
                .DefaultTextStyle(x => x.Bold().FontSize(7.5f).FontColor(Colors.White));

        private IContainer HeaderCellStyleCenter(IContainer container) =>
            container.Background(Colors.Blue.Darken3).Padding(5).AlignCenter().AlignMiddle()
                .DefaultTextStyle(x => x.Bold().FontSize(7.5f).FontColor(Colors.White));

        private IContainer CellStyle(IContainer container, string bg) =>
            container.Background(bg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(4).AlignLeft().AlignMiddle();

        private IContainer CellStyleRight(IContainer container, string bg) =>
            container.Background(bg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(4).AlignRight().AlignMiddle();

        private IContainer CellStyleCenter(IContainer container, string bg) =>
            container.Background(bg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(4).AlignCenter().AlignMiddle();

        private IContainer TotalCellStyle(IContainer container) =>
            container.BorderTop(1.5f).BorderBottom(2.5f).BorderColor(Colors.Grey.Darken3).Padding(5).AlignLeft().AlignMiddle();

        private IContainer TotalCellStyleRight(IContainer container) =>
            container.BorderTop(1.5f).BorderBottom(2.5f).BorderColor(Colors.Grey.Darken3).Padding(5).AlignRight().AlignMiddle();

        private IContainer TotalCellStyleCenter(IContainer container) =>
            container.BorderTop(1.5f).BorderBottom(2.5f).BorderColor(Colors.Grey.Darken3).Padding(5).AlignCenter().AlignMiddle();

        public async Task<string> GeneratePdfToTempFileAsync()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), string.Format("CustomerRecovery_{0}_{1:yyyyMMddHHmmss}.pdf", _header.FromDate.ToString("yyyyMMdd"), DateTime.Now));
            await Task.Run(() => this.GeneratePdf(tempPath));
            return tempPath;
        }

        public void PrintDirectToPrinter(string printerName)
        {
            var printDoc = new PrintDocument();
            if (!string.IsNullOrWhiteSpace(printerName))
            {
                printDoc.PrinterSettings.PrinterName = printerName;
            }

            printDoc.DefaultPageSettings.Landscape = true;
            printDoc.PrintController = new StandardPrintController();

            int lineIndex = 0;
            printDoc.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                float y = 40;
                var fontHeader = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
                var fontSub = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Regular);
                var fontBold = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);

                string compName = !string.IsNullOrWhiteSpace(_header.CompanyName) ? _header.CompanyName : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");
                g.DrawString(compName.ToUpper() + " - CUSTOMER RECOVERY REPORT", fontHeader, System.Drawing.Brushes.Black, 40, y);
                y += 22;
                g.DrawString(string.Format("Basis: {0} | Filter: {1} | Period: {2:dd-MMM-yyyy} to {3:dd-MMM-yyyy}", _header.DateBasis, _header.BalanceFilter, _header.FromDate, _header.ToDate), fontSub, System.Drawing.Brushes.Black, 40, y);
                y += 24;

                g.DrawLine(System.Drawing.Pens.Black, 40, y, ev.PageBounds.Width - 40, y);
                y += 6;

                // Column Headers
                g.DrawString("Customer Account", fontBold, System.Drawing.Brushes.Black, 40, y);
                g.DrawString("Prev Bal", fontBold, System.Drawing.Brushes.Black, 280, y);
                g.DrawString("Billing", fontBold, System.Drawing.Brushes.Black, 380, y);
                g.DrawString("Total Due", fontBold, System.Drawing.Brushes.Black, 480, y);
                g.DrawString("Recovered", fontBold, System.Drawing.Brushes.Black, 580, y);
                g.DrawString("Closing Bal", fontBold, System.Drawing.Brushes.Black, 680, y);
                g.DrawString("Status", fontBold, System.Drawing.Brushes.Black, 780, y);
                y += 18;
                g.DrawLine(System.Drawing.Pens.Gray, 40, y, ev.PageBounds.Width - 40, y);
                y += 6;

                while (lineIndex < _lines.Count)
                {
                    var item = _lines[lineIndex];
                    string title = item.CustomerTitle.Length > 28 ? item.CustomerTitle.Substring(0, 26) + ".." : item.CustomerTitle;
                    g.DrawString(title, fontSub, System.Drawing.Brushes.Black, 40, y);
                    g.DrawString(string.Format("{0:N0}", item.PreviousBalance), fontSub, System.Drawing.Brushes.Black, 280, y);
                    g.DrawString(string.Format("{0:N0}", item.CurrentBilling), fontSub, System.Drawing.Brushes.Black, 380, y);
                    g.DrawString(string.Format("{0:N0}", item.TotalDue), fontSub, System.Drawing.Brushes.Black, 480, y);
                    g.DrawString(string.Format("{0:N0}", item.RecoveryAmount), fontBold, System.Drawing.Brushes.Black, 580, y);
                    g.DrawString(string.Format("{0:N0}", item.ClosingBalance), fontBold, System.Drawing.Brushes.Black, 680, y);
                    g.DrawString(item.Status ?? "Unpaid", fontSub, System.Drawing.Brushes.Black, 780, y);

                    y += 18;
                    lineIndex++;

                    if (y > ev.PageBounds.Height - 60 && lineIndex < _lines.Count)
                    {
                        ev.HasMorePages = true;
                        return;
                    }
                }

                // Summary
                y += 6;
                g.DrawLine(System.Drawing.Pens.Black, 40, y, ev.PageBounds.Width - 40, y);
                y += 8;
                g.DrawString("TOTALS:", fontBold, System.Drawing.Brushes.Black, 40, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalPreviousBalance), fontBold, System.Drawing.Brushes.Black, 280, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalCurrentBilling), fontBold, System.Drawing.Brushes.Black, 380, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalDue), fontBold, System.Drawing.Brushes.Black, 480, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalRecovery), fontBold, System.Drawing.Brushes.Black, 580, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalClosingBalance), fontBold, System.Drawing.Brushes.Black, 680, y);

                ev.HasMorePages = false;
            };

            printDoc.Print();
        }
    }
}
