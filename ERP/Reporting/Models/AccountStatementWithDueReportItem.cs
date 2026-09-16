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
    /// Service to transform ReportQuery.AccountStatementWithDue DataTable results
    /// and provide realistic sample mock data for offline/fallback mode.
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

        /// <summary>
        /// Generates 15+ realistic retail account ledger transactions with due days and due dates.
        /// </summary>
        public static AccountStatementWithDueDataResult GetSampleAccountStatementWithDue(string companyName = null)
        {
            decimal openingBal = 25000.00m;
            var header = new AccountStatementWithDueHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(companyName)
                    ? companyName
                    : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise"),
                AccountTitle = "Prime Wholesale Distribution Ledger",
                AccountCode = "001-002-001-001",
                FromDate = DateTime.Today.AddDays(-45),
                ToDate = DateTime.Today,
                OpeningBalance = openingBal,
                DateBasis = "Voucher Date"
            };

            var items = new List<AccountStatementWithDueReportItem>();
            decimal currentBal = openingBal;

            void AddRow(int dayOffset, string vno, int seq, string desc, decimal dr, decimal cr, int? dueDays)
            {
                DateTime dt = header.FromDate.AddDays(dayOffset);
                currentBal += (dr - cr);
                items.Add(new AccountStatementWithDueReportItem
                {
                    Date = dt,
                    VoucherNo = vno,
                    Sequence = seq,
                    Particular = desc,
                    Debit = dr,
                    Credit = cr,
                    DueDays = dueDays,
                    Balance = currentBal
                });
            }

            AddRow(1, "SL-10142", 1, "Wholesale Invoice - Fast Moving Consumer Goods (FMCG)", 42500.00m, 0m, 30);
            AddRow(3, "RV-05210", 1, "Direct Bank Wire Settlement - HBL Corporate Portal", 0m, 35000.00m, null);
            AddRow(6, "SL-10189", 1, "Bulk Merchandise Shipment - Batch #49021", 58900.00m, 0m, 30);
            AddRow(8, "SR-01124", 1, "Authorized Credit Note - Defective Outer Packaging Returned", 0m, 4200.00m, null);
            AddRow(11, "RV-05244", 1, "Clearing Cheque #992812 - Standard Chartered Bank", 0m, 40000.00m, null);
            AddRow(14, "SL-10235", 1, "Commercial Supply - Central Distribution Center Depot", 71250.00m, 0m, 45);
            AddRow(17, "JV-08311", 1, "Year-End Volume Rebate & Promotional Loyalty Allowance", 0m, 5000.00m, null);
            AddRow(20, "SL-10290", 1, "Standard Consignment - Dairy & Cold Chain SKU Replenishment", 33400.00m, 0m, 15);
            AddRow(23, "RV-05302", 1, "Interbank Electronic Funds Transfer (1LINK FT)", 0m, 50000.00m, null);
            AddRow(26, "SL-10344", 1, "Enterprise Order - Specialized Institutional Supplies", 86000.00m, 0m, 30);
            AddRow(29, "SL-10398", 1, "Supplementary Invoicing - Express Pallet Delivery", 12800.00m, 0m, 15);
            AddRow(32, "RV-05389", 1, "Cashier Counter Receipt - Partial Advance Payment", 0m, 25000.00m, null);
            AddRow(35, "SL-10442", 1, "Retailer Store Distribution Stocking Order", 64350.00m, 0m, 30);
            AddRow(38, "RV-05421", 1, "RTGS High-Value Institutional Settlement", 0m, 60000.00m, null);
            AddRow(41, "SL-10490", 1, "Monthly Bulk Confectionery & Beverage Supply", 38750.00m, 0m, 20);
            AddRow(44, "RV-05490", 1, "Weekly Balance Reconciliation Transfer", 0m, 30000.00m, null);

            header.TotalDebit = 0m;
            header.TotalCredit = 0m;
            foreach (var item in items)
            {
                header.TotalDebit += item.Debit;
                header.TotalCredit += item.Credit;
            }
            header.ClosingBalance = currentBal;

            return new AccountStatementWithDueDataResult(header, items);
        }
    }
}
