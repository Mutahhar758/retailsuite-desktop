using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class ReceiptApiService : ApiServiceBase
    {
        private const string ReceiptEndpoint = "/api/receipts";

        /// <summary>Returns a single server-side page of receipt vouchers.</summary>
        public async Task<ReceiptPageResult> GetPagedListAsync(
            string fromDate = "", string toDate = "",
            string cashBank = "", string account = "", string narration = "",
            int pageNumber = 1, int pageSize = 20)
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(cashBank)) qs.Add("cashBankAccount=" + Uri.EscapeDataString(cashBank));
            if (!string.IsNullOrWhiteSpace(account)) qs.Add("account=" + Uri.EscapeDataString(account));
            if (!string.IsNullOrWhiteSpace(narration)) qs.Add("narration=" + Uri.EscapeDataString(narration));
            qs.Add("pageNumber=" + pageNumber);
            qs.Add("pageSize=" + pageSize);

            var url = ReceiptEndpoint + "/paged?" + string.Join("&", qs);

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<ReceiptPagedBody>>(json);
                var body = payload?.Body;
                var meta = body?.Metadata;
                return new ReceiptPageResult
                {
                    Data = body?.Data ?? new List<ReceiptDto>(),
                    CurrentPage = meta?.CurrentPage ?? pageNumber,
                    TotalPages = meta?.TotalPages ?? 1,
                    TotalCount = meta?.TotalCount ?? 0,
                    PageSize = meta?.PageSize ?? pageSize
                };
            }
        }
        public async Task<List<ReceiptLineDto>> GetDetailAsync(string voucherNo, string cashBankAccount = "")
        {
            var url = ReceiptEndpoint + "/" + Uri.EscapeDataString(voucherNo);
            if (!string.IsNullOrWhiteSpace(cashBankAccount))
                url += "?cashBankAccount=" + Uri.EscapeDataString(cashBankAccount);

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ReceiptLineDto>>>(json);
                return payload?.Body ?? new List<ReceiptLineDto>();
            }
        }

        public async Task<decimal> GetAccountBalanceAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return 0;

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ReceiptEndpoint + "/accounts/" + Uri.EscapeDataString(accountId) + "/balance");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<ReceiptBalanceDto>>(json);
                return payload?.Body?.Balance ?? 0;
            }
        }

        public async Task<string> CreateAsync(ReceiptCreateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ReceiptEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, ReceiptUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(ReceiptEndpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(ReceiptEndpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteLineAsync(string voucherNo, int seq)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(
                    ReceiptEndpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class ReceiptDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("clearingDate")]
        public DateTime? ClearingDate { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class ReceiptLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("clearingDate")]
        public DateTime? ClearingDate { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("cashBankAccountId")]
        public string CashBankAccountId { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("checkNum")]
        public string CheckNum { get; set; }

        [JsonProperty("checkDate")]
        public DateTime? CheckDate { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class ReceiptBalanceDto
    {
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class ReceiptCreateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("clearingDate")]
        public string ClearingDate { get; set; }

        [JsonProperty("cashBankAccount")]
        public string CashBankAccount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<ReceiptLineRequest> Lines { get; set; } = new List<ReceiptLineRequest>();
    }

    internal class ReceiptUpdateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("clearingDate")]
        public string ClearingDate { get; set; }

        [JsonProperty("cashBankAccount")]
        public string CashBankAccount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<ReceiptLineRequest> Lines { get; set; } = new List<ReceiptLineRequest>();
    }

    internal class ReceiptLineRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("checkNum")]
        public string CheckNum { get; set; }

        [JsonProperty("checkDate")]
        public string CheckDate { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }
    }

    internal class ReceiptPagedBody
    {
        [JsonProperty("data")]
        public List<ReceiptDto> Data { get; set; }

        [JsonProperty("metadata")]
        public ReceiptPageMetadata Metadata { get; set; }
    }

    internal class ReceiptPageMetadata
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }

    internal class ReceiptPageResult
    {
        public List<ReceiptDto> Data { get; set; } = new List<ReceiptDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
    }}
