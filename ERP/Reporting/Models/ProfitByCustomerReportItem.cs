using System;
using System.Collections.Generic;

namespace ERP.Reporting.Models
{
    public class ProfitByCustomerReportItem
    {
        public string AccountId { get; set; }
        public string AccountTitle { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit => TotalSales - TotalCost;
        public decimal GrossMarginPct => TotalSales > 0 ? Math.Round((GrossProfit / TotalSales) * 100m, 2) : 0m;
        public List<ProfitByCustomerDetailItem> Details { get; set; } = new List<ProfitByCustomerDetailItem>();
    }

    public class ProfitByCustomerDetailItem
    {
        public DateTime VDate { get; set; }
        public string VNo { get; set; }
        public string VType { get; set; }
        public string ItemId { get; set; }
        public string ItemTitle { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public decimal SaleRate { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostAmount { get; set; }
        public decimal Profit => SaleAmount - CostAmount;
        public decimal MarginPct => SaleAmount > 0 ? Math.Round((Profit / SaleAmount) * 100m, 2) : 0m;
    }

    public class ProfitByCustomerHeader
    {
        public string CompanyName { get; set; } = "RETAIL SUITE";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CustomerFilter { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit => TotalSales - TotalCost;
        public decimal GrossMarginPct => TotalSales > 0 ? Math.Round((GrossProfit / TotalSales) * 100m, 2) : 0m;
        public decimal TotalQtySold { get; set; }
        public int TotalCustomers { get; set; }
        public bool IsProfitable => GrossProfit >= 0;
    }
}
