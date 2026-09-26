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

        public decimal? SecQty { get; set; }
        public decimal? SecRate { get; set; }
        public decimal? QtyInPack { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public decimal? ReceiptAmount { get; set; }

        public string FormattedDate => Date != DateTime.MinValue ? Date.ToString("dd-MMM-yyyy") : string.Empty;
        public string FormattedQty => Qty.ToString("#,##0.##");
        public string FormattedRate => Rate.ToString("#,##0");
        public string FormattedAddLess => AddLess != 0 ? AddLess.ToString("#,##0") : "-";
        public string FormattedAmount => Amount.ToString("#,##0");
        public string FormattedSecQty => SecQty.HasValue ? SecQty.Value.ToString("#,##0.##") : "-";
        public string FormattedSecRate => SecRate.HasValue ? SecRate.Value.ToString("#,##0") : "-";
        public string FormattedReceiptDate => ReceiptDate.HasValue && ReceiptDate.Value != DateTime.MinValue ? ReceiptDate.Value.ToString("dd-MMM-yyyy") : "-";
        public string FormattedReceiptAmount => ReceiptAmount.HasValue && ReceiptAmount.Value != 0 ? ReceiptAmount.Value.ToString("#,##0") : "-";
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
        public decimal? ExplicitNetBalance { get; set; }
        public decimal NetBalance => ExplicitNetBalance ?? (GrossTotal - PaymentsReceived);

        public string ThankyouLine { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public string FormattedPreviousBalance => PreviousBalance.ToString("#,##0");
        public string FormattedCurrentBillTotal => CurrentBillTotal.ToString("#,##0");
        public string FormattedGrossTotal => GrossTotal.ToString("#,##0");
        public string FormattedPaymentsReceived => PaymentsReceived.ToString("#,##0");
        public string FormattedNetBalance => NetBalance.ToString("#,##0");

        public QrPaymentInfo QrPayment { get; set; }
        public bool ShowQrPayment => QrPayment != null && QrPayment.IsEnabled && NetBalance > 0;
        public bool IsWandaLayout { get; set; }
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

                        decimal? secQty = null;
                        if (row.Table.Columns.Contains("secqty") && row["secqty"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["secqty"].ToString(), out var sq)) secQty = sq;
                        }
                        else if (row.Table.Columns.Contains("SecQty") && row["SecQty"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["SecQty"].ToString(), out var sq)) secQty = sq;
                        }

                        decimal? secRate = null;
                        if (row.Table.Columns.Contains("secrate") && row["secrate"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["secrate"].ToString(), out var sr)) secRate = sr;
                        }
                        else if (row.Table.Columns.Contains("SecRate") && row["SecRate"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["SecRate"].ToString(), out var sr)) secRate = sr;
                        }

                        decimal? qtyInPack = null;
                        if (row.Table.Columns.Contains("qtyinpack") && row["qtyinpack"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["qtyinpack"].ToString(), out var qip)) qtyInPack = qip;
                        }
                        else if (row.Table.Columns.Contains("QtyInPack") && row["QtyInPack"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["QtyInPack"].ToString(), out var qip)) qtyInPack = qip;
                        }

                        DateTime? receiptDate = null;
                        if (row.Table.Columns.Contains("receiptdate") && row["receiptdate"] != DBNull.Value)
                        {
                            if (DateTime.TryParse(row["receiptdate"].ToString(), out var rd)) receiptDate = rd;
                        }
                        else if (row.Table.Columns.Contains("ReceiptDate") && row["ReceiptDate"] != DBNull.Value)
                        {
                            if (DateTime.TryParse(row["ReceiptDate"].ToString(), out var rd)) receiptDate = rd;
                        }

                        decimal? receiptAmount = null;
                        if (row.Table.Columns.Contains("receiptamount") && row["receiptamount"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["receiptamount"].ToString(), out var ra)) receiptAmount = ra;
                        }
                        else if (row.Table.Columns.Contains("ReceiptAmount") && row["ReceiptAmount"] != DBNull.Value)
                        {
                            if (decimal.TryParse(row["ReceiptAmount"].ToString(), out var ra)) receiptAmount = ra;
                        }

                        lines.Add(new CustomerBillLineItem
                        {
                            Date = date,
                            VNo = vno,
                            Item = item,
                            Unit = unit,
                            Qty = qty,
                            Rate = rate,
                            AddLess = addless,
                            Amount = amount,
                            SecQty = secQty,
                            SecRate = secRate,
                            QtyInPack = qtyInPack,
                            ReceiptDate = receiptDate,
                            ReceiptAmount = receiptAmount
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
                ExplicitNetBalance = netBal,

                ThankyouLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine) ? ConfigInfo.ThankyouLine : "Thank you for your business!",
                GeneratedBy = !string.IsNullOrWhiteSpace(UserInfo.UserName) ? UserInfo.UserName : "System Operator",
                GeneratedAt = DateTime.Now,
                QrPayment = QrPaymentInfo.GetCached(),
                IsWandaLayout = ApiSession.HasVariablePackFeature
            };

            return new CustomerBillDataResult(summary, lines);
        }
    }
}
