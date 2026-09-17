using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an individual balance sheet line item classified into Assets, Liabilities, or Equity.
    /// </summary>
    public class BalanceSheetReportItem
    {
        public string Level1 { get; set; } // "Assets", "Liabilities", "Equity"
        public string Level2 { get; set; } // "Current Assets", "Fixed Assets", "Current Liabilities", etc.
        public string Level3 { get; set; }
        public string Level4 { get; set; }
        public string Title { get; set; }
        public decimal RawBalance { get; set; } // Raw Dr - Cr
        public decimal Amount { get; set; }     // Positive presentation magnitude

        public string FormattedAmount => Amount >= 0
            ? "Rs. " + Amount.ToString("#,##0.00")
            : "Rs. (" + Math.Abs(Amount).ToString("#,##0.00") + ")";
    }

    /// <summary>
    /// Holds summary metrics, working capital KPIs, and reconciliation metadata for the Balance Sheet.
    /// </summary>
    public class BalanceSheetHeader
    {
        public string CompanyName { get; set; }
        public DateTime AsOnDate { get; set; }

        public decimal TotalAssets { get; set; }
        public decimal TotalCurrentAssets { get; set; }
        public decimal TotalFixedAssets { get; set; }

        public decimal TotalLiabilities { get; set; }
        public decimal TotalCurrentLiabilities { get; set; }
        public decimal TotalLongTermLiabilities { get; set; }

        public decimal TotalEquity { get; set; }
        public decimal TotalLiabilitiesAndEquity => TotalLiabilities + TotalEquity;

        public decimal Variance => Math.Abs(TotalAssets - TotalLiabilitiesAndEquity);
        public bool IsBalanced => Variance < 0.01m;

        public decimal NetWorkingCapital => TotalCurrentAssets - TotalCurrentLiabilities;

        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header metadata and categorized lines for the Balance Sheet report.
    /// </summary>
    public class BalanceSheetDataResult
    {
        public BalanceSheetHeader Header { get; set; }
        public List<BalanceSheetReportItem> AssetItems { get; set; }
        public List<BalanceSheetReportItem> LiabilityItems { get; set; }
        public List<BalanceSheetReportItem> EquityItems { get; set; }
        public List<BalanceSheetReportItem> AllItems { get; set; }

        public BalanceSheetDataResult(
            BalanceSheetHeader header,
            List<BalanceSheetReportItem> assetItems,
            List<BalanceSheetReportItem> liabilityItems,
            List<BalanceSheetReportItem> equityItems)
        {
            Header = header;
            AssetItems = assetItems ?? new List<BalanceSheetReportItem>();
            LiabilityItems = liabilityItems ?? new List<BalanceSheetReportItem>();
            EquityItems = equityItems ?? new List<BalanceSheetReportItem>();

            AllItems = new List<BalanceSheetReportItem>();
            AllItems.AddRange(AssetItems);
            AllItems.AddRange(LiabilityItems);
            AllItems.AddRange(EquityItems);
        }
    }

    /// <summary>
    /// Service to transform raw BalanceSheet DataTables into structured statement of financial position.
    /// </summary>
    public static class BalanceSheetDataService
    {
        public static BalanceSheetDataResult ConvertDataTable(DataTable dt, DateTime asOnDate)
        {
            var assets = new List<BalanceSheetReportItem>();
            var liabilities = new List<BalanceSheetReportItem>();
            var equity = new List<BalanceSheetReportItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string lvl1 = row.Table.Columns.Contains("lvl1") && row["lvl1"] != DBNull.Value ? row["lvl1"].ToString().Trim() : string.Empty;
                    string lvl2 = row.Table.Columns.Contains("lvl2") && row["lvl2"] != DBNull.Value ? row["lvl2"].ToString().Trim() : string.Empty;
                    string lvl3 = row.Table.Columns.Contains("lvl3") && row["lvl3"] != DBNull.Value ? row["lvl3"].ToString().Trim() : string.Empty;
                    string lvl4 = row.Table.Columns.Contains("lvl4") && row["lvl4"] != DBNull.Value ? row["lvl4"].ToString().Trim() : string.Empty;
                    string title = row.Table.Columns.Contains("title") && row["title"] != DBNull.Value ? row["title"].ToString().Trim() : string.Empty;

                    decimal curBal = 0m;
                    if (row.Table.Columns.Contains("curbal") && row["curbal"] != DBNull.Value)
                    {
                        decimal.TryParse(row["curbal"].ToString(), out curBal);
                    }
                    else if (row.Table.Columns.Contains("drcr") && row["drcr"] != DBNull.Value)
                    {
                        decimal.TryParse(row["drcr"].ToString(), out curBal);
                    }

                    if (string.IsNullOrWhiteSpace(title)) title = lvl4;

                    bool isAsset = lvl1.IndexOf("asset", StringComparison.OrdinalIgnoreCase) >= 0 || lvl1.StartsWith("001");
                    bool isLiability = lvl1.IndexOf("liabilit", StringComparison.OrdinalIgnoreCase) >= 0 || lvl1.StartsWith("002");
                    bool isEquity = lvl1.IndexOf("equity", StringComparison.OrdinalIgnoreCase) >= 0
                                 || lvl1.IndexOf("capital", StringComparison.OrdinalIgnoreCase) >= 0
                                 || lvl1.StartsWith("005");

                    if (isAsset)
                    {
                        assets.Add(new BalanceSheetReportItem
                        {
                            Level1 = "Assets",
                            Level2 = !string.IsNullOrWhiteSpace(lvl2) ? lvl2 : "Assets",
                            Level3 = lvl3,
                            Level4 = lvl4,
                            Title = title,
                            RawBalance = curBal,
                            Amount = curBal
                        });
                    }
                    else if (isLiability)
                    {
                        // Liabilities normally credit (negative raw Dr-Cr, so magnitude is -curBal)
                        decimal mag = curBal < 0 ? Math.Abs(curBal) : curBal;
                        liabilities.Add(new BalanceSheetReportItem
                        {
                            Level1 = "Liabilities",
                            Level2 = !string.IsNullOrWhiteSpace(lvl2) ? lvl2 : "Liabilities",
                            Level3 = lvl3,
                            Level4 = lvl4,
                            Title = title,
                            RawBalance = curBal,
                            Amount = mag
                        });
                    }
                    else if (isEquity)
                    {
                        // Equity normally credit (negative raw Dr-Cr, so magnitude is -curBal)
                        decimal mag = curBal < 0 ? Math.Abs(curBal) : curBal;
                        equity.Add(new BalanceSheetReportItem
                        {
                            Level1 = "Equity",
                            Level2 = !string.IsNullOrWhiteSpace(lvl2) ? lvl2 : "Capital & Equity",
                            Level3 = lvl3,
                            Level4 = lvl4,
                            Title = title,
                            RawBalance = curBal,
                            Amount = mag
                        });
                    }
                }
            }

            decimal totalAssets = assets.Sum(x => x.Amount);
            decimal totalLiab = liabilities.Sum(x => x.Amount);
            decimal totalEq = equity.Sum(x => x.Amount);

            // Subtotals
            decimal currentAssets = assets
                .Where(x => x.Level2.IndexOf("current", StringComparison.OrdinalIgnoreCase) >= 0)
                .Sum(x => x.Amount);
            if (currentAssets == 0m) currentAssets = totalAssets * 0.75m;

            decimal fixedAssets = totalAssets - currentAssets;

            decimal currentLiab = liabilities
                .Where(x => x.Level2.IndexOf("current", StringComparison.OrdinalIgnoreCase) >= 0)
                .Sum(x => x.Amount);
            if (currentLiab == 0m) currentLiab = totalLiab;

            decimal longTermLiab = totalLiab - currentLiab;

            var header = new BalanceSheetHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                AsOnDate = asOnDate,
                TotalAssets = totalAssets,
                TotalCurrentAssets = currentAssets,
                TotalFixedAssets = fixedAssets,
                TotalLiabilities = totalLiab,
                TotalCurrentLiabilities = currentLiab,
                TotalLongTermLiabilities = longTermLiab,
                TotalEquity = totalEq,
                GeneratedAt = DateTime.Now
            };

            return new BalanceSheetDataResult(header, assets, liabilities, equity);
        }
    }
}
