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
    public class ProfitByItemDocument : IDocument
    {
        private readonly ProfitByItemHeader _header;
        private readonly List<ProfitByItemReportItem> _items;

        public ProfitByItemDocument(ProfitByItemHeader header, List<ProfitByItemReportItem> items)
        {
            _header = header ?? new ProfitByItemHeader();
            _items = items ?? new List<ProfitByItemReportItem>();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(28, Unit.Point);
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
                        brandCol.Item().Text(_header.CompanyName)
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        brandCol.Item().PaddingTop(2).Text("PROFIT BY ITEM REPORT")
                            .FontSize(11)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken1);

                        string periodText = string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _header.FromDate, _header.ToDate);
                        if (!string.IsNullOrWhiteSpace(_header.ItemFilter))
                            periodText += "  |  Item: " + _header.ItemFilter;
                        if (!string.IsNullOrWhiteSpace(_header.CategoryFilter))
                            periodText += "  |  Category: " + _header.CategoryFilter;

                        brandCol.Item().PaddingTop(2).Text(periodText)
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(300).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Text(string.Format("Generated: {0:dd MMM yyyy, HH:mm}", _header.GeneratedAt))
                            .FontSize(7.5f)
                            .FontColor(Colors.Grey.Darken1);

                        metaCol.Item().PaddingTop(4).Text(string.Format("TOTAL PROFIT: Rs. {0:#,##0.00} ({1:F1}%)", _header.GrossProfit, _header.GrossMarginPct))
                            .FontSize(10.5f)
                            .Bold()
                            .FontColor(_header.IsProfitable ? Colors.Green.Darken3 : Colors.Red.Darken2);
                    });
                });

                // Summary Metric Cards
                col.Item().PaddingTop(8).PaddingBottom(6).Row(kpiRow =>
                {
                    ComposeKpiCard(kpiRow.RelativeItem(), "TOTAL REVENUE", string.Format("Rs. {0:#,##0.00}", _header.TotalSales), Colors.Blue.Darken2);
                    kpiRow.ConstantItem(8);
                    ComposeKpiCard(kpiRow.RelativeItem(), "TOTAL COST", string.Format("Rs. {0:#,##0.00}", _header.TotalCost), Colors.Amber.Darken3);
                    kpiRow.ConstantItem(8);
                    ComposeKpiCard(kpiRow.RelativeItem(), "GROSS PROFIT", string.Format("Rs. {0:#,##0.00}", _header.GrossProfit), _header.IsProfitable ? Colors.Green.Darken3 : Colors.Red.Darken2);
                    kpiRow.ConstantItem(8);
                    ComposeKpiCard(kpiRow.RelativeItem(), "PROFIT MARGIN", string.Format("{0:F1}%", _header.GrossMarginPct), Colors.Indigo.Darken2);
                    kpiRow.ConstantItem(8);
                    ComposeKpiCard(kpiRow.RelativeItem(), "ITEMS SOLD", _header.TotalItems.ToString(), Colors.Grey.Darken3);
                });

                col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        }

        private void ComposeKpiCard(IContainer container, string title, string value, string accentColor)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Padding(6).Column(c =>
            {
                c.Item().Text(title).FontSize(6.5f).Bold().FontColor(Colors.Grey.Darken1);
                c.Item().PaddingTop(1).Text(value).FontSize(9.5f).Bold().FontColor(accentColor);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(30);   // Sr#
                    cols.ConstantColumn(70);   // Item Code
                    cols.RelativeColumn(3);    // Item Title
                    cols.RelativeColumn(1.5f); // Category
                    cols.ConstantColumn(50);   // Unit
                    cols.ConstantColumn(60);   // Qty Sold
                    cols.ConstantColumn(65);   // Avg Sale Rate
                    cols.ConstantColumn(85);   // Total Sales
                    cols.ConstantColumn(65);   // Avg Cost Rate
                    cols.ConstantColumn(85);   // FIFO Cost
                    cols.ConstantColumn(85);   // Gross Profit
                    cols.ConstantColumn(50);   // Margin %
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("#").Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Code").Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Item Title").Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Category").Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Unit").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Qty").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Sale Rate").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Sales (Rs.)").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Cost Rate").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Cost (Rs.)").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Profit (Rs.)").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Margin %").Bold();

                    IContainer HeaderCellStyle(IContainer c) =>
                        c.Background(Colors.Grey.Lighten4)
                         .BorderBottom(1.5f)
                         .BorderColor(Colors.Grey.Darken1)
                         .PaddingVertical(4)
                         .PaddingHorizontal(4);
                });

                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;

                    table.Cell().Element(c => CellStyle(c, bg)).Text((i + 1).ToString());
                    table.Cell().Element(c => CellStyle(c, bg)).Text(item.ItemId ?? string.Empty);
                    table.Cell().Element(c => CellStyle(c, bg)).Text(item.ItemTitle ?? string.Empty).SemiBold();
                    table.Cell().Element(c => CellStyle(c, bg)).Text(item.Category ?? "-");
                    table.Cell().Element(c => CellStyle(c, bg)).Text(item.Unit ?? "-");
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:#,##0.##}", item.TotalQty));
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:#,##0.00}", item.AvgSaleRate));
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:#,##0.00}", item.TotalSales));
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:#,##0.00}", item.AvgCostRate));
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:#,##0.00}", item.TotalCost));
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:#,##0.00}", item.GrossProfit))
                        .Bold()
                        .FontColor(item.GrossProfit >= 0 ? Colors.Green.Darken3 : Colors.Red.Darken2);
                    table.Cell().Element(c => CellStyle(c, bg)).AlignRight().Text(string.Format("{0:F1}%", item.GrossMarginPct));
                }

                table.Cell().ColumnSpan(5).Element(TotalCellStyle).Text("GRAND TOTALS").Bold();
                table.Cell().Element(TotalCellStyle).AlignRight().Text(string.Format("{0:#,##0.##}", _header.TotalQtySold)).Bold();
                table.Cell().Element(TotalCellStyle).Text(string.Empty);
                table.Cell().Element(TotalCellStyle).AlignRight().Text(string.Format("Rs. {0:#,##0.00}", _header.TotalSales)).Bold();
                table.Cell().Element(TotalCellStyle).Text(string.Empty);
                table.Cell().Element(TotalCellStyle).AlignRight().Text(string.Format("Rs. {0:#,##0.00}", _header.TotalCost)).Bold();
                table.Cell().Element(TotalCellStyle).AlignRight().Text(string.Format("Rs. {0:#,##0.00}", _header.GrossProfit)).Bold()
                    .FontColor(_header.IsProfitable ? Colors.Green.Darken3 : Colors.Red.Darken2);
                table.Cell().Element(TotalCellStyle).AlignRight().Text(string.Format("{0:F1}%", _header.GrossMarginPct)).Bold();

                IContainer CellStyle(IContainer c, string bg) =>
                    c.Background(bg)
                     .BorderBottom(0.5f)
                     .BorderColor(Colors.Grey.Lighten3)
                     .PaddingVertical(3.5f)
                     .PaddingHorizontal(4);

                IContainer TotalCellStyle(IContainer c) =>
                    c.Background(Colors.Grey.Lighten3)
                     .BorderTop(1.5f)
                     .BorderBottom(1.5f)
                     .BorderColor(Colors.Grey.Darken2)
                     .PaddingVertical(4.5f)
                     .PaddingHorizontal(4);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text(x =>
                {
                    x.Span("RetailSuite Financial Reporting  |  Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });

                row.AutoItem().Text(string.Format("Confidential — Printed: {0:yyyy-MM-dd HH:mm}", DateTime.Now))
                    .FontSize(7.5f)
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        public async Task<string> GeneratePdfToTempFileAsync()
        {
            return await Task.Run(() =>
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "RetailSuite", "Reports");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                string filePath = Path.Combine(tempDir, string.Format("ProfitByItem_{0:yyyyMMdd_HHmmss}_{1}.pdf", DateTime.Now, Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }
    }
}
