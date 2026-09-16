using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an individual revenue, cost of goods sold, or operating expense line in the Income Statement.
    /// </summary>
    public class IncomeSummaryLineItem
    {
        public string Category { get; set; } // "Sales", "Cost of Goods Sold", "Expenses"
        public string Title { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Amount { get; set; }

        public string FormattedAmount => Amount >= 0
            ? "Rs. " + Amount.ToString("#,##0.00")
            : "Rs. (" + Math.Abs(Amount).ToString("#,##0.00") + ")";
    }

    /// <summary>
    /// Holds summary metrics, profitability KPIs, and metadata for the Income Summary / Profit & Loss statement.
    /// </summary>
    public class IncomeSummaryHeader
    {
        public string CompanyName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalSales { get; set; }
        public decimal TotalCogs { get; set; }
        public decimal GrossProfit => TotalSales - TotalCogs;
        public decimal GrossMarginPct => TotalSales > 0 ? (GrossProfit / TotalSales) * 100m : 0m;

        public decimal TotalExpenses { get; set; }
        public decimal NetIncome => GrossProfit - TotalExpenses;
        public decimal NetMarginPct => TotalSales > 0 ? (NetIncome / TotalSales) * 100m : 0m;

        public bool IsProfitable => NetIncome >= 0;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header metadata and categorized lines for the Income Summary report.
    /// </summary>
    public class IncomeSummaryDataResult
    {
        public IncomeSummaryHeader Header { get; set; }
        public List<IncomeSummaryLineItem> SalesItems { get; set; }
        public List<IncomeSummaryLineItem> CogsItems { get; set; }
        public List<IncomeSummaryLineItem> ExpenseItems { get; set; }
        public List<IncomeSummaryLineItem> AllItems { get; set; }

        public IncomeSummaryDataResult(
            IncomeSummaryHeader header,
            List<IncomeSummaryLineItem> salesItems,
            List<IncomeSummaryLineItem> cogsItems,
            List<IncomeSummaryLineItem> expenseItems)
        {
            Header = header;
            SalesItems = salesItems ?? new List<IncomeSummaryLineItem>();
            CogsItems = cogsItems ?? new List<IncomeSummaryLineItem>();
            ExpenseItems = expenseItems ?? new List<IncomeSummaryLineItem>();

            AllItems = new List<IncomeSummaryLineItem>();
            AllItems.AddRange(SalesItems);
            AllItems.AddRange(CogsItems);
            AllItems.AddRange(ExpenseItems);
        }
    }

    /// <summary>
    /// Service to transform raw IncomeSummary DataTables into multi-step financial income statements.
    /// </summary>
    public static class IncomeSummaryDataService
    {
        public static IncomeSummaryDataResult ConvertDataTable(DataTable dt, DateTime fromDate, DateTime toDate)
        {
            var sales = new List<IncomeSummaryLineItem>();
            var cogs = new List<IncomeSummaryLineItem>();
            var expenses = new List<IncomeSummaryLineItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string vtype = row.Table.Columns.Contains("vtype") && row["vtype"] != DBNull.Value
                        ? row["vtype"].ToString().Trim()
                        : string.Empty;

                    string title = row.Table.Columns.Contains("title") && row["title"] != DBNull.Value
                        ? row["title"].ToString().Trim()
                        : string.Empty;

                    decimal dr = 0m;
                    if (row.Table.Columns.Contains("dr") && row["dr"] != DBNull.Value)
                    {
                        decimal.TryParse(row["dr"].ToString(), out dr);
                    }

                    decimal cr = 0m;
                    if (row.Table.Columns.Contains("cr") && row["cr"] != DBNull.Value)
                    {
                        decimal.TryParse(row["cr"].ToString(), out cr);
                    }

                    decimal bal = 0m;
                    if (row.Table.Columns.Contains("bal") && row["bal"] != DBNull.Value)
                    {
                        decimal.TryParse(row["bal"].ToString(), out bal);
                    }

                    if (string.Equals(vtype, "Sales", StringComparison.OrdinalIgnoreCase))
                    {
                        // In double-entry, Sales balance is Credit (Cr - Dr). If positive Credit, revenue is positive.
                        decimal netSales = cr - dr;
                        if (netSales == 0m && bal != 0m) netSales = Math.Abs(bal);

                        sales.Add(new IncomeSummaryLineItem
                        {
                            Category = "Sales",
                            Title = title,
                            Debit = dr,
                            Credit = cr,
                            Amount = netSales
                        });
                    }
                    else if (string.Equals(vtype, "Cost of Goods Sold", StringComparison.OrdinalIgnoreCase))
                    {
                        // COGS components: Opening Stock (Dr), Purchases (Dr - Cr), Closing Stock (-Dr)
                        decimal amount = dr - cr;
                        if (amount == 0m && bal != 0m) amount = bal;

                        cogs.Add(new IncomeSummaryLineItem
                        {
                            Category = "Cost of Goods Sold",
                            Title = title,
                            Debit = dr,
                            Credit = cr,
                            Amount = amount
                        });
                    }
                    else // Expenses
                    {
                        // Operating Expenses: Debit balance (Dr - Cr)
                        decimal netExp = dr - cr;
                        if (netExp == 0m && bal != 0m) netExp = Math.Abs(bal);

                        expenses.Add(new IncomeSummaryLineItem
                        {
                            Category = "Expenses",
                            Title = title,
                            Debit = dr,
                            Credit = cr,
                            Amount = netExp
                        });
                    }
                }
            }

            decimal totalSales = sales.Sum(x => x.Amount);
            decimal totalCogs = cogs.Sum(x => x.Amount);
            decimal totalExpenses = expenses.Sum(x => x.Amount);

            var header = new IncomeSummaryHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                FromDate = fromDate,
                ToDate = toDate,
                TotalSales = totalSales,
                TotalCogs = totalCogs,
                TotalExpenses = totalExpenses,
                GeneratedAt = DateTime.Now
            };

            return new IncomeSummaryDataResult(header, sales, cogs, expenses);
        }

        public static IncomeSummaryDataResult GetMockData(DateTime fromDate, DateTime toDate)
        {
            var sales = new List<IncomeSummaryLineItem>
            {
                new IncomeSummaryLineItem { Category = "Sales", Title = "Counter Retail Sales", Debit = 45000.00m, Credit = 3120000.00m, Amount = 3075000.00m },
                new IncomeSummaryLineItem { Category = "Sales", Title = "Corporate & Wholesale Sales", Debit = 12000.00m, Credit = 1485000.00m, Amount = 1473000.00m },
                new IncomeSummaryLineItem { Category = "Sales", Title = "Online Delivery Orders", Debit = 3500.00m, Credit = 345000.00m, Amount = 341500.00m }
            };

            var cogs = new List<IncomeSummaryLineItem>
            {
                new IncomeSummaryLineItem { Category = "Cost of Goods Sold", Title = "Opening Inventory Balance", Debit = 1250000.00m, Credit = 0m, Amount = 1250000.00m },
                new IncomeSummaryLineItem { Category = "Cost of Goods Sold", Title = "Purchases (Net of Returns)", Debit = 3420000.00m, Credit = 85000.00m, Amount = 3335000.00m },
                new IncomeSummaryLineItem { Category = "Cost of Goods Sold", Title = "Less: Closing Inventory Balance", Debit = -1480000.00m, Credit = 0m, Amount = -1480000.00m }
            };

            var expenses = new List<IncomeSummaryLineItem>
            {
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Salaries & Staff Allowances", Debit = 340000.00m, Credit = 0m, Amount = 340000.00m },
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Building & Store Rent", Debit = 120000.00m, Credit = 0m, Amount = 120000.00m },
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Electricity & Utility Bills", Debit = 85400.00m, Credit = 0m, Amount = 85400.00m },
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Freight & Delivery Charges", Debit = 42500.00m, Credit = 0m, Amount = 42500.00m },
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Marketing, Packaging & Promotion", Debit = 28600.00m, Credit = 0m, Amount = 28600.00m },
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Store Maintenance & Repair", Debit = 14300.00m, Credit = 0m, Amount = 14300.00m },
                new IncomeSummaryLineItem { Category = "Expenses", Title = "Office Supplies & Software", Debit = 9800.00m, Credit = 0m, Amount = 9800.00m }
            };

            decimal totalSales = sales.Sum(x => x.Amount);
            decimal totalCogs = cogs.Sum(x => x.Amount);
            decimal totalExpenses = expenses.Sum(x => x.Amount);

            var header = new IncomeSummaryHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                FromDate = fromDate,
                ToDate = toDate,
                TotalSales = totalSales,
                TotalCogs = totalCogs,
                TotalExpenses = totalExpenses,
                GeneratedAt = DateTime.Now
            };

            return new IncomeSummaryDataResult(header, sales, cogs, expenses);
        }
    }
}
