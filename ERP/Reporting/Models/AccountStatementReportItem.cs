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
    /// Service to transform DataTable data and produce realistic mock ledger data.
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

        /// <summary>
        /// Generates 15+ realistic retail account ledger transactions for preview and offline testing.
        /// </summary>
        public static AccountStatementDataResult GetSampleAccountStatement(string companyName = null)
        {
            decimal openingBal = 15000.00m;
            var header = new AccountStatementHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(companyName)
                    ? companyName
                    : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise"),
                AccountTitle = "General Trading & Distribution A/C",
                AccountCode = "001-002-001-001",
                FromDate = DateTime.Today.AddDays(-30),
                ToDate = DateTime.Today,
                OpeningBalance = openingBal
            };

            var rawTransactions = new[]
            {
                new { Day = -28, VNo = "OB-001", Desc = "Opening Balance Carried Forward", Dr = 0m, Cr = 0m },
                new { Day = -27, VNo = "SL-1021", Desc = "Sale of Groceries & Packaged Goods", Dr = 4500.00m, Cr = 0m },
                new { Day = -25, VNo = "RV-2041", Desc = "Customer Payment Received (Cash)", Dr = 0m, Cr = 3000.00m },
                new { Day = -24, VNo = "SL-1028", Desc = "Wholesale Beverage Supply Batch #401", Dr = 7800.00m, Cr = 0m },
                new { Day = -22, VNo = "PV-3012", Desc = "Payment to Vendor - Freight & Logistics", Dr = 0m, Cr = 1250.00m },
                new { Day = -20, VNo = "JV-4005", Desc = "Adjustment for Damaged Goods Return", Dr = 0m, Cr = 650.00m },
                new { Day = -19, VNo = "SL-1035", Desc = "Counter Sale - Electronics & Hardware", Dr = 6200.00m, Cr = 0m },
                new { Day = -17, VNo = "RV-2055", Desc = "Direct Bank Transfer from Customer A/C", Dr = 0m, Cr = 5000.00m },
                new { Day = -15, VNo = "SL-1042", Desc = "Invoice #1042 - Monthly Provision Supplies", Dr = 9450.00m, Cr = 0m },
                new { Day = -13, VNo = "PR-5011", Desc = "Purchase Return - Dairy Products", Dr = 1100.00m, Cr = 0m },
                new { Day = -11, VNo = "PV-3029", Desc = "Utility Bill Payment via Cheque #4412", Dr = 0m, Cr = 2100.00m },
                new { Day = -9,  VNo = "SL-1051", Desc = "Sale of Household & Cleaning Supplies", Dr = 3850.00m, Cr = 0m },
                new { Day = -7,  VNo = "RV-2068", Desc = "Settlement of Invoice #1028 in full", Dr = 0m, Cr = 4800.00m },
                new { Day = -5,  VNo = "SL-1060", Desc = "Retail Counter Sale - Assorted Items", Dr = 5250.00m, Cr = 0m },
                new { Day = -3,  VNo = "JV-4018", Desc = "Inter-branch Stock Transfer Reconciled", Dr = 1800.00m, Cr = 0m },
                new { Day = -1,  VNo = "RV-2079", Desc = "Online Merchant Gateway Settlement", Dr = 0m, Cr = 3500.00m }
            };

            var items = new List<AccountStatementReportItem>();
            decimal runningBalance = openingBal;
            decimal totalDebit = 0m;
            decimal totalCredit = 0m;
            int seq = 1;

            foreach (var t in rawTransactions)
            {
                runningBalance += (t.Dr - t.Cr);
                totalDebit += t.Dr;
                totalCredit += t.Cr;

                items.Add(new AccountStatementReportItem
                {
                    Date = DateTime.Today.AddDays(t.Day),
                    VoucherNo = t.VNo,
                    Sequence = seq++,
                    Particular = t.Desc,
                    Debit = t.Dr,
                    Credit = t.Cr,
                    Balance = runningBalance
                });
            }

            header.TotalDebit = totalDebit;
            header.TotalCredit = totalCredit;
            header.ClosingBalance = runningBalance;

            return new AccountStatementDataResult(header, items);
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
