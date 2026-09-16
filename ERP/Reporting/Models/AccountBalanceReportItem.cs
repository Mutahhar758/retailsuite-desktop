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
    /// Service to transform raw DataTables and provide mock testing datasets for Account Balance.
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

        public static AccountBalanceDataResult GetMockData(
            string accountHeadId,
            string accountHeadTitle,
            DateTime asOnDate,
            string balanceFilter = "All")
        {
            var mockEntries = new List<(string Title, decimal Balance)>
            {
                ("Al-Madina Superstore - Faisalabad", 45000.00m),
                ("Bismillah Traders - Gujranwala", 128500.00m),
                ("Chenab Valley General Store", -15000.00m),
                ("Crown Cash & Carry - Lahore", 89400.00m),
                ("Diamond Mart - Islamabad", 23100.00m),
                ("Faisal Wholesale Center", -8400.00m),
                ("Ghazi Traders & Distributors", 67800.00m),
                ("Haseeb Provision Store - Rawalpindi", 14350.00m),
                ("Ittifaq Departmental Store", 92000.00m),
                ("Jinnah Super Market", 54100.00m),
                ("Khyber Trading Agency - Peshawar", 112000.00m),
                ("Lahore General Store", -32000.00m),
                ("Metro Mega Mart - Sialkot", 76400.00m),
                ("National Mart & Bakery", 18900.00m),
                ("Pak Pearl Cash & Carry", 42750.00m),
                ("Rehman & Sons Enterprise", 98300.00m),
                ("Siddiqui Mart - Multan", 31200.00m),
                ("United Wholesale Agency", -11500.00m),
                ("Vertex Retail Outlet", 27600.00m),
                ("Zubair Brothers Provision", 58900.00m)
            };

            var items = new List<AccountBalanceReportItem>();
            int idx = 1;

            foreach (var entry in mockEntries)
            {
                decimal debit = entry.Balance > 0 ? entry.Balance : 0m;
                decimal credit = entry.Balance < 0 ? Math.Abs(entry.Balance) : 0m;

                if (balanceFilter == "Debit" && debit <= 0) continue;
                if (balanceFilter == "Credit" && credit <= 0) continue;

                items.Add(new AccountBalanceReportItem
                {
                    Index = idx++,
                    AccountTitle = entry.Title,
                    Debit = debit,
                    Credit = credit,
                    Balance = entry.Balance
                });
            }

            string headName = !string.IsNullOrWhiteSpace(accountHeadTitle) ? accountHeadTitle : "Customers / Trade Debtors";

            var header = new AccountBalanceHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                AccountHeadTitle = headName,
                AccountHeadId = !string.IsNullOrWhiteSpace(accountHeadId) ? accountHeadId : "01-01-001-0001",
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
