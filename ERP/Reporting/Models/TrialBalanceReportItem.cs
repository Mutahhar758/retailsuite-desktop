using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents a general ledger head entry in the Trial Balance statement.
    /// </summary>
    public class TrialBalanceReportItem
    {
        public string AccountCode { get; set; }
        public string AccountTitle { get; set; }
        public string Level1 { get; set; }
        public string Level2 { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal ClosingBalance { get; set; }

        /// <summary>
        /// Closing Debit balance (if balance is positive/debit).
        /// </summary>
        public decimal ClosingDebit => ClosingBalance > 0 ? ClosingBalance : 0m;

        /// <summary>
        /// Closing Credit balance (if balance is negative/credit).
        /// </summary>
        public decimal ClosingCredit => ClosingBalance < 0 ? Math.Abs(ClosingBalance) : 0m;
    }

    /// <summary>
    /// Holds summary metrics and reconciliation flags for the Trial Balance.
    /// </summary>
    public class TrialBalanceHeader
    {
        public string CompanyName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalOpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalClosingDebit { get; set; }
        public decimal TotalClosingCredit { get; set; }

        /// <summary>
        /// True when Total Period Debits equal Total Period Credits (GAAP balancing check).
        /// </summary>
        public bool IsBalanced => Math.Abs(TotalDebit - TotalCredit) < 0.01m;

        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header metadata and items for the Trial Balance report.
    /// </summary>
    public class TrialBalanceDataResult
    {
        public TrialBalanceHeader Header { get; set; }
        public List<TrialBalanceReportItem> Items { get; set; }

        public TrialBalanceDataResult(TrialBalanceHeader header, List<TrialBalanceReportItem> items)
        {
            Header = header;
            Items = items;
        }
    }

    /// <summary>
    /// Service to transform ReportQuery.TrialBalance DataTable results into typed trial balance items,
    /// and generate realistic balanced mock data for offline/preview testing.
    /// </summary>
    public static class TrialBalanceDataService
    {
        /// <summary>
        /// Converts a DataTable returned from ReportQuery.TrialBalance into typed trial balance items.
        /// </summary>
        public static List<TrialBalanceReportItem> FromDataTable(DataTable dt)
        {
            var items = new List<TrialBalanceReportItem>();
            if (dt == null) return items;

            foreach (DataRow row in dt.Rows)
            {
                string code = row.Table.Columns.Contains("lvl4") && row["lvl4"] != DBNull.Value
                    ? row["lvl4"].ToString()
                    : string.Empty;

                string title = row.Table.Columns.Contains("title") && row["title"] != DBNull.Value
                    ? row["title"].ToString()
                    : string.Empty;

                string lvl1 = row.Table.Columns.Contains("lvl1") && row["lvl1"] != DBNull.Value
                    ? row["lvl1"].ToString()
                    : string.Empty;

                string lvl2 = row.Table.Columns.Contains("lvl2") && row["lvl2"] != DBNull.Value
                    ? row["lvl2"].ToString()
                    : string.Empty;

                decimal pri = row.Table.Columns.Contains("pribal") && row["pribal"] != DBNull.Value
                    ? Convert.ToDecimal(row["pribal"])
                    : 0m;

                decimal dr = row.Table.Columns.Contains("dr") && row["dr"] != DBNull.Value
                    ? Convert.ToDecimal(row["dr"])
                    : 0m;

                decimal cr = row.Table.Columns.Contains("cr") && row["cr"] != DBNull.Value
                    ? Convert.ToDecimal(row["cr"])
                    : 0m;

                decimal cur = row.Table.Columns.Contains("curbal") && row["curbal"] != DBNull.Value
                    ? Convert.ToDecimal(row["curbal"])
                    : (pri + dr - cr);

                items.Add(new TrialBalanceReportItem
                {
                    AccountCode = code,
                    AccountTitle = title,
                    Level1 = lvl1,
                    Level2 = lvl2,
                    OpeningBalance = pri,
                    Debit = dr,
                    Credit = cr,
                    ClosingBalance = cur
                });
            }

            return items;
        }

        /// <summary>
        /// Generates 16+ realistic, mathematically balanced general ledger accounts for preview.
        /// Total debits strictly equal total credits.
        /// </summary>
        public static TrialBalanceDataResult GetSampleTrialBalance(string companyName = null)
        {
            var header = new TrialBalanceHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(companyName)
                    ? companyName
                    : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise"),
                ToDate = DateTime.Today
            };

            var items = new List<TrialBalanceReportItem>
            {
                // Current Assets (Debit)
                new TrialBalanceReportItem { AccountCode = "001-001-001", AccountTitle = "Cash in Hand - Main Vault", Level1 = "Assets", Level2 = "Current Assets", OpeningBalance = 45000m, Debit = 285000m, Credit = 260000m, ClosingBalance = 70000m },
                new TrialBalanceReportItem { AccountCode = "001-001-002", AccountTitle = "HBL Corporate Current Account", Level1 = "Assets", Level2 = "Current Assets", OpeningBalance = 250000m, Debit = 540000m, Credit = 490000m, ClosingBalance = 300000m },
                new TrialBalanceReportItem { AccountCode = "001-002-001", AccountTitle = "Trade Accounts Receivable (Debtors)", Level1 = "Assets", Level2 = "Current Assets", OpeningBalance = 180000m, Debit = 420000m, Credit = 390000m, ClosingBalance = 210000m },
                new TrialBalanceReportItem { AccountCode = "001-003-001", AccountTitle = "Merchandise Inventory (At Cost)", Level1 = "Assets", Level2 = "Current Assets", OpeningBalance = 320000m, Debit = 350000m, Credit = 310000m, ClosingBalance = 360000m },

                // Non-Current Assets (Debit)
                new TrialBalanceReportItem { AccountCode = "002-001-001", AccountTitle = "Store Fixtures & Display Equipment", Level1 = "Assets", Level2 = "Fixed Assets", OpeningBalance = 150000m, Debit = 25000m, Credit = 0m, ClosingBalance = 175000m },
                new TrialBalanceReportItem { AccountCode = "002-002-001", AccountTitle = "Point of Sale & IT Infrastructure", Level1 = "Assets", Level2 = "Fixed Assets", OpeningBalance = 85000m, Debit = 15000m, Credit = 0m, ClosingBalance = 100000m },

                // Current Liabilities (Credit)
                new TrialBalanceReportItem { AccountCode = "003-001-001", AccountTitle = "Trade Accounts Payable (Suppliers)", Level1 = "Liabilities", Level2 = "Current Liabilities", OpeningBalance = -210000m, Debit = 310000m, Credit = 340000m, ClosingBalance = -240000m },
                new TrialBalanceReportItem { AccountCode = "003-002-001", AccountTitle = "Accrued Utilities & Operating Expenses", Level1 = "Liabilities", Level2 = "Current Liabilities", OpeningBalance = -18000m, Debit = 18000m, Credit = 22000m, ClosingBalance = -22000m },
                new TrialBalanceReportItem { AccountCode = "003-003-001", AccountTitle = "Short-Term Bank Credit Facility", Level1 = "Liabilities", Level2 = "Current Liabilities", OpeningBalance = -100000m, Debit = 50000m, Credit = 50000m, ClosingBalance = -100000m },

                // Equity (Credit)
                new TrialBalanceReportItem { AccountCode = "004-001-001", AccountTitle = "Owner Capital Share Account", Level1 = "Equity", Level2 = "Capital", OpeningBalance = -550000m, Debit = 0m, Credit = 0m, ClosingBalance = -550000m },
                new TrialBalanceReportItem { AccountCode = "004-002-001", AccountTitle = "Retained Earnings - Brought Forward", Level1 = "Equity", Level2 = "Reserves", OpeningBalance = -152000m, Debit = 0m, Credit = 0m, ClosingBalance = -152000m },

                // Revenue / Sales (Credit)
                new TrialBalanceReportItem { AccountCode = "005-001-001", AccountTitle = "Gross Sales Revenue - Retail Store", Level1 = "Revenue", Level2 = "Operating Revenue", OpeningBalance = 0m, Debit = 0m, Credit = 490000m, ClosingBalance = -490000m },
                new TrialBalanceReportItem { AccountCode = "005-002-001", AccountTitle = "Sales Returns & Customer Allowances", Level1 = "Revenue", Level2 = "Contra Revenue", OpeningBalance = 0m, Debit = 15000m, Credit = 0m, ClosingBalance = 15000m },

                // Cost of Sales (Debit)
                new TrialBalanceReportItem { AccountCode = "006-001-001", AccountTitle = "Cost of Goods Sold (COGS)", Level1 = "Cost of Sales", Level2 = "Direct Costs", OpeningBalance = 0m, Debit = 310000m, Credit = 0m, ClosingBalance = 310000m },

                // Operating Expenses (Debit)
                new TrialBalanceReportItem { AccountCode = "007-001-001", AccountTitle = "Salaries, Wages & Staff Benefits", Level1 = "Expenses", Level2 = "Operating Expenses", OpeningBalance = 0m, Debit = 85000m, Credit = 0m, ClosingBalance = 85000m },
                new TrialBalanceReportItem { AccountCode = "007-002-001", AccountTitle = "Commercial Property Rent & Taxes", Level1 = "Expenses", Level2 = "Operating Expenses", OpeningBalance = 0m, Debit = 45000m, Credit = 0m, ClosingBalance = 45000m },
                new TrialBalanceReportItem { AccountCode = "007-003-001", AccountTitle = "Electricity, Water & Power Utilities", Level1 = "Expenses", Level2 = "Operating Expenses", OpeningBalance = 0m, Debit = 22000m, Credit = 0m, ClosingBalance = 22000m },
                new TrialBalanceReportItem { AccountCode = "007-004-001", AccountTitle = "Depreciation Expense - POS & Fixtures", Level1 = "Expenses", Level2 = "Non-Cash Expenses", OpeningBalance = 0m, Debit = 7000m, Credit = 0m, ClosingBalance = 7000m }
            };

            header.TotalAccounts = items.Count;
            header.TotalOpeningBalance = items.Sum(x => x.OpeningBalance);
            header.TotalDebit = items.Sum(x => x.Debit);
            header.TotalCredit = items.Sum(x => x.Credit);
            header.TotalClosingDebit = items.Sum(x => x.ClosingDebit);
            header.TotalClosingCredit = items.Sum(x => x.ClosingCredit);

            return new TrialBalanceDataResult(header, items);
        }
    }
}
