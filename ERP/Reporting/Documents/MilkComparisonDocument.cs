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
    public class MilkComparisonDocument : IDocument
    {
        private readonly MilkComparisonHeader _header;
        private readonly List<MilkComparisonLineItem> _lines;
        private readonly MilkComparisonSummary _summary;

        public MilkComparisonDocument(
            MilkComparisonHeader header,
            List<MilkComparisonLineItem> lines,
            MilkComparisonSummary summary)
        {
            _header = header ?? new MilkComparisonHeader();
            _lines = lines ?? new List<MilkComparisonLineItem>();
            _summary = summary ?? new MilkComparisonSummary();
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

                        titleCol.Item().Text("MILK PURCHASE VS SUPPLY & DISPATCH COMPARISON")
                            .FontSize(11)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken2);

                        titleCol.Item().Text(string.Format("PRODUCT: {0} ({1})", _header.ItemTitle.ToUpper(), _header.UnitTitle))
                            .FontSize(9.5f)
                            .Bold()
                            .FontColor(Colors.Indigo.Medium);
                    });

                    row.ConstantItem(260).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Background(Colors.Grey.Lighten4).Padding(6).Column(box =>
                        {
                            box.Item().Text(string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _header.FromDate, _header.ToDate))
                                .FontSize(8.5f)
                                .SemiBold();

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
                // KPI 1: Purchase
                row.RelativeItem().Background(Colors.Blue.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("TOTAL PURCHASE").FontSize(7.5f).Bold().FontColor(Colors.Blue.Darken2);
                    kpi.Item().Text(string.Format("{0:N1} {1}", _summary.TotalPurchaseQty, _header.UnitTitle)).FontSize(11).Bold().FontColor(Colors.Blue.Darken3);
                    kpi.Item().Text(string.Format("Cost: Rs. {0:N0} (@ {1:N1})", _summary.TotalPurchaseAmount, _summary.AvgPurchaseRate)).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(8);

                // KPI 2: Dispatched
                row.RelativeItem().Background(Colors.Green.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("TOTAL DISPATCHED").FontSize(7.5f).Bold().FontColor(Colors.Green.Darken2);
                    kpi.Item().Text(string.Format("{0:N1} {1}", _summary.TotalDispatchedQty, _header.UnitTitle)).FontSize(11).Bold().FontColor(Colors.Green.Darken3);
                    kpi.Item().Text(string.Format("Revenue: Rs. {0:N0} (@ {1:N1})", _summary.TotalSupplyAmount + _summary.TotalRegularSaleAmount, _summary.AvgSupplyRate)).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(8);

                // KPI 3: Net Diff Qty
                bool isSurplus = _summary.TotalNetDiffQty >= 0;
                string diffBg = isSurplus ? Colors.Amber.Lighten5 : Colors.Red.Lighten5;
                string diffFg = isSurplus ? Colors.Amber.Darken3 : Colors.Red.Darken2;

                row.RelativeItem().Background(diffBg).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("NET VARIANCE (QTY)").FontSize(7.5f).Bold().FontColor(diffFg);
                    kpi.Item().Text(string.Format("{0}{1:N1} {2}", _summary.TotalNetDiffQty > 0 ? "+" : "", _summary.TotalNetDiffQty, _header.UnitTitle))
                        .FontSize(11).Bold().FontColor(diffFg);
                    kpi.Item().Text(isSurplus ? "Surplus / Unsold Stock" : "Shortage / Deficit Deficit").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(8);

                // KPI 4: Margin / Diff Amount
                row.RelativeItem().Background(Colors.Teal.Lighten5).Padding(6).Column(kpi =>
                {
                    kpi.Item().Text("GROSS MARGIN / DIFF VALUE").FontSize(7.5f).Bold().FontColor(Colors.Teal.Darken2);
                    kpi.Item().Text(string.Format("Rs. {0:N0}", (_summary.TotalSupplyAmount + _summary.TotalRegularSaleAmount) - _summary.TotalPurchaseAmount))
                        .FontSize(11).Bold().FontColor(Colors.Teal.Darken3);
                    kpi.Item().Text(string.Format("Diff Val: Rs. {0:N0}", _summary.TotalDiffAmount)).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(85);  // Date & Day
                    cols.RelativeColumn(1.0f); // Purch Qty
                    cols.RelativeColumn(0.9f); // Purch Rate
                    cols.RelativeColumn(1.1f); // Purch Amount
                    cols.RelativeColumn(1.0f); // Supply Qty
                    cols.RelativeColumn(0.9f); // Supply Rate
                    cols.RelativeColumn(1.1f); // Supply Amount
                    cols.RelativeColumn(1.0f); // Daily Diff
                    cols.RelativeColumn(1.0f); // Net Diff Qty
                    cols.RelativeColumn(1.1f); // Diff Amount
                    cols.ConstantColumn(65);  // Status Tag
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("DATE & DAY");
                    header.Cell().Element(HeaderCellStyleRight).Text(string.Format("PURCHASE ({0})", _header.UnitTitle));
                    header.Cell().Element(HeaderCellStyleRight).Text("PURCH RATE");
                    header.Cell().Element(HeaderCellStyleRight).Text("PURCH AMOUNT");
                    header.Cell().Element(HeaderCellStyleRight).Text(string.Format("DISPATCH ({0})", _header.UnitTitle));
                    header.Cell().Element(HeaderCellStyleRight).Text("SALE RATE");
                    header.Cell().Element(HeaderCellStyleRight).Text("SALE AMOUNT");
                    header.Cell().Element(HeaderCellStyleRight).Text(string.Format("DAILY DIFF ({0})", _header.UnitTitle));
                    header.Cell().Element(HeaderCellStyleRight).Text(string.Format("NET CUMUL ({0})", _header.UnitTitle));
                    header.Cell().Element(HeaderCellStyleRight).Text("DIFF AMOUNT");
                    header.Cell().Element(HeaderCellStyleCenter).Text("STATUS");
                });

                // Rows
                for (int i = 0; i < _lines.Count; i++)
                {
                    var line = _lines[i];
                    bool isEven = i % 2 == 0;
                    var rowBg = isEven ? Colors.White : Colors.Grey.Lighten5;

                    // Date & Day
                    table.Cell().Element(c => CellStyle(c, rowBg)).Text(string.Format("{0:dd-MMM-yy} ({1})", line.Date, line.DayName));

                    // Purchase
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(line.PurchaseQty > 0 ? string.Format("{0:N1}", line.PurchaseQty) : "-").Bold().FontColor(Colors.Blue.Darken2);
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(line.PurchaseAvgRate > 0 ? string.Format("{0:N1}", line.PurchaseAvgRate) : "-");
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(line.PurchaseAmount > 0 ? string.Format("{0:N0}", line.PurchaseAmount) : "-");

                    // Supply / Dispatch
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(line.TotalDispatchedQty > 0 ? string.Format("{0:N1}", line.TotalDispatchedQty) : "-").Bold().FontColor(Colors.Green.Darken2);
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(line.SupplyAvgRate > 0 ? string.Format("{0:N1}", line.SupplyAvgRate) : "-");
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(line.SupplyAmount > 0 ? string.Format("{0:N0}", line.SupplyAmount) : "-");

                    // Daily Diff
                    string diffSign = line.DiffQty > 0 ? string.Format("+{0:N1}", line.DiffQty) : (line.DiffQty < 0 ? string.Format("{0:N1}", line.DiffQty) : "0.0");
                    string diffColor = line.DiffQty > 0 ? Colors.Green.Darken2 : (line.DiffQty < 0 ? Colors.Red.Darken2 : Colors.Grey.Darken2);
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(diffSign).SemiBold().FontColor(diffColor);

                    // Net Diff Cumulative
                    string netSign = line.NetDiffQty > 0 ? string.Format("+{0:N1}", line.NetDiffQty) : string.Format("{0:N1}", line.NetDiffQty);
                    string netColor = line.NetDiffQty > 0 ? Colors.Amber.Darken3 : (line.NetDiffQty < 0 ? Colors.Red.Darken2 : Colors.Grey.Darken2);
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(netSign).Bold().FontColor(netColor);

                    // Diff Amount
                    table.Cell().Element(c => CellStyleRight(c, rowBg)).Text(string.Format("{0:N0}", line.DiffAmount));

                    // Status Badge
                    table.Cell().Element(c => CellStyleCenter(c, rowBg)).Element(badgeCell =>
                    {
                        string badgeBg = Colors.Grey.Lighten3;
                        string badgeFg = Colors.Grey.Darken2;
                        string txt = line.Status ?? "Equal";

                        if (string.Equals(txt, "Surplus", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Amber.Lighten4;
                            badgeFg = Colors.Amber.Darken3;
                        }
                        else if (string.Equals(txt, "Shortage", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Red.Lighten4;
                            badgeFg = Colors.Red.Darken2;
                        }
                        else if (string.Equals(txt, "Equal", StringComparison.OrdinalIgnoreCase))
                        {
                            badgeBg = Colors.Green.Lighten4;
                            badgeFg = Colors.Green.Darken2;
                        }

                        badgeCell.Background(badgeBg).PaddingHorizontal(4).PaddingVertical(1).Text(txt)
                            .FontSize(7f).Bold().FontColor(badgeFg);
                    });
                }

                // Grand Total Row
                table.Cell().Element(TotalCellStyle).Text("GRAND TOTAL").Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N1}", _summary.TotalPurchaseQty)).Bold().FontColor(Colors.Blue.Darken3);
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N1}", _summary.AvgPurchaseRate)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalPurchaseAmount)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N1}", _summary.TotalDispatchedQty)).Bold().FontColor(Colors.Green.Darken3);
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N1}", _summary.AvgSupplyRate)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalSupplyAmount + _summary.TotalRegularSaleAmount)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N1}", _summary.TotalDiffQty)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N1}", _summary.TotalNetDiffQty)).Bold();
                table.Cell().Element(TotalCellStyleRight).Text(string.Format("{0:N0}", _summary.TotalDiffAmount)).Bold();
                table.Cell().Element(TotalCellStyleCenter).Text("-");
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text("Software powered by Bizgrip Solutions (Contact: 03228258734)")
                    .FontSize(7.5f)
                    .FontColor(Colors.Grey.Darken1);

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                    text.CurrentPageNumber().FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken3);
                    text.Span(" of ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                    text.TotalPages().FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken3);
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
            string tempPath = Path.Combine(Path.GetTempPath(), string.Format("MilkComparison_{0}_{1:yyyyMMddHHmmss}.pdf", _header.FromDate.ToString("yyyyMMdd"), DateTime.Now));
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
                g.DrawString(compName.ToUpper() + " - MILK COMPARISON REPORT", fontHeader, System.Drawing.Brushes.Black, 40, y);
                y += 22;
                g.DrawString(string.Format("Item: {0} ({1}) | Period: {2:dd-MMM-yyyy} to {3:dd-MMM-yyyy}", _header.ItemTitle, _header.UnitTitle, _header.FromDate, _header.ToDate), fontSub, System.Drawing.Brushes.Black, 40, y);
                y += 24;

                g.DrawLine(System.Drawing.Pens.Black, 40, y, ev.PageBounds.Width - 40, y);
                y += 6;

                // Column Headers
                g.DrawString("Date", fontBold, System.Drawing.Brushes.Black, 40, y);
                g.DrawString("Purch Qty", fontBold, System.Drawing.Brushes.Black, 160, y);
                g.DrawString("Purch Amt", fontBold, System.Drawing.Brushes.Black, 250, y);
                g.DrawString("Sale Qty", fontBold, System.Drawing.Brushes.Black, 350, y);
                g.DrawString("Sale Amt", fontBold, System.Drawing.Brushes.Black, 440, y);
                g.DrawString("Daily Diff", fontBold, System.Drawing.Brushes.Black, 540, y);
                g.DrawString("Net Cumul", fontBold, System.Drawing.Brushes.Black, 630, y);
                g.DrawString("Status", fontBold, System.Drawing.Brushes.Black, 720, y);
                y += 18;
                g.DrawLine(System.Drawing.Pens.Gray, 40, y, ev.PageBounds.Width - 40, y);
                y += 6;

                while (lineIndex < _lines.Count)
                {
                    var item = _lines[lineIndex];
                    g.DrawString(string.Format("{0:dd-MMM-yy} ({1})", item.Date, item.DayName), fontSub, System.Drawing.Brushes.Black, 40, y);
                    g.DrawString(string.Format("{0:N1}", item.PurchaseQty), fontSub, System.Drawing.Brushes.Black, 160, y);
                    g.DrawString(string.Format("{0:N0}", item.PurchaseAmount), fontSub, System.Drawing.Brushes.Black, 250, y);
                    g.DrawString(string.Format("{0:N1}", item.TotalDispatchedQty), fontSub, System.Drawing.Brushes.Black, 350, y);
                    g.DrawString(string.Format("{0:N0}", item.SupplyAmount), fontSub, System.Drawing.Brushes.Black, 440, y);
                    g.DrawString(string.Format("{0:N1}", item.DiffQty), fontSub, System.Drawing.Brushes.Black, 540, y);
                    g.DrawString(string.Format("{0:N1}", item.NetDiffQty), fontSub, System.Drawing.Brushes.Black, 630, y);
                    g.DrawString(item.Status ?? "Equal", fontBold, System.Drawing.Brushes.Black, 720, y);

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
                g.DrawString(string.Format("{0:N1}", _summary.TotalPurchaseQty), fontBold, System.Drawing.Brushes.Black, 160, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalPurchaseAmount), fontBold, System.Drawing.Brushes.Black, 250, y);
                g.DrawString(string.Format("{0:N1}", _summary.TotalDispatchedQty), fontBold, System.Drawing.Brushes.Black, 350, y);
                g.DrawString(string.Format("{0:N0}", _summary.TotalSupplyAmount + _summary.TotalRegularSaleAmount), fontBold, System.Drawing.Brushes.Black, 440, y);
                g.DrawString(string.Format("{0:N1}", _summary.TotalDiffQty), fontBold, System.Drawing.Brushes.Black, 540, y);
                g.DrawString(string.Format("{0:N1}", _summary.TotalNetDiffQty), fontBold, System.Drawing.Brushes.Black, 630, y);

                ev.HasMorePages = false;
            };

            printDoc.Print();
        }
    }
}
