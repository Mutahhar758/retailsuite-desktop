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
    /// Service to transform ReportQuery.TrialBalance DataTable results into typed trial balance items.
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
    }
}
