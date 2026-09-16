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
    /// Code-first QuestPDF document implementing the corporate Balance Sheet (Statement of Financial Position).
    /// Follows international GAAP / IFRS accounting standards (Assets = Liabilities + Equity).
    /// </summary>
    public class BalanceSheetDocument : IDocument
    {
        private readonly BalanceSheetHeader _header;
        private readonly List<BalanceSheetReportItem> _assetItems;
        private readonly List<BalanceSheetReportItem> _liabilityItems;
        private readonly List<BalanceSheetReportItem> _equityItems;

        public BalanceSheetDocument(
            BalanceSheetHeader header,
            List<BalanceSheetReportItem> assetItems,
            List<BalanceSheetReportItem> liabilityItems,
            List<BalanceSheetReportItem> equityItems)
        {
            _header = header ?? new BalanceSheetHeader();
            _assetItems = assetItems ?? new List<BalanceSheetReportItem>();
            _liabilityItems = liabilityItems ?? new List<BalanceSheetReportItem>();
            _equityItems = equityItems ?? new List<BalanceSheetReportItem>();
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

                        brandCol.Item().PaddingTop(2).Text("BALANCE SHEET (STATEMENT OF FINANCIAL POSITION)")
                            .FontSize(11)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken1);

                        brandCol.Item().PaddingTop(2).Text(string.Format("As of Date: {0:dd-MMM-yyyy}", _header.AsOnDate))
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(260).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().Text(string.Format("Generated: {0:dd MMM yyyy, HH:mm}", _header.GeneratedAt))
                            .FontSize(7.5f)
                            .FontColor(Colors.Grey.Darken1);

                        // Audit Balancing Indicator Pill
                        if (_header.IsBalanced)
                        {
                            metaCol.Item().PaddingTop(4).Text("✓ BALANCED (Assets = Liab + Equity)")
                                .FontSize(8.5f)
                                .Bold()
                                .FontColor(Colors.Green.Darken3);
                        }
                        else
                        {
                            metaCol.Item().PaddingTop(4).Text(string.Format("⚠ OUT OF BALANCE: Diff Rs. {0:#,##0.00}", _header.Variance))
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
                        x.Span("Total Assets: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span("Rs. " + _header.TotalAssets.ToString("#,##0.00")).FontSize(9f).Bold().FontColor(Colors.Grey.Darken4);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total Liabilities: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span("Rs. " + _header.TotalLiabilities.ToString("#,##0.00")).FontSize(9f).Bold().FontColor(Colors.Red.Darken2);
                    });

                    kpiRow.RelativeItem().Text(x =>
                    {
                        x.Span("Total Equity: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span("Rs. " + _header.TotalEquity.ToString("#,##0.00")).FontSize(9f).Bold().FontColor(Colors.Blue.Darken3);
                    });

                    kpiRow.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Working Capital: ").FontSize(8f).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Span("Rs. " + _header.NetWorkingCapital.ToString("#,##0.00")).FontSize(9f).Bold().FontColor(Colors.Green.Darken3);
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(4).Column(col =>
            {
                // ==========================================
                // SECTION 1: ASSETS
                // ==========================================
                col.Item().Element(c => SectionTitle(c, "1. ASSETS", Colors.Blue.Darken3));
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // #
                        columns.RelativeColumn(4.5f); // Title
                        columns.ConstantColumn(120);  // Amount
                    });

                    for (int i = 0; i < _assetItems.Count; i++)
                    {
                        var item = _assetItems[i];
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

                    // Total Assets Row with double-underline
                    table.Cell().ColumnSpan(2).Element(GrandTotalCell).Text("TOTAL ASSETS (A)").Bold().FontColor(Colors.Blue.Darken3);
                    table.Cell().Element(GrandTotalCell).AlignRight().Text("Rs. " + _header.TotalAssets.ToString("#,##0.00")).Bold().FontColor(Colors.Blue.Darken3);
                });

                col.Item().PaddingTop(12);

                // ==========================================
                // SECTION 2: LIABILITIES
                // ==========================================
                col.Item().Element(c => SectionTitle(c, "2. LIABILITIES", Colors.Red.Darken2));
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // #
                        columns.RelativeColumn(4.5f); // Title
                        columns.ConstantColumn(120);  // Amount
                    });

                    for (int i = 0; i < _liabilityItems.Count; i++)
                    {
                        var item = _liabilityItems[i];
                        var bg = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                        table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text((i + 1).ToString()).FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        table.Cell().Element(c => BodyCell(c, bg)).Text(item.Title ?? string.Empty).SemiBold();
                        table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text("Rs. " + item.Amount.ToString("#,##0.00")).FontColor(Colors.Grey.Darken4);
                    }

                    // Total Liabilities Subtotal
                    table.Cell().ColumnSpan(2).Element(SectionSubtotalCell).Text("TOTAL LIABILITIES (B)").Bold().FontColor(Colors.Grey.Darken4);
                    table.Cell().Element(SectionSubtotalCell).AlignRight().Text("Rs. " + _header.TotalLiabilities.ToString("#,##0.00")).Bold().FontColor(Colors.Red.Darken2);
                });

                col.Item().PaddingTop(12);

                // ==========================================
                // SECTION 3: CAPITAL & OWNER'S EQUITY
                // ==========================================
                col.Item().Element(c => SectionTitle(c, "3. CAPITAL & OWNER'S EQUITY", Colors.Green.Darken3));
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // #
                        columns.RelativeColumn(4.5f); // Title
                        columns.ConstantColumn(120);  // Amount
                    });

                    for (int i = 0; i < _equityItems.Count; i++)
                    {
                        var item = _equityItems[i];
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

                    // Total Equity Subtotal
                    table.Cell().ColumnSpan(2).Element(SectionSubtotalCell).Text("TOTAL CAPITAL & EQUITY (C)").Bold().FontColor(Colors.Grey.Darken4);
                    table.Cell().Element(SectionSubtotalCell).AlignRight().Text("Rs. " + _header.TotalEquity.ToString("#,##0.00")).Bold().FontColor(Colors.Green.Darken3);
                });

                col.Item().PaddingTop(12);

                // ==========================================
                // GRAND RECONCILIATION: TOTAL LIABILITIES & EQUITY
                // ==========================================
                col.Item().BorderTop(1.5f).BorderColor(Colors.Grey.Darken3).BorderBottom(2.5f).BorderColor(Colors.Grey.Darken3)
                    .Background(Colors.Grey.Lighten4).PaddingVertical(8).PaddingHorizontal(8).Row(row =>
                    {
                        row.RelativeItem().Column(titleCol =>
                        {
                            titleCol.Item().Text("TOTAL LIABILITIES & EQUITY  [B + C]").FontSize(10.5f).Bold().FontColor(Colors.Grey.Darken4);
                            titleCol.Item().PaddingTop(2).Text("GAAP Reconciliation: Total Liabilities plus Owner's Equity").FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(180).AlignRight().Text("Rs. " + _header.TotalLiabilitiesAndEquity.ToString("#,##0.00"))
                            .FontSize(12f)
                            .Bold()
                            .FontColor(_header.IsBalanced ? Colors.Green.Darken3 : Colors.Red.Darken2);
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
                    row.RelativeItem().Text("Confidential • Corporate Financial Statement of Position")
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

        private static IContainer GrandTotalCell(IContainer container)
        {
            return container
                .BorderTop(1.5f)
                .BorderColor(Colors.Grey.Darken3)
                .BorderBottom(2.5f) // Double-underline GAAP standard
                .BorderColor(Colors.Grey.Darken3)
                .Background(Colors.Grey.Lighten4)
                .PaddingVertical(5)
                .PaddingHorizontal(4)
                .DefaultTextStyle(x => x.FontSize(9f));
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

                string filePath = Path.Combine(tempDir, string.Format("BalanceSheet_{0:yyyyMMdd_HHmmss}_{1}.pdf", DateTime.Now, Guid.NewGuid().ToString().Substring(0, 6)));
                this.GeneratePdf(filePath);
                return filePath;
            });
        }
    }
}
