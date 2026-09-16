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
    /// Code-first QuestPDF document implementing a corporate, multi-step Income Statement (Profit & Loss).
    /// Follows international GAAP / IFRS reporting standards for financial statements.
    /// </summary>
    public class IncomeSummaryDocument : IDocument
    {
        private readonly IncomeSummaryHeader _header;
        private readonly List<IncomeSummaryLineItem> _salesItems;
        private readonly List<IncomeSummaryLineItem> _cogsItems;
        private readonly List<IncomeSummaryLineItem> _expenseItems;

        public IncomeSummaryDocument(
            IncomeSummaryHeader header,
            List<IncomeSummaryLineItem> salesItems,
            List<IncomeSummaryLineItem> cogsItems,
            List<IncomeSummaryLineItem> expenseItems)
        {
            _header = header ?? new IncomeSummaryHeader();
            _salesItems = salesItems ?? new List<IncomeSummaryLineItem>();
            _cogsItems = cogsItems ?? new List<IncomeSummaryLineItem>();
            _expenseItems = expenseItems ?? new List<IncomeSummaryLineItem>();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32, Unit.Point);
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
                        string compName = !string.IsNullOrWhiteSpace(_header?.CompanyName)
                            ? _header.CompanyName
                            : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise");

                        brandCol.Item().Text(compName)
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        brandCol.Item().PaddingTop(2).Text("INCOME STATEMENT (PROFIT & LOSS)")
                            .FontSize(11)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken1);

                        brandCol.Item().PaddingTop(2).Text(string.Format("For the Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _header.FromDate, _header.ToDate))
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(240).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Text(string.Format("Generated: {0:dd MMM yyyy, HH:mm}", _header.GeneratedAt))
                            .FontSize(7.5f)
                            .FontColor(Colors.Grey.Darken1);

                        // Profitability Pill Badge
                        if (_header.IsProfitable)
                        {
                            metaCol.Item().PaddingTop(4).Text(string.Format("✓ NET PROFIT: Rs. {0:#,##0.00} ({1:F1}%)", _header.NetIncome, _header.NetMarginPct))
                                .FontSize(8.5f)
                                .Bold()
                                .FontColor(Colors.Green.Darken3);
                        }
                        else
                        {
                            metaCol.Item().PaddingTop(4).Text(string.Format("⚠ NET DEFICIT: Rs. ({0:#,##0.00})", Math.Abs(_header.NetIncome)))
                                .FontSize(8.5f)
                                .Bold()
                                .FontColor(Colors.Red.Darken2);
                        }
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(0.75f).LineColor(Colors.Grey.Lighten2);

                // KPI Metric Summary Strip
                col.Item().PaddingTop(6).PaddingBottom(6).Row(kpiRow =>
                {
                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total Revenue: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span("Rs. " + _header.TotalSales.ToString("#,##0.00")).FontSize(9f).Bold().FontColor(Colors.Grey.Darken4);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Gross Profit: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span(string.Format("Rs. {0:#,##0.00} ({1:F1}%)", _header.GrossProfit, _header.GrossMarginPct))
                            .FontSize(9f).Bold().FontColor(Colors.Blue.Darken3);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total Expenses: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span("Rs. " + _header.TotalExpenses.ToString("#,##0.00")).FontSize(9f).Bold().FontColor(Colors.Red.Darken2);
                    });

                    kpiRow.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Net Margin: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span(string.Format("{0:F1}%", _header.NetMarginPct))
                            .FontSize(9f).Bold().FontColor(_header.IsProfitable ? Colors.Green.Darken3 : Colors.Red.Darken2);
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(4).Column(col =>
            {
                // ==========================================
                // SECTION 1: REVENUE / SALES
                // ==========================================
                col.Item().Element(c => SectionTitle(c, "1. REVENUE / OPERATING TURNOVER", Colors.Blue.Darken3));
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // #
                        columns.RelativeColumn(4.5f); // Title
                        columns.ConstantColumn(120);  // Amount
                    });

                    for (int i = 0; i < _salesItems.Count; i++)
                    {
                        var item = _salesItems[i];
                        var bg = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                        table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text((i + 1).ToString()).FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        table.Cell().Element(c => BodyCell(c, bg)).Text(item.Title ?? string.Empty).SemiBold();
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("Rs. " + item.Amount.ToString("#,##0.00")).FontColor(Colors.Grey.Darken4);
                    }

                    // Section Subtotal
                    table.Cell().ColumnSpan(2).Element(SectionSubtotalCell).Text("TOTAL REVENUE (A)").Bold().FontColor(Colors.Grey.Darken4);
                    table.Cell().Element(SectionSubtotalCell).AlignRight().Text("Rs. " + _header.TotalSales.ToString("#,##0.00")).Bold().FontColor(Colors.Grey.Darken4);
                });

                col.Item().PaddingTop(10);

                // ==========================================
                // SECTION 2: COST OF GOODS SOLD (COGS)
                // ==========================================
                col.Item().Element(c => SectionTitle(c, "2. COST OF GOODS SOLD (COGS)", Colors.Grey.Darken3));
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // #
                        columns.RelativeColumn(4.5f); // Title
                        columns.ConstantColumn(120);  // Amount
                    });

                    for (int i = 0; i < _cogsItems.Count; i++)
                    {
                        var item = _cogsItems[i];
                        var bg = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                        table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text((i + 1).ToString()).FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        table.Cell().Element(c => BodyCell(c, bg)).Text(item.Title ?? string.Empty).SemiBold();

                        if (item.Amount < 0)
                        {
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("Rs. (" + Math.Abs(item.Amount).ToString("#,##0.00") + ")").FontColor(Colors.Grey.Darken3);
                        }
                        else
                        {
                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("Rs. " + item.Amount.ToString("#,##0.00")).FontColor(Colors.Grey.Darken4);
                        }
                    }

                    // Section Subtotal
                    table.Cell().ColumnSpan(2).Element(SectionSubtotalCell).Text("TOTAL COST OF GOODS SOLD (B)").Bold().FontColor(Colors.Grey.Darken4);
                    table.Cell().Element(SectionSubtotalCell).AlignRight().Text("Rs. " + _header.TotalCogs.ToString("#,##0.00")).Bold().FontColor(Colors.Grey.Darken4);
                });

                col.Item().PaddingTop(10);

                // ==========================================
                // GROSS PROFIT HIGHLIGHT ROW
                // ==========================================
                col.Item().Border(1).BorderColor(Colors.Blue.Lighten2).Background(Colors.Blue.Lighten5).PaddingVertical(6).PaddingHorizontal(8).Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span("GROSS PROFIT / (LOSS)  [A - B]: ").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                        x.Span(string.Format("(Gross Margin: {0:F1}%)", _header.GrossMarginPct)).FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(150).AlignRight().Text("Rs. " + _header.GrossProfit.ToString("#,##0.00"))
                        .FontSize(10f)
                        .Bold()
                        .FontColor(Colors.Blue.Darken3);
                });

                col.Item().PaddingTop(10);

                // ==========================================
                // SECTION 3: OPERATING EXPENSES
                // ==========================================
                col.Item().Element(c => SectionTitle(c, "3. OPERATING EXPENSES", Colors.Red.Darken2));
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // #
                        columns.RelativeColumn(4.5f); // Title
                        columns.ConstantColumn(120);  // Amount
                    });

                    for (int i = 0; i < _expenseItems.Count; i++)
                    {
                        var item = _expenseItems[i];
                        var bg = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                        table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text((i + 1).ToString()).FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        table.Cell().Element(c => BodyCell(c, bg)).Text(item.Title ?? string.Empty).SemiBold();
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("Rs. " + item.Amount.ToString("#,##0.00")).FontColor(Colors.Grey.Darken4);
                    }

                    // Section Subtotal
                    table.Cell().ColumnSpan(2).Element(SectionSubtotalCell).Text("TOTAL OPERATING EXPENSES (C)").Bold().FontColor(Colors.Grey.Darken4);
                    table.Cell().Element(SectionSubtotalCell).AlignRight().Text("Rs. " + _header.TotalExpenses.ToString("#,##0.00")).Bold().FontColor(Colors.Red.Darken2);
                });

                col.Item().PaddingTop(12);

                // ==========================================
                // NET INCOME (LOSS) FINAL GAAP SUMMARY
                // ==========================================
                col.Item().BorderTop(1.5f).BorderColor(Colors.Grey.Darken3).BorderBottom(2.5f).BorderColor(Colors.Grey.Darken3)
                    .Background(Colors.Grey.Lighten4).PaddingVertical(8).PaddingHorizontal(8).Row(row =>
                    {
                        row.RelativeItem().Column(titleCol =>
                        {
                            titleCol.Item().Text(x =>
                            {
                                x.Span("NET PROFIT / (LOSS) FOR THE PERIOD: ").FontSize(10.5f).Bold().FontColor(Colors.Grey.Darken4);
                                x.Span(string.Format("(Net Margin: {0:F1}%)", _header.NetMarginPct)).FontSize(8.5f).SemiBold().FontColor(Colors.Grey.Darken2);
                            });

                            titleCol.Item().PaddingTop(2).Text("Gross Profit [A - B] less Operating Expenses [C]").FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(180).AlignRight().Text(x =>
                        {
                            if (_header.IsProfitable)
                            {
                                x.Span("Rs. " + _header.NetIncome.ToString("#,##0.00")).FontSize(12f).Bold().FontColor(Colors.Green.Darken3);
                            }
                            else
                            {
                                x.Span("Rs. (" + Math.Abs(_header.NetIncome).ToString("#,##0.00") + ")").FontSize(12f).Bold().FontColor(Colors.Red.Darken2);
                            }
                        });
                    });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("Confidential • Executive Management Financial Reporting")
                        .FontSize(7.5f)
                        .FontColor(Colors.Grey.Darken1);

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        x.CurrentPageNumber().FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken3);
                        x.Span(" of ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        x.TotalPages().FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken3);
                    });
                });
            });
        }

        private static void SectionTitle(IContainer container, string title, string color)
        {
            container
                .Background(Colors.Grey.Lighten3)
                .PaddingVertical(4)
                .PaddingHorizontal(6)
                .Text(title)
                .FontSize(8.5f)
                .Bold()
                .FontColor(color);
        }

        private static IContainer BodyCell(IContainer container, string backgroundColor)
        {
            return container
                .Background(backgroundColor)
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten3)
                .PaddingVertical(3.5f)
                .PaddingHorizontal(4);
        }

        private static IContainer SectionSubtotalCell(IContainer container)
        {
            return container
                .BorderTop(1f)
                .BorderColor(Colors.Grey.Darken2)
                .BorderBottom(1f)
                .BorderColor(Colors.Grey.Darken2)
                .Background(Colors.Grey.Lighten4)
                .PaddingVertical(4.5f)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(8.5f));
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

                string filePath = Path.Combine(tempDir, string.Format("IncomeSummary_{0:yyyyMMdd_HHmmss}_{1}.pdf", DateTime.Now, Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }
    }
}
