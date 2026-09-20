using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERP.Reporting.Models
{
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

        public string FormattedDate => Date.ToString("dd-MMM-yyyy");
        public string FormattedRate => Rate.HasValue && Rate.Value > 0 ? Rate.Value.ToString("N2") : "-";
        public string FormattedCostPrice => CostPrice.HasValue && CostPrice.Value > 0
            ? CostPrice.Value.ToString("N2")
            : (QtyIn > 0 && Rate.HasValue && Rate.Value > 0
                ? Rate.Value.ToString("N2")
                : (CostPrice.HasValue ? "0.00" : "-"));
        public string FormattedQtyIn => QtyIn > 0 ? QtyIn.ToString("N2") : "-";
        public string FormattedQtyOut => QtyOut > 0 ? QtyOut.ToString("N2") : "-";
        public string FormattedBalance => Balance.ToString("N2");
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

                        rawItems.Add(new ItemLedgerReportItem
                        {
                            Date = vdate != DateTime.MinValue ? vdate : fromDate.AddDays(-1),
                            VoucherNo = "-",
                            Particular = "Opening Stock Balance",
                            Rate = rate,
                            CostPrice = null,
                            QtyIn = qtyIn,
                            QtyOut = qtyOut,
                            Balance = runningBalance
                        });
                    }
                    else
                    {
                        runningBalance += (qtyIn - qtyOut);

                        rawItems.Add(new ItemLedgerReportItem
                        {
                            Date = vdate != DateTime.MinValue ? vdate : fromDate,
                            VoucherNo = vno,
                            Particular = particular,
                            Rate = rate,
                            CostPrice = costPrice,
                            QtyIn = qtyIn,
                            QtyOut = qtyOut,
                            Balance = runningBalance
                        });
                    }
                }
            }

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

