using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Classes;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Individual line item on a customer bill (sale transaction or invoice item).
    /// </summary>
    public class CustomerBillLineItem
    {
        public DateTime Date { get; set; }
        public string VNo { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal AddLess { get; set; }
        public decimal Amount { get; set; }

        public string FormattedDate => Date != DateTime.MinValue ? Date.ToString("dd-MMM-yyyy") : string.Empty;
        public string FormattedQty => Qty.ToString("#,##0.##");
        public string FormattedRate => Rate.ToString("#,##0.00");
        public string FormattedAddLess => AddLess != 0 ? AddLess.ToString("#,##0.00") : "-";
        public string FormattedAmount => Amount.ToString("#,##0.00");
    }

    /// <summary>
    /// Financial summary and metadata for a single customer bill statement.
    /// </summary>
    public class CustomerBillSummary
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string DateBasis { get; set; } = "VoucherDate";

        public decimal PreviousBalance { get; set; }
        public decimal CurrentBillTotal { get; set; }
        public decimal GrossTotal => PreviousBalance + CurrentBillTotal;
        public decimal PaymentsReceived { get; set; }
        public decimal NetBalance => GrossTotal - PaymentsReceived;

        public string ThankyouLine { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public string FormattedPreviousBalance => PreviousBalance.ToString("#,##0.00");
        public string FormattedCurrentBillTotal => CurrentBillTotal.ToString("#,##0.00");
        public string FormattedGrossTotal => GrossTotal.ToString("#,##0.00");
        public string FormattedPaymentsReceived => PaymentsReceived.ToString("#,##0.00");
        public string FormattedNetBalance => NetBalance.ToString("#,##0.00");
    }

    /// <summary>
    /// Combined customer bill data result containing summary statement and line items.
    /// </summary>
    public class CustomerBillDataResult
    {
        public CustomerBillSummary Summary { get; set; }
        public List<CustomerBillLineItem> Lines { get; set; }

        public CustomerBillDataResult(CustomerBillSummary summary, List<CustomerBillLineItem> lines)
        {
            Summary = summary ?? new CustomerBillSummary();
            Lines = lines ?? new List<CustomerBillLineItem>();
        }
    }

    /// <summary>
    /// Service to transform CustomerBill DataSets and produce preview data.
    /// </summary>
    public static class CustomerBillDataService
    {
        public static CustomerBillDataResult ConvertDataSet(
            DataSet ds,
            string customerCode,
            string customerTitle,
            DateTime fromDate,
            DateTime toDate,
            string dateBasis = "VoucherDate",
            string customerAddress = "",
            string customerPhone = "")
        {
            var lines = new List<CustomerBillLineItem>();
            decimal prevBalance = 0m;
            decimal payment = 0m;
            decimal netBal = 0m;

            if (ds != null && ds.Tables.Count > 0)
            {
                var dtLines = ds.Tables[0];
                if (dtLines != null)
                {
                    foreach (DataRow row in dtLines.Rows)
                    {
                        DateTime date = DateTime.MinValue;
                        if (row.Table.Columns.Contains("date") && row["date"] != DBNull.Value)
                        {
                            DateTime.TryParse(row["date"].ToString(), out date);
                        }

                        string vno = row.Table.Columns.Contains("vno") && row["vno"] != DBNull.Value ? row["vno"].ToString().Trim() : string.Empty;
                        string item = row.Table.Columns.Contains("item") && row["item"] != DBNull.Value ? row["item"].ToString().Trim() : string.Empty;
                        string unit = row.Table.Columns.Contains("unit") && row["unit"] != DBNull.Value ? row["unit"].ToString().Trim() : string.Empty;

                        decimal qty = 0m;
                        if (row.Table.Columns.Contains("qty") && row["qty"] != DBNull.Value)
                            decimal.TryParse(row["qty"].ToString(), out qty);

                        decimal rate = 0m;
                        if (row.Table.Columns.Contains("rate") && row["rate"] != DBNull.Value)
                            decimal.TryParse(row["rate"].ToString(), out rate);

                        decimal addless = 0m;
                        if (row.Table.Columns.Contains("addless") && row["addless"] != DBNull.Value)
                            decimal.TryParse(row["addless"].ToString(), out addless);

                        decimal amount = 0m;
                        if (row.Table.Columns.Contains("amount") && row["amount"] != DBNull.Value)
                            decimal.TryParse(row["amount"].ToString(), out amount);

                        lines.Add(new CustomerBillLineItem
                        {
                            Date = date,
                            VNo = vno,
                            Item = item,
                            Unit = unit,
                            Qty = qty,
                            Rate = rate,
                            AddLess = addless,
                            Amount = amount
                        });
                    }
                }

                if (ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    var sumRow = ds.Tables[1].Rows[0];
                    if (sumRow.Table.Columns.Contains("PreviousBalance") && sumRow["PreviousBalance"] != DBNull.Value)
                        decimal.TryParse(sumRow["PreviousBalance"].ToString(), out prevBalance);

                    if (sumRow.Table.Columns.Contains("Payment") && sumRow["Payment"] != DBNull.Value)
                        decimal.TryParse(sumRow["Payment"].ToString(), out payment);

                    if (sumRow.Table.Columns.Contains("Balance") && sumRow["Balance"] != DBNull.Value)
                        decimal.TryParse(sumRow["Balance"].ToString(), out netBal);
                }
            }

            decimal currentBillTotal = lines.Sum(x => x.Amount);

            var summary = new CustomerBillSummary
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                CompanyAddress = CompanyInfo.Address ?? string.Empty,
                CompanyPhone = !string.IsNullOrWhiteSpace(CompanyInfo.ContactHead) ? CompanyInfo.ContactHead : CompanyInfo.Cell,

                CustomerCode = customerCode ?? string.Empty,
                CustomerName = !string.IsNullOrWhiteSpace(customerTitle) ? customerTitle : "Valued Customer",
                CustomerAddress = customerAddress ?? string.Empty,
                CustomerPhone = customerPhone ?? string.Empty,

                FromDate = fromDate,
                ToDate = toDate,
                DateBasis = dateBasis,

                PreviousBalance = prevBalance,
                CurrentBillTotal = currentBillTotal,
                PaymentsReceived = payment,

                ThankyouLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine) ? ConfigInfo.ThankyouLine : "Thank you for your business!",
                GeneratedBy = !string.IsNullOrWhiteSpace(UserInfo.UserName) ? UserInfo.UserName : "System Operator",
                GeneratedAt = DateTime.Now
            };

            return new CustomerBillDataResult(summary, lines);
        }

        public static CustomerBillDataResult GetMockData(
            DateTime fromDate,
            DateTime toDate,
            string customerCode = "CUST-1042",
            string customerTitle = "Al-Madina Cash & Carry")
        {
            var lines = new List<CustomerBillLineItem>
            {
                new CustomerBillLineItem
                {
                    Date = fromDate.AddDays(1),
                    VNo = "SAL-10821",
                    Item = "Nestle MilkPak 1000ml (Carton of 12)",
                    Unit = "Ctn",
                    Qty = 15,
                    Rate = 3450.00m,
                    AddLess = -500.00m,
                    Amount = 51250.00m
                },
                new CustomerBillLineItem
                {
                    Date = fromDate.AddDays(2),
                    VNo = "SAL-10821",
                    Item = "Olpers Milk 1000ml (Carton of 12)",
                    Unit = "Ctn",
                    Qty = 10,
                    Rate = 3400.00m,
                    AddLess = 0.00m,
                    Amount = 34000.00m
                },
                new CustomerBillLineItem
                {
                    Date = fromDate.AddDays(3),
                    VNo = "SAL-10854",
                    Item = "Tapal Danedar Tea 950g Value Pack",
                    Unit = "Pcs",
                    Qty = 24,
                    Rate = 1420.00m,
                    AddLess = -300.00m,
                    Amount = 33780.00m
                },
                new CustomerBillLineItem
                {
                    Date = fromDate.AddDays(4),
                    VNo = "SAL-10889",
                    Item = "Dalda Cooking Oil 5 Litre Tin",
                    Unit = "Tin",
                    Qty = 8,
                    Rate = 2850.00m,
                    AddLess = 0.00m,
                    Amount = 22800.00m
                },
                new CustomerBillLineItem
                {
                    Date = fromDate.AddDays(5),
                    VNo = "SAL-10902",
                    Item = "National Recipe Masala Assorted 50g",
                    Unit = "Dzn",
                    Qty = 12,
                    Rate = 1150.00m,
                    AddLess = -200.00m,
                    Amount = 13600.00m
                },
                new CustomerBillLineItem
                {
                    Date = fromDate.AddDays(6),
                    VNo = "SAL-10925",
                    Item = "Ariel Washing Powder Original 1kg",
                    Unit = "Bags",
                    Qty = 30,
                    Rate = 680.00m,
                    AddLess = 0.00m,
                    Amount = 20400.00m
                }
            };

            decimal prevBalance = 45000.00m;
            decimal payments = 80000.00m;
            decimal currentTotal = lines.Sum(x => x.Amount); // 175,830.00

            var summary = new CustomerBillSummary
            {
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                CompanyAddress = !string.IsNullOrWhiteSpace(CompanyInfo.Address) ? CompanyInfo.Address : "Plot 42-B, Commercial Area, Lahore",
                CompanyPhone = !string.IsNullOrWhiteSpace(CompanyInfo.ContactHead) ? CompanyInfo.ContactHead : "+92 42 35789000",

                CustomerCode = customerCode,
                CustomerName = customerTitle,
                CustomerAddress = "Main Bazaar, Block 5, Gulberg III, Lahore",
                CustomerPhone = "0300-1234567",

                FromDate = fromDate,
                ToDate = toDate,
                DateBasis = "VoucherDate",

                PreviousBalance = prevBalance,
                CurrentBillTotal = currentTotal,
                PaymentsReceived = payments,

                ThankyouLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine) ? ConfigInfo.ThankyouLine : "Thank you for your valued business!",
                GeneratedBy = !string.IsNullOrWhiteSpace(UserInfo.UserName) ? UserInfo.UserName : "Administrator",
                GeneratedAt = DateTime.Now
            };

            return new CustomerBillDataResult(summary, lines);
        }
    }
}
