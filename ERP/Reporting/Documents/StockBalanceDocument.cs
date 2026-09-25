using System;
using System.Collections.Generic;
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
    /// Code-first QuestPDF document implementing an executive, modern Stock Balance & Inventory Valuation report.
    /// Follows international enterprise inventory audit and valuation standards.
    /// </summary>
    public class StockBalanceDocument : IDocument
    {
        private readonly StockBalanceHeader _header;
        private readonly List<StockBalanceReportItem> _items;
        private readonly bool _showStockValue;

        public StockBalanceDocument(StockBalanceHeader header, List<StockBalanceReportItem> items, bool showStockValue = false)
        {
            _header = header ?? new StockBalanceHeader();
            _items = items ?? new List<StockBalanceReportItem>();
            _showStockValue = showStockValue;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(26, Unit.Point);
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
                    row.RelativeItem().Column(brandCol =>
                    {
                        string compName = !string.IsNullOrWhiteSpace(_header?.CompanyName) ? _header.CompanyName : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");
                        brandCol.Item().Text(compName)
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        brandCol.Item().PaddingTop(2).Text("STOCK BALANCE & INVENTORY VALUATION")
                            .FontSize(11)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(260).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Text(string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _header.FromDate, _header.ToDate))
                            .FontSize(8.5f)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken3);

                        metaCol.Item().PaddingTop(2).Text("Category: " + (_header.CategoryName ?? "All Categories"))
                            .FontSize(8f)
                            .FontColor(Colors.Grey.Darken2);

                        metaCol.Item().PaddingTop(2).Text(string.Format("Generated: {0:dd MMM yyyy, HH:mm}", _header.GeneratedAt))
                            .FontSize(7.5f)
                            .FontColor(Colors.Grey.Darken1);
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(0.75f).LineColor(Colors.Grey.Lighten2);

                // Financial Summary KPI Strip
                col.Item().PaddingTop(6).PaddingBottom(6).Row(kpiRow =>
                {
                    decimal totalVal = _items.Sum(x => x.TotalValue);
                    decimal totalQty = _items.Sum(x => x.ClosingQty);
                    decimal totalIn = _items.Sum(x => x.QtyIn);
                    decimal totalOut = _items.Sum(x => x.QtyOut);

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total SKUs: ").FontSize(8.5f).Bold().FontColor(Colors.Grey.Darken2);
                        x.Span(_items.Count.ToString()).FontSize(9.5f).Bold().FontColor(Colors.Grey.Darken4);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total Inward: ").FontSize(8.5f).Bold().FontColor(Colors.Grey.Darken2);
                        x.Span(totalIn.ToString("#,##0")).FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken2);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total Outward: ").FontSize(8.5f).Bold().FontColor(Colors.Grey.Darken2);
                        x.Span(totalOut.ToString("#,##0")).FontSize(9.5f).Bold().FontColor(Colors.Orange.Darken3);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Closing Units: ").FontSize(8.5f).Bold().FontColor(Colors.Grey.Darken2);
                        x.Span(totalQty.ToString("#,##0")).FontSize(9.5f).Bold().FontColor(Colors.Grey.Darken4);
                    });

                    if (_showStockValue)
                    {
                        kpiRow.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Inventory Valuation: ").FontSize(8.5f).Bold().FontColor(Colors.Grey.Darken2);
                            x.Span(totalVal.ToString("#,##0.00")).FontSize(10.5f).Bold().FontColor(Colors.Green.Darken3);
                        });
                    }
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(4).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(28);  // Index #
                    columns.RelativeColumn(3.5f);// Item Description
                    columns.ConstantColumn(46);  // Unit
                    columns.ConstantColumn(64);  // Opening Qty
                    columns.ConstantColumn(64);  // Qty In
                    columns.ConstantColumn(64);  // Qty Out
                    if (_showStockValue)
                    {
                        columns.ConstantColumn(70);  // Balance Qty
                        columns.ConstantColumn(64);  // Rate
                        columns.ConstantColumn(80);  // Stock Value
                    }
                    else
                    {
                        columns.ConstantColumn(90);  // Balance Qty (wider)
                    }
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).AlignCenter().Text("#");
                    header.Cell().Element(HeaderCell).Text("Item / Product Name");
                    header.Cell().Element(HeaderCell).AlignCenter().Text("Unit");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Opening");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Inward (+)");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Outward (-)");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Closing Qty");
                    if (_showStockValue)
                    {
                        header.Cell().Element(HeaderCell).AlignRight().Text("Rate");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Total Value");
                    }
                });

                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    var isEven = (i % 2 == 0);
                    var bg = isEven ? Colors.White : Colors.Grey.Lighten5;

                    table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text((i + 1).ToString()).FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                    table.Cell().Element(c => BodyCell(c, bg)).Text(item.ItemName ?? string.Empty).SemiBold();
                    table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text(item.Unit ?? "Pcs");

                    // Opening
                    table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(item.OpeningQty.ToString("#,##0.##"));

                    // Inward
                    if (item.QtyIn > 0)
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(item.QtyIn.ToString("#,##0.##")).FontColor(Colors.Blue.Darken2);
                    else
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("-").FontColor(Colors.Grey.Lighten1);

                    // Outward
                    if (item.QtyOut > 0)
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(item.QtyOut.ToString("#,##0.##")).FontColor(Colors.Orange.Darken3);
                    else
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("-").FontColor(Colors.Grey.Lighten1);

                    // Closing Qty
                    table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(item.ClosingQty.ToString("#,##0.##")).SemiBold();

                    if (_showStockValue)
                    {
                        // Rate
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(item.Rate.ToString("#,##0.00"));
                        // Total Value
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(item.TotalValue.ToString("#,##0.00")).SemiBold();
                    }
                }

                // Table Summary Totals Row (Accounting Standard)
                decimal sumOpening = _items.Sum(x => x.OpeningQty);
                decimal sumIn = _items.Sum(x => x.QtyIn);
                decimal sumOut = _items.Sum(x => x.QtyOut);
                decimal sumBal = _items.Sum(x => x.ClosingQty);
                decimal sumVal = _items.Sum(x => x.TotalValue);

                table.Cell().ColumnSpan(3).Element(TotalCell).Text("TOTAL SUMMARY").Bold();
                table.Cell().Element(TotalCell).AlignRight().Text(sumOpening.ToString("#,##0.##")).Bold();
                table.Cell().Element(TotalCell).AlignRight().Text(sumIn.ToString("#,##0.##")).Bold();
                table.Cell().Element(TotalCell).AlignRight().Text(sumOut.ToString("#,##0.##")).Bold();
                table.Cell().Element(TotalCell).AlignRight().Text(sumBal.ToString("#,##0.##")).Bold();
                if (_showStockValue)
                {
                    table.Cell().Element(TotalCell).AlignRight().Text("-").FontColor(Colors.Grey.Darken1);
                    table.Cell().Element(TotalCell).AlignRight().Text(sumVal.ToString("#,##0.00")).Bold();
                }
            });
        }

        private static IContainer HeaderCell(IContainer cell)
        {
            return cell.Background(Colors.Grey.Lighten4)
                .BorderTop(1f)
                .BorderColor(Colors.Grey.Lighten2)
                .BorderBottom(1.5f)
                .BorderColor(Colors.Grey.Medium)
                .PaddingVertical(5)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.Bold().FontColor(Colors.Grey.Darken3).FontSize(7.5f));
        }

        private static IContainer BodyCell(IContainer cell, string bg)
        {
            return cell.Background(bg)
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten3)
                .PaddingVertical(4)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(7.5f));
        }

        private static IContainer TotalCell(IContainer cell)
        {
            return cell.Background(Colors.Grey.Lighten5)
                .BorderTop(1f)
                .BorderColor(Colors.Grey.Medium)
                .BorderBottom(2f)
                .BorderColor(Colors.Grey.Medium)
                .PaddingVertical(6)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.Bold().FontSize(8f).FontColor(Colors.Grey.Darken3));
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("Software powered by Bizgrip Solutions (Contact: 03228258734)")
                        .FontSize(7.5f)
                        .FontColor(Colors.Grey.Darken1);

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        x.CurrentPageNumber().FontSize(7.5f).Bold().FontColor(Colors.Grey.Darken3);
                        x.Span(" of ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        x.TotalPages().FontSize(7.5f).Bold().FontColor(Colors.Grey.Darken3);
                    });
                });
            });
        }

        /// <summary>
        /// Asynchronously renders the QuestPDF document to a temporary file on disk.
        /// </summary>
        public static async Task<string> GeneratePdfToTempFileAsync(StockBalanceHeader header, List<StockBalanceReportItem> items, bool showStockValue = false)
        {
            return await Task.Run(() =>
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "RetailSuiteReports");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                string tempPath = Path.Combine(tempDir, string.Format("StockBalance_{0:yyyyMMdd_HHmmss}_{1}.pdf", DateTime.Now, Guid.NewGuid().ToString("N").Substring(0, 6)));
                var document = new StockBalanceDocument(header, items, showStockValue);
                document.GeneratePdf(tempPath);
                return tempPath;
            });
        }
    }
}
