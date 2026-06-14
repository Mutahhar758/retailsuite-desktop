using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class ReportsApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/reports";

        public async Task<DataTable> GetAccountStatementAsync(string account, DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "/account-statement?account=" + Uri.EscapeDataString(account ?? string.Empty)
                + "&fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<AccountStatementLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<AccountStatementLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("vdate", typeof(DateTime));
                dt.Columns.Add("vno", typeof(string));
                dt.Columns.Add("vseq", typeof(int));
                dt.Columns.Add("particular", typeof(string));
                dt.Columns.Add("dr", typeof(decimal));
                dt.Columns.Add("cr", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(
                        rows[i].VDate,
                        string.IsNullOrWhiteSpace(rows[i].VNo) ? (object)DBNull.Value : rows[i].VNo,
                        rows[i].VSeq,
                        rows[i].Particular ?? string.Empty,
                        rows[i].Dr,
                        rows[i].Cr);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetAccountStatementWithDueAsync(string account, DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "/account-statement-with-due?account=" + Uri.EscapeDataString(account ?? string.Empty)
                + "&fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<AccountStatementWithDueLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<AccountStatementWithDueLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("rid", typeof(int));
                dt.Columns.Add("vdate", typeof(DateTime));
                dt.Columns.Add("vno", typeof(string));
                dt.Columns.Add("vseq", typeof(int));
                dt.Columns.Add("particular", typeof(string));
                dt.Columns.Add("dr", typeof(decimal));
                dt.Columns.Add("cr", typeof(decimal));
                dt.Columns.Add("duedays", typeof(int));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(
                        i + 1,
                        rows[i].VDate,
                        string.IsNullOrWhiteSpace(rows[i].VNo) ? (object)DBNull.Value : rows[i].VNo,
                        rows[i].VSeq,
                        rows[i].Particular ?? string.Empty,
                        rows[i].Dr,
                        rows[i].Cr,
                        rows[i].DueDays.HasValue ? (object)rows[i].DueDays.Value : DBNull.Value);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetBalanceDetailAsync(string account, DateTime toDate)
        {
            var url = Endpoint
                + "/balance-detail?account=" + Uri.EscapeDataString(account ?? string.Empty)
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<BalanceDetailLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<BalanceDetailLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("account", typeof(string));
                dt.Columns.Add("balance", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(rows[i].Account ?? string.Empty, rows[i].Balance);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetTrialBalanceAsync(DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "/trial-balance?fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<TrialBalanceLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<TrialBalanceLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("lvl1", typeof(string));
                dt.Columns.Add("lvl2", typeof(string));
                dt.Columns.Add("lvl3", typeof(string));
                dt.Columns.Add("lvl4", typeof(string));
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("pribal", typeof(decimal));
                dt.Columns.Add("dr", typeof(decimal));
                dt.Columns.Add("cr", typeof(decimal));
                dt.Columns.Add("curbal", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(
                        rows[i].Lvl1 ?? string.Empty,
                        rows[i].Lvl2 ?? string.Empty,
                        rows[i].Lvl3 ?? string.Empty,
                        rows[i].Lvl4 ?? string.Empty,
                        rows[i].Title ?? string.Empty,
                        rows[i].PriBal,
                        rows[i].Dr,
                        rows[i].Cr,
                        rows[i].CurBal);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetStockLedgerAsync(string itemId, DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "/stock-ledger?fkItem=" + Uri.EscapeDataString(itemId ?? string.Empty)
                + "&fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<StockLedgerLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<StockLedgerLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("vdate", typeof(DateTime));
                dt.Columns.Add("vno", typeof(string));
                dt.Columns.Add("particular", typeof(string));
                dt.Columns.Add("qtyin", typeof(decimal));
                dt.Columns.Add("qtyout", typeof(decimal));
                dt.Columns.Add("rate", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(
                        rows[i].Vdate,
                        string.IsNullOrWhiteSpace(rows[i].Vno) ? (object)DBNull.Value : rows[i].Vno,
                        rows[i].Particular ?? string.Empty,
                        rows[i].QtyIn,
                        rows[i].QtyOut,
                        rows[i].Rate.HasValue ? (object)rows[i].Rate.Value : DBNull.Value);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetStockBalanceAsync(DateTime fromDate, DateTime toDate, string filter, decimal qty, string catagory, string type)
        {
            var url = Endpoint
                + "/stock-balance?fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"))
                + "&filter=" + Uri.EscapeDataString(filter ?? "All")
                + "&qty=" + Uri.EscapeDataString(qty.ToString(System.Globalization.CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(catagory))
                url += "&catagory=" + Uri.EscapeDataString(catagory);

            if (!string.IsNullOrWhiteSpace(type))
                url += "&type=" + Uri.EscapeDataString(type);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<StockBalanceLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<StockBalanceLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("unit", typeof(string));
                dt.Columns.Add("priqty", typeof(decimal));
                dt.Columns.Add("qty", typeof(decimal));
                dt.Columns.Add("qtyin", typeof(decimal));
                dt.Columns.Add("qtyout", typeof(decimal));
                dt.Columns.Add("qtybal", typeof(decimal));
                dt.Columns.Add("rate", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(rows[i].Item ?? string.Empty, rows[i].Unit ?? string.Empty, rows[i].PriQty, rows[i].Qty, rows[i].QtyIn, rows[i].QtyOut, rows[i].QtyBal, rows[i].Rate);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetBalanceSheetAsync(DateTime toDate)
        {
            var url = Endpoint
                + "/balance-sheet?toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<BalanceSheetLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<BalanceSheetLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("lvl1", typeof(string));
                dt.Columns.Add("lvl2", typeof(string));
                dt.Columns.Add("lvl3", typeof(string));
                dt.Columns.Add("lvl4", typeof(string));
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("pribal", typeof(decimal));
                dt.Columns.Add("drcr", typeof(decimal));
                dt.Columns.Add("curbal", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(
                        rows[i].Lvl1 ?? string.Empty,
                        rows[i].Lvl2 ?? string.Empty,
                        rows[i].Lvl3 ?? string.Empty,
                        rows[i].Lvl4 ?? string.Empty,
                        rows[i].Title ?? string.Empty,
                        rows[i].PriBal,
                        rows[i].DrCr,
                        rows[i].CurBal);
                }

                return dt;
            }
        }

        public async Task<DataTable> GetIncomeSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "/income-summary?fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<IncomeSummaryLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<IncomeSummaryLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("vtype", typeof(string));
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("dr", typeof(decimal));
                dt.Columns.Add("cr", typeof(decimal));
                dt.Columns.Add("bal", typeof(decimal));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(rows[i].VType ?? string.Empty, rows[i].Title ?? string.Empty, rows[i].Dr, rows[i].Cr, rows[i].Bal);
                }

                return dt;
            }
        }

        public async Task<DataSet> GetCustomerBillAsync(string account, DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "/customer-bill?account=" + Uri.EscapeDataString(account ?? string.Empty)
                + "&fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<CustomerBillDto>>(json);
                var body = payload != null && payload.Body != null ? payload.Body : new CustomerBillDto();

                var ds = new DataSet();

                var lines = new DataTable();
                lines.Columns.Add("date", typeof(DateTime));
                lines.Columns.Add("vno", typeof(string));
                lines.Columns.Add("item", typeof(string));
                lines.Columns.Add("unit", typeof(string));
                lines.Columns.Add("qty", typeof(decimal));
                lines.Columns.Add("rate", typeof(decimal));
                lines.Columns.Add("addless", typeof(decimal));
                lines.Columns.Add("amount", typeof(decimal));

                for (int i = 0; i < body.Lines.Count; i++)
                {
                    var row = body.Lines[i];
                    lines.Rows.Add(row.Date, row.VNo ?? string.Empty, row.Item ?? string.Empty, row.Unit ?? string.Empty, row.Qty, row.Rate, row.AddLess, row.Amount);
                }

                var summary = new DataTable();
                summary.Columns.Add("PreviousBalance", typeof(decimal));
                summary.Columns.Add("Payment", typeof(decimal));
                summary.Columns.Add("Balance", typeof(decimal));
                summary.Rows.Add(body.Summary.PreviousBalance, body.Summary.Payment, body.Summary.Balance);

                ds.Tables.Add(lines);
                ds.Tables.Add(summary);
                return ds;
            }
        }

        public async Task<DataTable> GetEnvelopeAsync(string[] accounts)
        {
            var joined = accounts == null ? string.Empty : string.Join(",", accounts);
            var url = Endpoint + "/envelope?accounts=" + Uri.EscapeDataString(joined);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<EnvelopeLineDto>>>(json);
                var rows = payload != null && payload.Body != null ? payload.Body : new List<EnvelopeLineDto>();

                var dt = new DataTable();
                dt.Columns.Add("CustomerName", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("Cell", typeof(string));
                dt.Columns.Add("CompanyName", typeof(string));

                for (int i = 0; i < rows.Count; i++)
                {
                    dt.Rows.Add(rows[i].CustomerName ?? string.Empty, rows[i].Address ?? string.Empty, rows[i].Cell ?? string.Empty, rows[i].CompanyName ?? string.Empty);
                }

                return dt;
            }
        }

        public async Task<DataSet> GetSaleBillAsync(string voucherNo)
        {
            var url = Endpoint + "/sale-bill?voucherNo=" + Uri.EscapeDataString(voucherNo ?? string.Empty);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SaleBillDto>>(json);
                var body = payload != null && payload.Body != null ? payload.Body : new SaleBillDto();

                var ds = new DataSet();

                var lines = new DataTable();
                lines.Columns.Add("ItemName", typeof(string));
                lines.Columns.Add("Qty", typeof(decimal));
                lines.Columns.Add("Unit", typeof(string));
                lines.Columns.Add("Rate", typeof(decimal));
                lines.Columns.Add("GrossRate", typeof(decimal));
                lines.Columns.Add("disc", typeof(decimal));
                lines.Columns.Add("TAmount", typeof(decimal));

                for (int i = 0; i < body.Lines.Count; i++)
                {
                    var r = body.Lines[i];
                    lines.Rows.Add(r.ItemName ?? string.Empty, r.Qty, r.Unit ?? string.Empty, r.Rate, r.GrossRate, r.Disc, r.TAmount);
                }

                var header = new DataTable();
                header.Columns.Add("vdate", typeof(DateTime));
                header.Columns.Add("Title", typeof(string));
                header.Columns.Add("Amount", typeof(decimal));
                header.Columns.Add("Discount", typeof(decimal));
                header.Columns.Add("NetAmount", typeof(decimal));
                header.Columns.Add("CashReceipt", typeof(decimal));
                header.Columns.Add("CashBack", typeof(decimal));
                header.Columns.Add("descr", typeof(string));
                header.Columns.Add("Balance", typeof(decimal));
                header.Rows.Add(body.Header.VDate, body.Header.Title ?? string.Empty, body.Header.Amount, body.Header.Discount, body.Header.NetAmount, body.Header.CashReceipt, body.Header.CashBack, body.Header.Descr ?? string.Empty, body.Header.Balance);

                var address = new DataTable();
                address.Columns.Add("AccAddress", typeof(string));
                address.Rows.Add(body.AccAddress ?? string.Empty);

                ds.Tables.Add(lines);
                ds.Tables.Add(header);
                ds.Tables.Add(address);
                return ds;
            }
        }

        public async Task<DataSet> GetSaleBill2Async(string voucherNo)
        {
            var url = Endpoint + "/sale-bill?voucherNo=" + Uri.EscapeDataString(voucherNo ?? string.Empty);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SaleBillDto>>(json);
                var body = payload != null && payload.Body != null ? payload.Body : new SaleBillDto();

                var ds = new DataSet();

                var lines = new DataTable();
                lines.Columns.Add("Item", typeof(string));
                lines.Columns.Add("Qty", typeof(decimal));
                lines.Columns.Add("Unit", typeof(string));
                lines.Columns.Add("Rate", typeof(decimal));
                lines.Columns.Add("GrossRate", typeof(decimal));
                lines.Columns.Add("disc", typeof(decimal));
                lines.Columns.Add("Amount", typeof(decimal));

                for (int i = 0; i < body.Lines.Count; i++)
                {
                    var r = body.Lines[i];
                    lines.Rows.Add(r.ItemName ?? string.Empty, r.Qty, r.Unit ?? string.Empty, r.Rate, r.GrossRate, r.Disc, r.TAmount);
                }

                var header = new DataTable();
                header.Columns.Add("vdate", typeof(DateTime));
                header.Columns.Add("Title", typeof(string));
                header.Columns.Add("Amount", typeof(decimal));
                header.Columns.Add("Discount", typeof(decimal));
                header.Columns.Add("NetAmount", typeof(decimal));
                header.Columns.Add("CashReceipt", typeof(decimal));
                header.Columns.Add("CashBack", typeof(decimal));
                header.Columns.Add("descr", typeof(string));
                header.Columns.Add("Balance", typeof(decimal));
                header.Rows.Add(body.Header.VDate, body.Header.Title ?? string.Empty, body.Header.Amount, body.Header.Discount, body.Header.NetAmount, body.Header.CashReceipt, body.Header.CashBack, body.Header.Descr ?? string.Empty, body.Header.Balance);

                var address = new DataTable();
                address.Columns.Add("AccAddress", typeof(string));
                address.Rows.Add(body.AccAddress ?? string.Empty);

                ds.Tables.Add(lines);
                ds.Tables.Add(header);
                ds.Tables.Add(address);
                return ds;
            }
        }

        public async Task<DataSet> GetPurchaseBillAsync(string voucherNo)
        {
            var url = Endpoint + "/purchase-bill?voucherNo=" + Uri.EscapeDataString(voucherNo ?? string.Empty);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PurchaseBillDto>>(json);
                var body = payload != null && payload.Body != null ? payload.Body : new PurchaseBillDto();

                var ds = new DataSet();

                var lines = new DataTable();
                lines.Columns.Add("ItemName", typeof(string));
                lines.Columns.Add("Qty", typeof(decimal));
                lines.Columns.Add("QtyInPack", typeof(decimal));
                lines.Columns.Add("Unit", typeof(string));
                lines.Columns.Add("Rate", typeof(decimal));
                lines.Columns.Add("TAmount", typeof(decimal));

                for (int i = 0; i < body.Lines.Count; i++)
                {
                    var r = body.Lines[i];
                    lines.Rows.Add(r.ItemName ?? string.Empty, r.Qty, r.QtyInPack, r.Unit ?? string.Empty, r.Rate, r.TAmount);
                }

                var header = new DataTable();
                header.Columns.Add("vdate", typeof(DateTime));
                header.Columns.Add("Title", typeof(string));
                header.Columns.Add("Amount", typeof(decimal));
                header.Columns.Add("Discount", typeof(decimal));
                header.Columns.Add("NetAmount", typeof(decimal));
                header.Rows.Add(body.Header.VDate, body.Header.Title ?? string.Empty, body.Header.Amount, body.Header.Discount, body.Header.NetAmount);

                var address = new DataTable();
                address.Columns.Add("AccAddress", typeof(string));
                address.Rows.Add(body.AccAddress ?? string.Empty);

                ds.Tables.Add(lines);
                ds.Tables.Add(header);
                ds.Tables.Add(address);
                return ds;
            }
        }

        public async Task<DataSet> GetPurchaseRetBillAsync(string voucherNo)
        {
            var url = Endpoint + "/purchase-ret-bill?voucherNo=" + Uri.EscapeDataString(voucherNo ?? string.Empty);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PurchaseBillDto>>(json);
                var body = payload != null && payload.Body != null ? payload.Body : new PurchaseBillDto();

                var ds = new DataSet();

                var lines = new DataTable();
                lines.Columns.Add("ItemName", typeof(string));
                lines.Columns.Add("Qty", typeof(decimal));
                lines.Columns.Add("QtyInPack", typeof(decimal));
                lines.Columns.Add("Unit", typeof(string));
                lines.Columns.Add("Rate", typeof(decimal));
                lines.Columns.Add("TAmount", typeof(decimal));

                for (int i = 0; i < body.Lines.Count; i++)
                {
                    var r = body.Lines[i];
                    lines.Rows.Add(r.ItemName ?? string.Empty, r.Qty, r.QtyInPack, r.Unit ?? string.Empty, r.Rate, r.TAmount);
                }

                var header = new DataTable();
                header.Columns.Add("vdate", typeof(DateTime));
                header.Columns.Add("Title", typeof(string));
                header.Columns.Add("Amount", typeof(decimal));
                header.Columns.Add("Discount", typeof(decimal));
                header.Columns.Add("NetAmount", typeof(decimal));
                header.Rows.Add(body.Header.VDate, body.Header.Title ?? string.Empty, body.Header.Amount, body.Header.Discount, body.Header.NetAmount);

                var address = new DataTable();
                address.Columns.Add("AccAddress", typeof(string));
                address.Rows.Add(body.AccAddress ?? string.Empty);

                ds.Tables.Add(lines);
                ds.Tables.Add(header);
                ds.Tables.Add(address);
                return ds;
            }
        }

        public async Task<DataSet> GetSaleRetBillAsync(string voucherNo)
        {
            var url = Endpoint + "/sale-ret-bill?voucherNo=" + Uri.EscapeDataString(voucherNo ?? string.Empty);

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SaleRetBillDto>>(json);
                var body = payload != null && payload.Body != null ? payload.Body : new SaleRetBillDto();

                var ds = new DataSet();

                var lines = new DataTable();
                lines.Columns.Add("ItemName", typeof(string));
                lines.Columns.Add("Qty", typeof(decimal));
                lines.Columns.Add("QtyInPack", typeof(decimal));
                lines.Columns.Add("Unit", typeof(string));
                lines.Columns.Add("Rate", typeof(decimal));
                lines.Columns.Add("GrossRate", typeof(decimal));
                lines.Columns.Add("TAmount", typeof(decimal));

                for (int i = 0; i < body.Lines.Count; i++)
                {
                    var r = body.Lines[i];
                    lines.Rows.Add(r.ItemName ?? string.Empty, r.Qty, r.QtyInPack, r.Unit ?? string.Empty, r.Rate, r.GrossRate, r.TAmount);
                }

                var header = new DataTable();
                header.Columns.Add("vdate", typeof(DateTime));
                header.Columns.Add("Title", typeof(string));
                header.Columns.Add("Amount", typeof(decimal));
                header.Columns.Add("Discount", typeof(decimal));
                header.Columns.Add("NetAmount", typeof(decimal));
                header.Columns.Add("CashReceipt", typeof(decimal));
                header.Columns.Add("CashBack", typeof(decimal));
                header.Columns.Add("descr", typeof(string));
                header.Columns.Add("Balance", typeof(decimal));
                header.Rows.Add(body.Header.VDate, body.Header.Title ?? string.Empty, body.Header.Amount, body.Header.Discount, body.Header.NetAmount, body.Header.CashReceipt, body.Header.CashBack, body.Header.Descr ?? string.Empty, body.Header.Balance);

                var address = new DataTable();
                address.Columns.Add("AccAddress", typeof(string));
                address.Rows.Add(body.AccAddress ?? string.Empty);

                ds.Tables.Add(lines);
                ds.Tables.Add(header);
                ds.Tables.Add(address);
                return ds;
            }
        }
    }

    internal class AccountStatementLineDto
    {
        [JsonProperty("vDate")]
        public DateTime VDate { get; set; }

        [JsonProperty("vNo")]
        public string VNo { get; set; }

        [JsonProperty("vSeq")]
        public int VSeq { get; set; }

        [JsonProperty("particular")]
        public string Particular { get; set; }

        [JsonProperty("dr")]
        public decimal Dr { get; set; }

        [JsonProperty("cr")]
        public decimal Cr { get; set; }
    }

    internal class AccountStatementWithDueLineDto
    {
        [JsonProperty("vDate")]
        public DateTime VDate { get; set; }

        [JsonProperty("vNo")]
        public string VNo { get; set; }

        [JsonProperty("vSeq")]
        public int VSeq { get; set; }

        [JsonProperty("particular")]
        public string Particular { get; set; }

        [JsonProperty("dr")]
        public decimal Dr { get; set; }

        [JsonProperty("cr")]
        public decimal Cr { get; set; }

        [JsonProperty("dueDays")]
        public int? DueDays { get; set; }
    }

    internal class BalanceDetailLineDto
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class TrialBalanceLineDto
    {
        [JsonProperty("lvl1")]
        public string Lvl1 { get; set; }

        [JsonProperty("lvl2")]
        public string Lvl2 { get; set; }

        [JsonProperty("lvl3")]
        public string Lvl3 { get; set; }

        [JsonProperty("lvl4")]
        public string Lvl4 { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("priBal")]
        public decimal PriBal { get; set; }

        [JsonProperty("dr")]
        public decimal Dr { get; set; }

        [JsonProperty("cr")]
        public decimal Cr { get; set; }

        [JsonProperty("curBal")]
        public decimal CurBal { get; set; }
    }

    internal class StockLedgerLineDto
    {
        [JsonProperty("vdate")]
        public DateTime Vdate { get; set; }

        [JsonProperty("vno")]
        public string Vno { get; set; }

        [JsonProperty("particular")]
        public string Particular { get; set; }

        [JsonProperty("qtyIn")]
        public decimal QtyIn { get; set; }

        [JsonProperty("qtyOut")]
        public decimal QtyOut { get; set; }

        [JsonProperty("rate")]
        public decimal? Rate { get; set; }
    }

    internal class StockBalanceLineDto
    {
        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("priQty")]
        public decimal PriQty { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("qtyIn")]
        public decimal QtyIn { get; set; }

        [JsonProperty("qtyOut")]
        public decimal QtyOut { get; set; }

        [JsonProperty("qtyBal")]
        public decimal QtyBal { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }
    }

    internal class BalanceSheetLineDto
    {
        [JsonProperty("lvl1")]
        public string Lvl1 { get; set; }

        [JsonProperty("lvl2")]
        public string Lvl2 { get; set; }

        [JsonProperty("lvl3")]
        public string Lvl3 { get; set; }

        [JsonProperty("lvl4")]
        public string Lvl4 { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("priBal")]
        public decimal PriBal { get; set; }

        [JsonProperty("drCr")]
        public decimal DrCr { get; set; }

        [JsonProperty("curBal")]
        public decimal CurBal { get; set; }
    }

    internal class IncomeSummaryLineDto
    {
        [JsonProperty("vType")]
        public string VType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("dr")]
        public decimal Dr { get; set; }

        [JsonProperty("cr")]
        public decimal Cr { get; set; }

        [JsonProperty("bal")]
        public decimal Bal { get; set; }
    }

    internal class CustomerBillDto
    {
        [JsonProperty("lines")]
        public List<CustomerBillLineDto> Lines { get; set; } = new List<CustomerBillLineDto>();

        [JsonProperty("summary")]
        public CustomerBillSummaryDto Summary { get; set; } = new CustomerBillSummaryDto();
    }

    internal class CustomerBillLineDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("vNo")]
        public string VNo { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("unitTitle")]
        public string Unit { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("addLess")]
        public decimal AddLess { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    internal class CustomerBillSummaryDto
    {
        [JsonProperty("previousBalance")]
        public decimal PreviousBalance { get; set; }

        [JsonProperty("payment")]
        public decimal Payment { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class EnvelopeLineDto
    {
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("cell")]
        public string Cell { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
    }

    internal class SaleBillDto
    {
        [JsonProperty("lines")]
        public List<SaleBillLineDto> Lines { get; set; } = new List<SaleBillLineDto>();

        [JsonProperty("header")]
        public SaleBillHeaderDto Header { get; set; } = new SaleBillHeaderDto();

        [JsonProperty("accAddress")]
        public string AccAddress { get; set; }
    }

    internal class SaleBillLineDto
    {

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("unitTitle")]
        public string Unit { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("grossRate")]
        public decimal GrossRate { get; set; }

        [JsonProperty("disc")]
        public decimal Disc { get; set; }

        [JsonProperty("tAmount")]
        public decimal TAmount { get; set; }
    }

    internal class SaleBillHeaderDto
    {
        [JsonProperty("vDate")]
        public DateTime VDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("netAmount")]
        public decimal NetAmount { get; set; }

        [JsonProperty("cashReceipt")]
        public decimal CashReceipt { get; set; }

        [JsonProperty("cashBack")]
        public decimal CashBack { get; set; }

        [JsonProperty("descr")]
        public string Descr { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class SaleRetBillDto
    {
        [JsonProperty("lines")]
        public List<SaleRetBillLineDto> Lines { get; set; } = new List<SaleRetBillLineDto>();

        [JsonProperty("header")]
        public SaleRetBillHeaderDto Header { get; set; } = new SaleRetBillHeaderDto();

        [JsonProperty("accAddress")]
        public string AccAddress { get; set; }
    }

    internal class SaleRetBillLineDto
    {

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal QtyInPack { get; set; }

        [JsonProperty("unitTitle")]
        public string Unit { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("grossRate")]
        public decimal GrossRate { get; set; }

        [JsonProperty("tAmount")]
        public decimal TAmount { get; set; }
    }

    internal class SaleRetBillHeaderDto
    {
        [JsonProperty("vDate")]
        public DateTime VDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("netAmount")]
        public decimal NetAmount { get; set; }

        [JsonProperty("cashReceipt")]
        public decimal CashReceipt { get; set; }

        [JsonProperty("cashBack")]
        public decimal CashBack { get; set; }

        [JsonProperty("descr")]
        public string Descr { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class PurchaseBillDto
    {
        [JsonProperty("lines")]
        public List<PurchaseBillLineDto> Lines { get; set; } = new List<PurchaseBillLineDto>();

        [JsonProperty("header")]
        public PurchaseBillHeaderDto Header { get; set; } = new PurchaseBillHeaderDto();

        [JsonProperty("accAddress")]
        public string AccAddress { get; set; }
    }

    internal class PurchaseBillLineDto
    {

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal QtyInPack { get; set; }

        [JsonProperty("unitTitle")]
        public string Unit { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("tAmount")]
        public decimal TAmount { get; set; }
    }

    internal class PurchaseBillHeaderDto
    {
        [JsonProperty("vDate")]
        public DateTime VDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("netAmount")]
        public decimal NetAmount { get; set; }
    }
}
