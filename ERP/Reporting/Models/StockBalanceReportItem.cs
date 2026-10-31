using System;
using System.Collections.Generic;
using System.Data;
using ERP.Classes;

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
        public decimal Rate { get; set; }

        public string WandaFormattedOpening => WandaItemLedgerDataService.FormatUnit(OpeningQty, Unit);
        public string WandaFormattedQtyIn => QtyIn > 0 ? WandaItemLedgerDataService.FormatUnit(QtyIn, Unit) : "-";
        public string WandaFormattedQtyOut => QtyOut > 0 ? WandaItemLedgerDataService.FormatUnit(QtyOut, Unit) : "-";
        public string WandaFormattedClosing => WandaItemLedgerDataService.FormatUnit(ClosingQty, Unit);

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
    /// Standard (non-Wanda) service to transform ReportQuery.StockBalance DataTable results into typed items.
    /// </summary>
    public static class StockBalanceDataService
    {
        public static List<StockBalanceReportItem> FromDataTable(DataTable dt)
        {
            if (ApiSession.HasVariablePackFeature)
            {
                return WandaStockBalanceDataService.FromDataTable(dt);
            }

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
                        : (decimal?)null
                };

                items.Add(item);
            }

            return items;
        }
    }

    /// <summary>
    /// Dedicated Wanda (variable pack) service to transform StockBalance DataTable results into default-unit items.
    /// </summary>
    public static class WandaStockBalanceDataService
    {
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
                        : string.Empty,
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
                        : (decimal?)null
                };

                items.Add(item);
            }

            return items;
        }
    }
}
