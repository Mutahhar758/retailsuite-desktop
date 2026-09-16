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
        public decimal Rate { get; set; }

        /// <summary>
        /// Total inventory valuation for this SKU (ClosingQty * Rate).
        /// </summary>
        public decimal TotalValue => ClosingQty * Rate;
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
    /// Service to transform ReportQuery.StockBalance DataTable results into typed items,
    /// and generate realistic mock data for offline/preview testing.
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
                        : 0m
                };

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Generates 15+ realistic inventory SKU records for instant offline testing and preview.
        /// </summary>
        public static StockBalanceDataResult GetSampleStockBalance(string companyName = null)
        {
            var header = new StockBalanceHeader
            {
                CompanyName = !string.IsNullOrWhiteSpace(companyName)
                    ? companyName
                    : (!string.IsNullOrWhiteSpace(ERP.CompanyInfo.CompanyName) ? ERP.CompanyInfo.CompanyName : "Retail Suite Enterprise"),
                CategoryName = "All Categories",
                Filter = "All Stock",
                FromDate = DateTime.Today.AddDays(-30),
                ToDate = DateTime.Today
            };

            var items = new List<StockBalanceReportItem>();
            int idx = 1;

            void AddSku(string name, string unit, decimal pri, decimal qIn, decimal qOut, decimal rate)
            {
                decimal bal = pri + qIn - qOut;
                items.Add(new StockBalanceReportItem
                {
                    Index = idx++,
                    ItemName = name,
                    Unit = unit,
                    OpeningQty = pri,
                    QtyIn = qIn,
                    QtyOut = qOut,
                    ClosingQty = bal,
                    Rate = rate
                });
            }

            AddSku("Coca Cola Regular 1.5L PET", "Bottle", 120, 300, 260, 165.00m);
            AddSku("Nestle Pure Life Mineral Water 500ml", "Carton", 45, 120, 95, 780.00m);
            AddSku("Lays Masala Potato Chips 50g", "Box", 60, 150, 135, 1200.00m);
            AddSku("Olper's Full Cream Milk 1L UHT", "Carton", 80, 200, 190, 3250.00m);
            AddSku("Tapal Danedar Black Tea 450g Pack", "Pack", 95, 160, 140, 680.00m);
            AddSku("Shan Special Biryani Masala 50g", "Dozen", 35, 80, 65, 960.00m);
            AddSku("Dalda Fortified Cooking Oil 5L Pouch", "Tin", 25, 60, 52, 2850.00m);
            AddSku("Guard Supreme Basmati Rice 5kg Bag", "Bag", 40, 100, 85, 1750.00m);
            AddSku("National Tomato Ketchup Dispenser 800g", "Bottle", 50, 90, 75, 420.00m);
            AddSku("Surf Excel Quick Wash Detergent 1kg", "Pack", 70, 140, 125, 610.00m);
            AddSku("Lux Rose & Vitamin E Beauty Soap 140g", "Dozen", 55, 110, 95, 1450.00m);
            AddSku("Colgate Total Dental Cavity Protection 140g", "Box", 48, 96, 80, 290.00m);
            AddSku("Head & Shoulders Smooth & Silky Shampoo 360ml", "Bottle", 30, 75, 62, 850.00m);
            AddSku("Dettol Original Antiseptic Liquid 250ml", "Bottle", 42, 85, 70, 495.00m);
            AddSku("Ariel Complete Floral Fragrance 2kg", "Bag", 28, 65, 54, 1180.00m);
            AddSku("Peak Freans Sooper Biscuit Family Pack 112g", "Carton", 65, 180, 160, 1400.00m);

            header.TotalItems = items.Count;
            header.TotalOpeningQty = 0m;
            header.TotalQtyIn = 0m;
            header.TotalQtyOut = 0m;
            header.TotalClosingQty = 0m;
            header.TotalStockValue = 0m;

            foreach (var sku in items)
            {
                header.TotalOpeningQty += sku.OpeningQty;
                header.TotalQtyIn += sku.QtyIn;
                header.TotalQtyOut += sku.QtyOut;
                header.TotalClosingQty += sku.ClosingQty;
                header.TotalStockValue += sku.TotalValue;
            }

            return new StockBalanceDataResult(header, items);
        }
    }
}
