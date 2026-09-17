using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ERP.Services.Legacy;

namespace ERP.Reporting.Models
{
    public class CustomerBalanceRecoveryLineItem
    {
        public string CustomerAccountId { get; set; }
        public string CustomerTitle { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBilling { get; set; }
        public decimal TotalDue { get; set; }
        public decimal RecoveryAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal RecoveryPercentage { get; set; }
        public string Status { get; set; } // "Cleared", "Partial", "Unpaid", "Advance"
    }

    public class CustomerBalanceRecoverySummary
    {
        public int TotalCustomers { get; set; }
        public decimal TotalPreviousBalance { get; set; }
        public decimal TotalCurrentBilling { get; set; }
        public decimal TotalDue { get; set; }
        public decimal TotalRecovery { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalClosingBalance { get; set; }
        public decimal OverallRecoveryRate { get; set; }
    }

    public class CustomerBalanceRecoveryHeader
    {
        public string CompanyName { get; set; } = !string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise";
        public string ReportTitle { get; set; } = "CUSTOMER BALANCE & RECOVERY REPORT";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string DateBasis { get; set; } = "ClearingDate"; // "ClearingDate" or "VoucherDate"
        public string BalanceFilter { get; set; } = "All"; // "All", "OutstandingOnly", "ClearedOnly", "UnpaidOnly"
        public string CustomerFilter { get; set; } = "All Customers";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    public static class CustomerBalanceRecoveryDataService
    {
        public static async Task<(CustomerBalanceRecoveryHeader Header, List<CustomerBalanceRecoveryLineItem> Lines, CustomerBalanceRecoverySummary Summary)> GetRecoveryDataAsync(
            DateTime fromDate,
            DateTime toDate,
            string customerAccountId = null,
            string customerTitle = null,
            string dateBasis = "ClearingDate",
            string balanceFilter = "All")
        {
            var header = new CustomerBalanceRecoveryHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise",
                FromDate = fromDate,
                ToDate = toDate,
                DateBasis = dateBasis ?? "ClearingDate",
                BalanceFilter = balanceFilter ?? "All",
                CustomerFilter = !string.IsNullOrWhiteSpace(customerTitle) ? customerTitle : "All Customers",
                GeneratedAt = DateTime.Now
            };

            try
            {
                var reportsApi = new ReportsApiService();
                var dt = await reportsApi.GetCustomerBalanceRecoveryAsync(fromDate, toDate, customerAccountId, dateBasis, balanceFilter);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var lines = new List<CustomerBalanceRecoveryLineItem>();

                    foreach (DataRow row in dt.Rows)
                    {
                        var line = new CustomerBalanceRecoveryLineItem
                        {
                            CustomerAccountId = row.Table.Columns.Contains("CustomerAccountId") ? Convert.ToString(row["CustomerAccountId"]) : string.Empty,
                            CustomerTitle = row.Table.Columns.Contains("CustomerTitle") ? Convert.ToString(row["CustomerTitle"]) : string.Empty,
                            Phone = row.Table.Columns.Contains("Phone") ? Convert.ToString(row["Phone"]) : string.Empty,
                            Address = row.Table.Columns.Contains("Address") ? Convert.ToString(row["Address"]) : string.Empty,
                            PreviousBalance = row.Table.Columns.Contains("PreviousBalance") && row["PreviousBalance"] != DBNull.Value ? Convert.ToDecimal(row["PreviousBalance"]) : 0m,
                            CurrentBilling = row.Table.Columns.Contains("CurrentBilling") && row["CurrentBilling"] != DBNull.Value ? Convert.ToDecimal(row["CurrentBilling"]) : 0m,
                            TotalDue = row.Table.Columns.Contains("TotalDue") && row["TotalDue"] != DBNull.Value ? Convert.ToDecimal(row["TotalDue"]) : 0m,
                            RecoveryAmount = row.Table.Columns.Contains("RecoveryAmount") && row["RecoveryAmount"] != DBNull.Value ? Convert.ToDecimal(row["RecoveryAmount"]) : 0m,
                            ClosingBalance = row.Table.Columns.Contains("ClosingBalance") && row["ClosingBalance"] != DBNull.Value ? Convert.ToDecimal(row["ClosingBalance"]) : 0m,
                            RecoveryPercentage = row.Table.Columns.Contains("RecoveryPercentage") && row["RecoveryPercentage"] != DBNull.Value ? Convert.ToDecimal(row["RecoveryPercentage"]) : 0m,
                            Status = row.Table.Columns.Contains("Status") ? Convert.ToString(row["Status"]) : "Unpaid"
                        };

                        if (line.TotalDue <= 0) line.TotalDue = line.PreviousBalance + line.CurrentBilling;
                        if (line.ClosingBalance == 0 && line.TotalDue > 0) line.ClosingBalance = line.TotalDue - line.RecoveryAmount - line.Discount;

                        if (line.TotalDue > 0 && line.RecoveryPercentage == 0)
                        {
                            line.RecoveryPercentage = Math.Min(100m, Math.Max(0m, (line.RecoveryAmount / line.TotalDue) * 100m));
                        }

                        if (string.IsNullOrWhiteSpace(line.Status))
                        {
                            if (line.ClosingBalance <= 0) line.Status = "Cleared";
                            else if (line.RecoveryAmount > 0) line.Status = "Partial";
                            else line.Status = "Unpaid";
                        }

                        lines.Add(line);
                    }

                    // Apply balance filter if client-side filtering needed
                    lines = ApplyClientFilter(lines, balanceFilter);
                    var summary = CalculateSummary(lines);
                    return (header, lines, summary);
                }
            }
            catch
            {
                // Return empty if database query fails or returns nothing
            }

            var emptyLines = new List<CustomerBalanceRecoveryLineItem>();
            var emptySummary = CalculateSummary(emptyLines);
            return (header, emptyLines, emptySummary);
        }

        private static List<CustomerBalanceRecoveryLineItem> ApplyClientFilter(List<CustomerBalanceRecoveryLineItem> list, string filter)
        {
            if (string.Equals(filter, "OutstandingOnly", StringComparison.OrdinalIgnoreCase))
                return list.Where(x => x.ClosingBalance > 0).ToList();
            if (string.Equals(filter, "ClearedOnly", StringComparison.OrdinalIgnoreCase))
                return list.Where(x => x.ClosingBalance <= 0 || string.Equals(x.Status, "Cleared", StringComparison.OrdinalIgnoreCase)).ToList();
            if (string.Equals(filter, "UnpaidOnly", StringComparison.OrdinalIgnoreCase))
                return list.Where(x => x.RecoveryAmount <= 0 && x.TotalDue > 0).ToList();
            return list;
        }

        public static CustomerBalanceRecoverySummary CalculateSummary(List<CustomerBalanceRecoveryLineItem> lines)
        {
            var summary = new CustomerBalanceRecoverySummary();
            if (lines == null || lines.Count == 0) return summary;

            summary.TotalCustomers = lines.Count;
            summary.TotalPreviousBalance = lines.Sum(x => x.PreviousBalance);
            summary.TotalCurrentBilling = lines.Sum(x => x.CurrentBilling);
            summary.TotalDue = lines.Sum(x => x.TotalDue);
            summary.TotalRecovery = lines.Sum(x => x.RecoveryAmount);
            summary.TotalDiscount = lines.Sum(x => x.Discount);
            summary.TotalClosingBalance = lines.Sum(x => x.ClosingBalance);
            summary.OverallRecoveryRate = summary.TotalDue > 0 ? ((summary.TotalRecovery / summary.TotalDue) * 100m) : 0m;

            return summary;
        }
    }
}
