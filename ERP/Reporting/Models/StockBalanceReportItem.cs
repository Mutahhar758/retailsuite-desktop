using System;
using System.Collections.Generic;
using System.Data;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an inventory item line in the Stock Balance report.
    /// </summary>
    public class StockBalanceReportItem
    {
        public int Index { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal OpeningQty { get; set; }
        public decimal QtyIn { get; set; }
        public decimal QtyOut { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal SecOpeningQty { get; set; }
        public decimal SecQtyIn { get; set; }
        public decimal SecQtyOut { get; set; }
        public decimal SecClosingQty { get; set; }
        public decimal Rate { get; set; }

        public string FormattedOpening => ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(SecOpeningQty, OpeningQty, Unit) : OpeningQty.ToString("#,##0.##");
        public string FormattedQtyIn => QtyIn > 0 ? (ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(SecQtyIn, QtyIn, Unit) : QtyIn.ToString("#,##0.##")) : "-";
        public string FormattedQtyOut => QtyOut > 0 ? (ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(SecQtyOut, QtyOut, Unit) : QtyOut.ToString("#,##0.##")) : "-";
        public string FormattedClosing => ItemLedgerFormat.UseBags ? ItemLedgerFormat.BagsKg(SecClosingQty, ClosingQty, Unit) : ClosingQty.ToString("#,##0.##");

        /// <summary>
        /// Total inventory valuation for this SKU (ClosingQty * Rate).
        /// </summary>
        public decimal TotalValue => ClosingQty * Rate;
        public decimal? LowStockQty { get; set; }
        public string Status { get; set; } = "In Stock";
    }

    /// <summary>
    /// Holds summary metrics and filter metadata for the Stock Balance report.
    /// </summary>
    public class StockBalanceHeader
    {
        public string CompanyName { get; set; }
        public string CategoryName { get; set; } = "All Categories";
        public string Filter { get; set; } = "All Stock";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalOpeningQty { get; set; }
        public decimal TotalQtyIn { get; set; }
        public decimal TotalQtyOut { get; set; }
        public decimal TotalClosingQty { get; set; }
        public decimal TotalStockValue { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Container combining header metadata and items for the Stock Balance report.
    /// </summary>
    public class StockBalanceDataResult
    {
        public StockBalanceHeader Header { get; set; }
        public List<StockBalanceReportItem> Items { get; set; }

        public StockBalanceDataResult(StockBalanceHeader header, List<StockBalanceReportItem> items)
        {
            Header = header;
            Items = items;
        }
    }

    /// <summary>
    /// Service to transform ReportQuery.StockBalance DataTable results into typed items.
    /// </summary>
    public static class StockBalanceDataService
    {
        /// <summary>
        /// Converts a DataTable returned from ReportQuery.StockBalance into typed stock items.
        /// </summary>
        public static List<StockBalanceReportItem> FromDataTable(DataTable dt)
        {
            var items = new List<StockBalanceReportItem>();
            if (dt == null) return items;

            int index = 1;
            foreach (DataRow row in dt.Rows)
            {
                var item = new StockBalanceReportItem
                {
                    Index = index++,
                    ItemName = row.Table.Columns.Contains("item") && row["item"] != DBNull.Value
                        ? row["item"].ToString()
                        : string.Empty,
                    Unit = row.Table.Columns.Contains("unit") && row["unit"] != DBNull.Value
                        ? row["unit"].ToString()
                        : "Pcs",
                    OpeningQty = row.Table.Columns.Contains("priqty") && row["priqty"] != DBNull.Value
                        ? Convert.ToDecimal(row["priqty"])
                        : 0m,
                    QtyIn = row.Table.Columns.Contains("qtyin") && row["qtyin"] != DBNull.Value
                        ? Convert.ToDecimal(row["qtyin"])
                        : 0m,
                    QtyOut = row.Table.Columns.Contains("qtyout") && row["qtyout"] != DBNull.Value
                        ? Convert.ToDecimal(row["qtyout"])
                        : 0m,
                    ClosingQty = row.Table.Columns.Contains("qtybal") && row["qtybal"] != DBNull.Value
                        ? Convert.ToDecimal(row["qtybal"])
                        : 0m,
                    Rate = row.Table.Columns.Contains("rate") && row["rate"] != DBNull.Value
                        ? Convert.ToDecimal(row["rate"])
                        : 0m,
                    Status = row.Table.Columns.Contains("status") && row["status"] != DBNull.Value
                        ? row["status"].ToString()
                        : "In Stock",
                    LowStockQty = row.Table.Columns.Contains("lowstockqty") && row["lowstockqty"] != DBNull.Value
                        ? Convert.ToDecimal(row["lowstockqty"])
                        : (decimal?)null,
                    SecOpeningQty = row.Table.Columns.Contains("secpriqty") && row["secpriqty"] != DBNull.Value
                        ? Convert.ToDecimal(row["secpriqty"])
                        : 0m,
                    SecQtyIn = row.Table.Columns.Contains("secqtyin") && row["secqtyin"] != DBNull.Value
                        ? Convert.ToDecimal(row["secqtyin"])
                        : 0m,
                    SecQtyOut = row.Table.Columns.Contains("secqtyout") && row["secqtyout"] != DBNull.Value
                        ? Convert.ToDecimal(row["secqtyout"])
                        : 0m,
                    SecClosingQty = row.Table.Columns.Contains("secqtybal") && row["secqtybal"] != DBNull.Value
                        ? Convert.ToDecimal(row["secqtybal"])
                        : 0m
                };

                items.Add(item);
            }

            return items;
        }
    }
}
