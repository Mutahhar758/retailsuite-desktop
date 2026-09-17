using System;
using System.Collections.Generic;
using System.Data;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an individual transaction row in the Account Statement With Due report,
    /// tracking due terms and calculating due dates alongside ledger balances.
    /// </summary>
    public class AccountStatementWithDueReportItem
    {
        public DateTime Date { get; set; }
        public string VoucherNo { get; set; }
        public int Sequence { get; set; }
        public string Particular { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public int? DueDays { get; set; }

        /// <summary>
        /// Due Date dynamically computed from transaction date plus due days.
        /// </summary>
        public DateTime? DueDate
        {
            get
            {
                if (DueDays.HasValue && DueDays.Value > 0)
                {
                    return Date.AddDays(DueDays.Value);
                }
                return null;
            }
        }
    }

    /// <summary>
    /// Holds summary metrics and filter metadata for an Account Statement With Due report.
    /// </summary>
    public class AccountStatementWithDueHeader
    {
        public string CompanyName { get; set; }
        public string AccountTitle { get; set; }
        public string AccountCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public string DateBasis { get; set; } = "Voucher Date";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header and item records for Account Statement With Due.
    /// </summary>
    public class AccountStatementWithDueDataResult
    {
        public AccountStatementWithDueHeader Header { get; set; }
        public List<AccountStatementWithDueReportItem> Items { get; set; }

        public AccountStatementWithDueDataResult(AccountStatementWithDueHeader header, List<AccountStatementWithDueReportItem> items)
        {
            Header = header;
            Items = items;
        }
    }

    /// <summary>
    /// Service to transform DataTable records into AccountStatementWithDue report items.
    /// </summary>
    public static class AccountStatementWithDueDataService
    {
        /// <summary>
        /// Converts live DataTable from ReportQuery.AccountStatementWithDue into typed items,
        /// computing the cumulative running ledger balance.
        /// </summary>
        public static List<AccountStatementWithDueReportItem> FromDataTable(DataTable dt, decimal openingBalance = 0m)
        {
            var items = new List<AccountStatementWithDueReportItem>();
            if (dt == null) return items;

            decimal runningBalance = openingBalance;

            foreach (DataRow row in dt.Rows)
            {
                int? dueDays = null;
                if (row.Table.Columns.Contains("duedays") && row["duedays"] != DBNull.Value)
                {
                    if (int.TryParse(row["duedays"].ToString(), out int parsedDue))
                    {
                        dueDays = parsedDue;
                    }
                }

                var item = new AccountStatementWithDueReportItem
                {
                    Date = row.Table.Columns.Contains("vdate") && row["vdate"] != DBNull.Value
                        ? Convert.ToDateTime(row["vdate"])
                        : DateTime.Now,
                    VoucherNo = row.Table.Columns.Contains("vno") && row["vno"] != DBNull.Value
                        ? row["vno"].ToString()
                        : string.Empty,
                    Sequence = row.Table.Columns.Contains("vseq") && row["vseq"] != DBNull.Value
                        ? Convert.ToInt32(row["vseq"])
                        : 0,
                    Particular = row.Table.Columns.Contains("particular") && row["particular"] != DBNull.Value
                        ? row["particular"].ToString()
                        : string.Empty,
                    Debit = row.Table.Columns.Contains("dr") && row["dr"] != DBNull.Value
                        ? Convert.ToDecimal(row["dr"])
                        : 0m,
                    Credit = row.Table.Columns.Contains("cr") && row["cr"] != DBNull.Value
                        ? Convert.ToDecimal(row["cr"])
                        : 0m,
                    DueDays = dueDays
                };

                runningBalance += (item.Debit - item.Credit);
                item.Balance = runningBalance;
                items.Add(item);
            }

            return items;
        }
    }
}
