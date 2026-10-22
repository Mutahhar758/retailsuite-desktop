using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Classes;

namespace ERP.Reporting.Models
{
    public enum SaleReceiptPrintLayout
    {
        Thermal80mm,
        A5Sheet,
        A4Sheet
    }

    /// <summary>
    /// Individual line item on a QuestPDF sale receipt / bill.
    /// </summary>
    public class SaleReceiptLineItem
    {
        public int Seq { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal GrossRate { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
        public string Imei { get; set; }

        public string FormattedQty => Qty.ToString("#,##0.##");
        public string FormattedQtyWithUnit => !string.IsNullOrWhiteSpace(Unit)
            ? string.Format("{0:#,##0.##} {1}", Qty, Unit)
            : Qty.ToString("#,##0.##");
        public string FormattedRate => Rate.ToString("#,##0.00");
        public string FormattedDiscount => Discount > 0 ? Discount.ToString("#,##0.00") : "-";
        public string FormattedAmount => Amount.ToString("#,##0.00");
    }

    /// <summary>
    /// Financial summary and metadata for a QuestPDF sale receipt / bill.
    /// </summary>
    public class SaleReceiptSummary
    {
        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Narration { get; set; }
        public string Description { get; set; }
        public string Cashier { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string ThankyouLine { get; set; }

        public decimal GrossTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CashReceipt { get; set; }
        public decimal CashBack { get; set; }
        public decimal Balance { get; set; }
        public bool IsDuplicate { get; set; }

        public string FormattedDate => Date != DateTime.MinValue ? Date.ToString("dd-MMM-yyyy HH:mm") : DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
        public string FormattedGrossTotal => GrossTotal.ToString("#,##0.00");
        public string FormattedDiscount => TotalDiscount.ToString("#,##0.00");
        public string FormattedNetAmount => NetAmount.ToString("#,##0.00");
        public string FormattedCashReceipt => CashReceipt.ToString("#,##0.00");
        public string FormattedCashBack => CashBack.ToString("#,##0.00");
        public string FormattedBalance => Balance.ToString("#,##0.00");
    }

    /// <summary>
    /// Bundled result representing a complete sale receipt.
    /// </summary>
    public class SaleReceiptDataResult
    {
        public SaleReceiptSummary Summary { get; set; }
        public List<SaleReceiptLineItem> Lines { get; set; }

        public SaleReceiptDataResult(SaleReceiptSummary summary, List<SaleReceiptLineItem> lines)
        {
            Summary = summary ?? new SaleReceiptSummary();
            Lines = lines ?? new List<SaleReceiptLineItem>();
        }

        public static SaleReceiptDataResult FromDataSet(DataSet ds, string voucherNo, bool isDuplicate = false)
        {
            var summary = new SaleReceiptSummary
            {
                VoucherNo = voucherNo.StartsWith("SL-", StringComparison.OrdinalIgnoreCase) ? voucherNo : ("SL-" + voucherNo),
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                CompanyAddress = CompanyInfo.Address ?? "",
                CompanyPhone = CompanyInfo.ContactHead ?? "",
                ThankyouLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine) ? ConfigInfo.ThankyouLine : "Thank you for your business!",
                Cashier = UserInfo.UserName ?? "Admin",
                IsDuplicate = isDuplicate,
                Date = DateTime.Now
            };

            var lines = new List<SaleReceiptLineItem>();

            if (ds != null && ds.Tables.Count > 0)
            {
                // Table 0: Lines
                DataTable dtLines = ds.Tables[0];
                int seq = 1;
                foreach (DataRow row in dtLines.Rows)
                {
                    string itemName = dtLines.Columns.Contains("Item") ? Convert.ToString(row["Item"])
                        : (dtLines.Columns.Contains("ItemName") ? Convert.ToString(row["ItemName"]) : "Item");
                    decimal qty = dtLines.Columns.Contains("Qty") && row["Qty"] != DBNull.Value ? Convert.ToDecimal(row["Qty"]) : 0m;
                    string unit = dtLines.Columns.Contains("Unit") ? Convert.ToString(row["Unit"]) : "";
                    decimal rate = dtLines.Columns.Contains("Rate") && row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0m;
                    decimal grossRate = dtLines.Columns.Contains("GrossRate") && row["GrossRate"] != DBNull.Value ? Convert.ToDecimal(row["GrossRate"]) : rate;
                    decimal disc = dtLines.Columns.Contains("disc") && row["disc"] != DBNull.Value ? Convert.ToDecimal(row["disc"])
                        : (dtLines.Columns.Contains("Discount") && row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0m);
                    decimal amt = dtLines.Columns.Contains("Amount") && row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"])
                        : (dtLines.Columns.Contains("TAmount") && row["TAmount"] != DBNull.Value ? Convert.ToDecimal(row["TAmount"]) : (qty * rate - disc));

                    lines.Add(new SaleReceiptLineItem
                    {
                        Seq = seq++,
                        Item = itemName,
                        Qty = qty,
                        Unit = unit,
                        Rate = rate,
                        GrossRate = grossRate,
                        Discount = disc,
                        Amount = amt
                    });
                }

                // Table 1: Header
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    DataRow headerRow = ds.Tables[1].Rows[0];
                    if (ds.Tables[1].Columns.Contains("vdate") && headerRow["vdate"] != DBNull.Value)
                        summary.Date = Convert.ToDateTime(headerRow["vdate"]);
                    else if (ds.Tables[1].Columns.Contains("VDate") && headerRow["VDate"] != DBNull.Value)
                        summary.Date = Convert.ToDateTime(headerRow["VDate"]);

                    if (ds.Tables[1].Columns.Contains("Title"))
                        summary.CustomerName = Convert.ToString(headerRow["Title"]);

                    if (ds.Tables[1].Columns.Contains("descr"))
                        summary.Description = Convert.ToString(headerRow["descr"]);

                    if (ds.Tables[1].Columns.Contains("Amount") && headerRow["Amount"] != DBNull.Value)
                        summary.GrossTotal = Convert.ToDecimal(headerRow["Amount"]);
                    else
                        summary.GrossTotal = lines.Sum(x => x.Qty * x.Rate);

                    if (ds.Tables[1].Columns.Contains("Discount") && headerRow["Discount"] != DBNull.Value)
                        summary.TotalDiscount = Convert.ToDecimal(headerRow["Discount"]);
                    else
                        summary.TotalDiscount = lines.Sum(x => x.Discount);

                    if (ds.Tables[1].Columns.Contains("NetAmount") && headerRow["NetAmount"] != DBNull.Value)
                        summary.NetAmount = Convert.ToDecimal(headerRow["NetAmount"]);
                    else
                        summary.NetAmount = summary.GrossTotal - summary.TotalDiscount;

                    if (ds.Tables[1].Columns.Contains("CashReceipt") && headerRow["CashReceipt"] != DBNull.Value)
                        summary.CashReceipt = Convert.ToDecimal(headerRow["CashReceipt"]);

                    if (ds.Tables[1].Columns.Contains("CashBack") && headerRow["CashBack"] != DBNull.Value)
                        summary.CashBack = Convert.ToDecimal(headerRow["CashBack"]);

                    if (ds.Tables[1].Columns.Contains("Balance") && headerRow["Balance"] != DBNull.Value)
                        summary.Balance = Convert.ToDecimal(headerRow["Balance"]);
                }
                else
                {
                    summary.GrossTotal = lines.Sum(x => x.Qty * x.Rate);
                    summary.TotalDiscount = lines.Sum(x => x.Discount);
                    summary.NetAmount = summary.GrossTotal - summary.TotalDiscount;
                }

                // Table 2: Address
                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    if (ds.Tables[2].Columns.Contains("AccAddress"))
                        summary.CustomerAddress = Convert.ToString(ds.Tables[2].Rows[0]["AccAddress"]);
                }
            }

            return new SaleReceiptDataResult(summary, lines);
        }
    }
}
