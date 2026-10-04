using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ERP.Services.Legacy;

namespace ERP.Reporting.Models
{
    public class PurchaseSupplyComparisonLineItem
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public decimal PurchaseQty { get; set; }
        public decimal PurchaseAvgRate { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SupplyQty { get; set; }
        public decimal SupplyAvgRate { get; set; }
        public decimal SupplyAmount { get; set; }
        public decimal RegularSaleQty { get; set; }
        public decimal RegularSaleAmount { get; set; }
        public decimal TotalDispatchedQty { get; set; }
        public decimal DiffQty { get; set; }
        public decimal DiffAmount { get; set; }
        public decimal NetDiffQty { get; set; }
        public string Status { get; set; } // "Surplus", "Shortage", "Equal"
    }

    public class PurchaseSupplyComparisonSummary
    {
        public decimal TotalPurchaseQty { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public decimal AvgPurchaseRate { get; set; }
        public decimal TotalSupplyQty { get; set; }
        public decimal TotalSupplyAmount { get; set; }
        public decimal AvgSupplyRate { get; set; }
        public decimal TotalRegularSaleQty { get; set; }
        public decimal TotalRegularSaleAmount { get; set; }
        public decimal TotalDispatchedQty { get; set; }
        public decimal TotalDiffQty { get; set; }
        public decimal TotalDiffAmount { get; set; }
        public decimal TotalNetDiffQty { get; set; }
    }

    public class PurchaseSupplyComparisonHeader
    {
        public string CompanyName { get; set; } = !string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise";
        public string ReportTitle { get; set; } = "PURCHASE VS SUPPLY & SALE COMPARISON";
        public string ItemTitle { get; set; } = "Item / Product";
        public string UnitTitle { get; set; } = "Unit";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    public static class PurchaseSupplyComparisonDataService
    {
        public static async Task<(PurchaseSupplyComparisonHeader Header, List<PurchaseSupplyComparisonLineItem> Lines, PurchaseSupplyComparisonSummary Summary)> GetComparisonDataAsync(
            DateTime fromDate,
            DateTime toDate,
            string itemId,
            string itemTitle = null)
        {
            var header = new PurchaseSupplyComparisonHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise",
                ItemTitle = !string.IsNullOrWhiteSpace(itemTitle) ? itemTitle : "Item / Product",
                UnitTitle = "Unit",
                FromDate = fromDate,
                ToDate = toDate,
                GeneratedAt = DateTime.Now
            };

            try
            {
                var reportsApi = new ReportsApiService();
                var dt = await reportsApi.GetPurchaseSupplyComparisonAsync(fromDate, toDate, itemId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var lines = new List<PurchaseSupplyComparisonLineItem>();
                    decimal runningNetDiff = 0m;

                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new PurchaseSupplyComparisonLineItem
                        {
                            Date = row.Table.Columns.Contains("Date") && row["Date"] != DBNull.Value ? Convert.ToDateTime(row["Date"]) : DateTime.Today,
                            DayName = row.Table.Columns.Contains("DayName") ? Convert.ToString(row["DayName"]) : string.Empty,
                            PurchaseQty = row.Table.Columns.Contains("PurchaseQty") && row["PurchaseQty"] != DBNull.Value ? Convert.ToDecimal(row["PurchaseQty"]) : 0m,
                            PurchaseAvgRate = row.Table.Columns.Contains("PurchaseAvgRate") && row["PurchaseAvgRate"] != DBNull.Value ? Convert.ToDecimal(row["PurchaseAvgRate"]) : 0m,
                            PurchaseAmount = row.Table.Columns.Contains("PurchaseAmount") && row["PurchaseAmount"] != DBNull.Value ? Convert.ToDecimal(row["PurchaseAmount"]) : 0m,
                            SupplyQty = row.Table.Columns.Contains("SupplyQty") && row["SupplyQty"] != DBNull.Value ? Convert.ToDecimal(row["SupplyQty"]) : 0m,
                            SupplyAvgRate = row.Table.Columns.Contains("SupplyAvgRate") && row["SupplyAvgRate"] != DBNull.Value ? Convert.ToDecimal(row["SupplyAvgRate"]) : 0m,
                            SupplyAmount = row.Table.Columns.Contains("SupplyAmount") && row["SupplyAmount"] != DBNull.Value ? Convert.ToDecimal(row["SupplyAmount"]) : 0m,
                            DiffQty = row.Table.Columns.Contains("DiffQty") && row["DiffQty"] != DBNull.Value ? Convert.ToDecimal(row["DiffQty"]) : 0m,
                            DiffAmount = row.Table.Columns.Contains("DiffAmount") && row["DiffAmount"] != DBNull.Value ? Convert.ToDecimal(row["DiffAmount"]) : 0m,
                            Status = row.Table.Columns.Contains("Status") ? Convert.ToString(row["Status"]) : "Equal"
                        };

                        item.TotalDispatchedQty = item.SupplyQty + item.RegularSaleQty;
                        if (string.IsNullOrEmpty(item.DayName))
                        {
                            item.DayName = item.Date.ToString("ddd");
                        }

                        // Reconcile Net Cumulative Diff
                        runningNetDiff += (item.PurchaseQty - item.TotalDispatchedQty);
                        item.NetDiffQty = runningNetDiff;

                        if (string.IsNullOrWhiteSpace(item.Status))
                        {
                            if (item.PurchaseQty > item.TotalDispatchedQty) item.Status = "Surplus";
                            else if (item.PurchaseQty < item.TotalDispatchedQty) item.Status = "Shortage";
                            else item.Status = "Equal";
                        }

                        lines.Add(item);
                    }

                    var summary = CalculateSummary(lines);
                    return (header, lines, summary);
                }
            }
            catch
            {
                // Return empty if database query fails or returns nothing
            }

            var emptyLines = new List<PurchaseSupplyComparisonLineItem>();
            var emptySummary = CalculateSummary(emptyLines);
            return (header, emptyLines, emptySummary);
        }

        public static PurchaseSupplyComparisonSummary CalculateSummary(List<PurchaseSupplyComparisonLineItem> lines)
        {
            var summary = new PurchaseSupplyComparisonSummary();
            if (lines == null || lines.Count == 0) return summary;

            summary.TotalPurchaseQty = lines.Sum(x => x.PurchaseQty);
            summary.TotalPurchaseAmount = lines.Sum(x => x.PurchaseAmount);
            summary.AvgPurchaseRate = summary.TotalPurchaseQty > 0 ? (summary.TotalPurchaseAmount / summary.TotalPurchaseQty) : 0m;

            summary.TotalSupplyQty = lines.Sum(x => x.SupplyQty);
            summary.TotalSupplyAmount = lines.Sum(x => x.SupplyAmount);
            summary.AvgSupplyRate = summary.TotalSupplyQty > 0 ? (summary.TotalSupplyAmount / summary.TotalSupplyQty) : 0m;

            summary.TotalRegularSaleQty = lines.Sum(x => x.RegularSaleQty);
            summary.TotalRegularSaleAmount = lines.Sum(x => x.RegularSaleAmount);

            summary.TotalDispatchedQty = lines.Sum(x => x.TotalDispatchedQty);
            summary.TotalDiffQty = lines.Sum(x => x.DiffQty);
            summary.TotalDiffAmount = lines.Sum(x => x.DiffAmount);
            summary.TotalNetDiffQty = summary.TotalPurchaseQty - summary.TotalDispatchedQty;

            return summary;
        }
    }
}
