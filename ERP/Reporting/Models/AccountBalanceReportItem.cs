using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an individual subsidiary / customer / vendor / ledger balance line under a selected Account Head.
    /// </summary>
    public class AccountBalanceReportItem
    {
        public int Index { get; set; }
        public string AccountTitle { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }

        /// <summary>
        /// Nature indicator: "Dr" if balance >= 0, "Cr" if balance < 0.
        /// </summary>
        public string Nature => Balance >= 0 ? "Dr" : "Cr";

        public string FormattedBalance => Balance >= 0
            ? "Rs. " + Debit.ToString("N2") + " Dr"
            : "Rs. " + Credit.ToString("N2") + " Cr";
    }

    /// <summary>
    /// Holds summary metrics and header metadata for Account Balance (Balance Detail).
    /// </summary>
    public class AccountBalanceHeader
    {
        public string CompanyName { get; set; }
        public string AccountHeadTitle { get; set; }
        public string AccountHeadId { get; set; }
        public DateTime AsOnDate { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal NetBalance => TotalDebit - TotalCredit;
        public string NetNature => NetBalance >= 0 ? "Dr" : "Cr";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header metadata and items for the Account Balance report.
    /// </summary>
    public class AccountBalanceDataResult
    {
        public AccountBalanceHeader Header { get; set; }
        public List<AccountBalanceReportItem> Items { get; set; }

        public AccountBalanceDataResult(AccountBalanceHeader header, List<AccountBalanceReportItem> items)
        {
            Header = header;
            Items = items;
        }
    }

    /// <summary>
    /// Service to transform raw DataTables for Account Balance.
    /// </summary>
    public static class AccountBalanceDataService
    {
        public static AccountBalanceDataResult ConvertDataTable(
            DataTable dt,
            string accountHeadId,
            string accountHeadTitle,
            DateTime asOnDate,
            string balanceFilter = "All")
        {
            var items = new List<AccountBalanceReportItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                int index = 1;
                foreach (DataRow row in dt.Rows)
                {
                    string title = row.Table.Columns.Contains("account")
                        ? (row["account"] != DBNull.Value ? row["account"].ToString() : string.Empty)
                        : (row.Table.Columns.Contains("title") ? row["title"].ToString() : string.Empty);

                    decimal rawBalance = 0m;
                    if (row.Table.Columns.Contains("balance") && row["balance"] != DBNull.Value)
                    {
                        decimal.TryParse(row["balance"].ToString(), out rawBalance);
                    }

                    decimal debit = rawBalance > 0 ? rawBalance : 0m;
                    decimal credit = rawBalance < 0 ? Math.Abs(rawBalance) : 0m;

                    // Filter condition:
                    if (balanceFilter == "Debit" && debit <= 0) continue;
                    if (balanceFilter == "Credit" && credit <= 0) continue;

                    items.Add(new AccountBalanceReportItem
                    {
                        Index = index++,
                        AccountTitle = title,
                        Debit = debit,
                        Credit = credit,
                        Balance = rawBalance
                    });
                }
            }

            var header = new AccountBalanceHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                AccountHeadTitle = !string.IsNullOrWhiteSpace(accountHeadTitle) ? accountHeadTitle : "Account Head",
                AccountHeadId = accountHeadId ?? string.Empty,
                AsOnDate = asOnDate,
                TotalAccounts = items.Count,
                TotalDebit = items.Sum(x => x.Debit),
                TotalCredit = items.Sum(x => x.Credit),
                GeneratedAt = DateTime.Now
            };

            return new AccountBalanceDataResult(header, items);
        }
    }
}
