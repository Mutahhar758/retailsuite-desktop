using System;
using System.Collections.Generic;
using System.Data;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an individual ledger transaction in the Account Statement report.
    /// </summary>
    public class AccountStatementReportItem
    {
        public DateTime Date { get; set; }
        public string VoucherNo { get; set; }
        public int Sequence { get; set; }
        public string Particular { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// Holds report metadata and summary metrics for an Account Statement report.
    /// </summary>
    public class AccountStatementHeader
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
    /// Service to transform DataTable data into typed ledger items.
    /// </summary>
    public static class AccountStatementDataService
    {
        /// <summary>
        /// Converts a DataTable returned from ReportQuery.AccountStatement into typed ledger items
        /// and computes the cumulative running balance.
        /// </summary>
        public static List<AccountStatementReportItem> FromDataTable(DataTable dt, decimal openingBalance = 0m)
        {
            var items = new List<AccountStatementReportItem>();
            if (dt == null) return items;

            decimal runningBalance = openingBalance;

            foreach (DataRow row in dt.Rows)
            {
                var item = new AccountStatementReportItem
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
                        : 0m
                };

                runningBalance += (item.Debit - item.Credit);
                item.Balance = runningBalance;
                items.Add(item);
            }

            return items;
        }
    }

    /// <summary>
    /// Result container combining the header and ledger line items.
    /// </summary>
    public class AccountStatementDataResult
    {
        public AccountStatementHeader Header { get; set; }
        public List<AccountStatementReportItem> Items { get; set; }

        public AccountStatementDataResult()
        {
            Items = new List<AccountStatementReportItem>();
        }

        public AccountStatementDataResult(AccountStatementHeader header, List<AccountStatementReportItem> items)
        {
            Header = header;
            Items = items ?? new List<AccountStatementReportItem>();
        }
    }
}
