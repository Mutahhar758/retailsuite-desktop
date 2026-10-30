using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Classes;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Wanda (variable pack) tenants view stock in bags with the kg equivalent, e.g. "5 bags (250 kg)".
    /// Other tenants keep plain quantities.
    /// </summary>
    public static class ItemLedgerFormat
    {
        public static bool UseBags { get { return ApiSession.HasVariablePackFeature; } }

        public static string BagsKg(decimal bags, decimal kg, string unit)
        {
            string u = string.IsNullOrWhiteSpace(unit) ? "kg" : unit.Trim();
            string kgText = kg % 1m == 0m ? kg.ToString("N0") : kg.ToString("#,##0.###");
            if (bags == 0m)
            {
                return kg == 0m ? "0" : kgText + " " + u;
            }
            string bagText = bags % 1m == 0m ? bags.ToString("N0") : bags.ToString("#,##0.###");
            string label = Math.Abs(bags) == 1m ? "bag" : "bags";
            decimal perBag = Math.Round(kg / bags, 2);
            if (Math.Abs((perBag * bags) - kg) <= 0.01m)
            {
                string perBagText = perBag % 1m == 0m ? perBag.ToString("N0") : perBag.ToString("#,##0.##");
                return bagText + " " + label + " @ " + perBagText + " " + u;
            }
            return bagText + " " + label + " (" + kgText + " " + u + ")";
        }
    }

    /// <summary>
    /// Represents an individual stock ledger movement transaction (Inward, Outward, Opening).
    /// </summary>
    public class ItemLedgerReportItem
    {
        public DateTime Date { get; set; }
        public string VoucherNo { get; set; }
        public string Particular { get; set; }
        public decimal? Rate { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal QtyIn { get; set; }
        public decimal QtyOut { get; set; }
        public decimal Balance { get; set; }
        public decimal SecQtyIn { get; set; }
        public decimal SecQtyOut { get; set; }
        public decimal SecBalance { get; set; }
        public string UnitLabel { get; set; }

        public string FormattedDate => Date.ToString("dd-MMM-yyyy");
        public string FormattedRate => Rate.HasValue && Rate.Value > 0 ? Rate.Value.ToString("N2") : "-";
        public string FormattedCostPrice => CostPrice.HasValue && CostPrice.Value > 0
            ? CostPrice.Value.ToString("N2")
            : (QtyIn > 0 && Rate.HasValue && Rate.Value > 0
                ? Rate.Value.ToString("N2")
                : (CostPrice.HasValue ? "0.00" : "-"));
        public string FormattedQtyIn => QtyIn > 0
            ? (ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(SecQtyIn, QtyIn, UnitLabel) : QtyIn.ToString("N2"))
            : "-";
        public string FormattedQtyOut => QtyOut > 0
            ? (ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(SecQtyOut, QtyOut, UnitLabel) : QtyOut.ToString("N2"))
            : "-";
        public string FormattedBalance => ItemLedgerFormat.UseBags
            ? ItemLedgerFormat.BagsKg(SecBalance, Balance, UnitLabel)
            : Balance.ToString("N2");
    }

    /// <summary>
    /// Holds summary metrics and header metadata for the Item Ledger / Stock Ledger.
    /// </summary>
    public class ItemLedgerHeader
    {
        public string CompanyName { get; set; }
        public string ItemId { get; set; }
        public string ItemTitle { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal OpeningSec { get; set; }
        public decimal TotalInSec { get; set; }
        public decimal TotalOutSec { get; set; }
        public decimal ClosingSec { get; set; }
        public string UnitLabel { get; set; }

        public string OpeningDisplay { get { return Show(OpeningSec, OpeningBalance); } }
        public string TotalInDisplay { get { return Show(TotalInSec, TotalIn); } }
        public string TotalOutDisplay { get { return Show(TotalOutSec, TotalOut); } }
        public string ClosingDisplay { get { return Show(ClosingSec, ClosingBalance); } }

        private string Show(decimal bags, decimal qty)
        {
            return ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(bags, qty, UnitLabel) : qty.ToString("#,##0.00");
        }

        public bool ShowCostPrice { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header metadata and items for the Item Ledger report.
    /// </summary>
    public class ItemLedgerDataResult
    {
        public ItemLedgerHeader Header { get; set; }
        public List<ItemLedgerReportItem> Items { get; set; }

        public ItemLedgerDataResult(ItemLedgerHeader header, List<ItemLedgerReportItem> items)
        {
            Header = header;
            Items = items;
        }
    }

    /// <summary>
    /// Service to transform raw StockLedger DataTables and calculate progressive inventory balances.
    /// </summary>
    public static class ItemLedgerDataService
    {
        public static ItemLedgerDataResult ConvertDataTable(
            DataTable dt,
            string itemId,
            string itemTitle,
            DateTime fromDate,
            DateTime toDate,
            string movementFilter = "All",
            bool showCostPrice = false)
        {
            var rawItems = new List<ItemLedgerReportItem>();
            decimal openingBalance = 0m;
            decimal runningBalance = 0m;
            decimal runningSec = 0m;
            decimal openingSec = 0m;
            string unitLabel = null;

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    DateTime vdate = DateTime.MinValue;
                    if (row.Table.Columns.Contains("vdate") && row["vdate"] != DBNull.Value)
                    {
                        DateTime.TryParse(row["vdate"].ToString(), out vdate);
                    }

                    string vno = row.Table.Columns.Contains("vno") && row["vno"] != DBNull.Value
                        ? row["vno"].ToString()
                        : string.Empty;

                    string particular = row.Table.Columns.Contains("particular") && row["particular"] != DBNull.Value
                        ? row["particular"].ToString()
                        : string.Empty;

                    decimal qtyIn = 0m;
                    if (row.Table.Columns.Contains("qtyin") && row["qtyin"] != DBNull.Value)
                    {
                        decimal.TryParse(row["qtyin"].ToString(), out qtyIn);
                    }

                    decimal qtyOut = 0m;
                    if (row.Table.Columns.Contains("qtyout") && row["qtyout"] != DBNull.Value)
                    {
                        decimal.TryParse(row["qtyout"].ToString(), out qtyOut);
                    }

                    decimal secIn = 0m;
                    if (row.Table.Columns.Contains("secqtyin") && row["secqtyin"] != DBNull.Value)
                    {
                        decimal.TryParse(row["secqtyin"].ToString(), out secIn);
                    }

                    decimal secOut = 0m;
                    if (row.Table.Columns.Contains("secqtyout") && row["secqtyout"] != DBNull.Value)
                    {
                        decimal.TryParse(row["secqtyout"].ToString(), out secOut);
                    }

                    if (row.Table.Columns.Contains("primaryunit") && row["primaryunit"] != DBNull.Value && string.IsNullOrWhiteSpace(unitLabel))
                    {
                        unitLabel = row["primaryunit"].ToString();
                    }

                    decimal? rate = null;
                    if (row.Table.Columns.Contains("rate") && row["rate"] != DBNull.Value)
                    {
                        decimal parsedRate;
                        if (decimal.TryParse(row["rate"].ToString(), out parsedRate))
                        {
                            rate = parsedRate;
                        }
                    }

                    decimal? costPrice = null;
                    if (row.Table.Columns.Contains("costprice") && row["costprice"] != DBNull.Value)
                    {
                        decimal parsedCost;
                        if (decimal.TryParse(row["costprice"].ToString(), out parsedCost))
                        {
                            costPrice = parsedCost;
                        }
                    }

                    // Check if opening stock row
                    bool isOpening = i == 0 && (string.IsNullOrWhiteSpace(vno) || particular.IndexOf("Openning", StringComparison.OrdinalIgnoreCase) >= 0 || particular.IndexOf("Opening", StringComparison.OrdinalIgnoreCase) >= 0);

                    if (isOpening)
                    {
                        openingBalance = qtyIn - qtyOut;
                        runningBalance = openingBalance;
                        openingSec = secIn - secOut;
                        runningSec = openingSec;

                        rawItems.Add(new ItemLedgerReportItem
                        {
                            Date = vdate != DateTime.MinValue ? vdate : fromDate.AddDays(-1),
                            VoucherNo = "-",
                            Particular = "Opening Stock Balance",
                            Rate = rate,
                            CostPrice = null,
                            QtyIn = qtyIn,
                            QtyOut = qtyOut,
                            Balance = runningBalance,
                            SecQtyIn = secIn,
                            SecQtyOut = secOut,
                            SecBalance = runningSec
                        });
                    }
                    else
                    {
                        runningBalance += (qtyIn - qtyOut);
                        runningSec += (secIn - secOut);

                        rawItems.Add(new ItemLedgerReportItem
                        {
                            Date = vdate != DateTime.MinValue ? vdate : fromDate,
                            VoucherNo = vno,
                            Particular = particular,
                            Rate = rate,
                            CostPrice = costPrice,
                            QtyIn = qtyIn,
                            QtyOut = qtyOut,
                            Balance = runningBalance,
                            SecQtyIn = secIn,
                            SecQtyOut = secOut,
                            SecBalance = runningSec
                        });
                    }
                }
            }

            foreach (var it in rawItems) it.UnitLabel = unitLabel;

            // Exclude opening from movement totals
            decimal totalIn = rawItems.Where(x => x.VoucherNo != "-").Sum(x => x.QtyIn);
            decimal totalOut = rawItems.Where(x => x.VoucherNo != "-").Sum(x => x.QtyOut);

            var header = new ItemLedgerHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                ItemId = itemId ?? string.Empty,
                ItemTitle = !string.IsNullOrWhiteSpace(itemTitle) ? itemTitle : "Inventory Item",
                FromDate = fromDate,
                ToDate = toDate,
                OpeningBalance = openingBalance,
                TotalIn = totalIn,
                TotalOut = totalOut,
                ClosingBalance = runningBalance,
                OpeningSec = openingSec,
                TotalInSec = rawItems.Where(x => x.VoucherNo != "-").Sum(x => x.SecQtyIn),
                TotalOutSec = rawItems.Where(x => x.VoucherNo != "-").Sum(x => x.SecQtyOut),
                ClosingSec = runningSec,
                UnitLabel = unitLabel,
                ShowCostPrice = showCostPrice,
                GeneratedAt = DateTime.Now
            };

            // Apply movement filter
            var filteredItems = rawItems;
            if (movementFilter == "Inward")
            {
                filteredItems = rawItems.Where(x => x.VoucherNo == "-" || x.QtyIn > 0).ToList();
            }
            else if (movementFilter == "Outward")
            {
                filteredItems = rawItems.Where(x => x.VoucherNo == "-" || x.QtyOut > 0).ToList();
            }

            return new ItemLedgerDataResult(header, filteredItems);
        }
    }
}

