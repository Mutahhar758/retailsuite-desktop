using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class PaymentApiService : ApiServiceBase
    {
        private const string PaymentEndpoint = "/api/payments";

        public async Task<List<PaymentDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string cashBank = "", string account = "", string narration = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(cashBank)) qs.Add("cashBankAccount=" + Uri.EscapeDataString(cashBank));
            if (!string.IsNullOrWhiteSpace(account)) qs.Add("account=" + Uri.EscapeDataString(account));
            if (!string.IsNullOrWhiteSpace(narration)) qs.Add("narration=" + Uri.EscapeDataString(narration));

            var url = PaymentEndpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<PaymentDto>>>(json);
                return payload?.Body ?? new List<PaymentDto>();
            }
        }

        public async Task<List<PaymentLineDto>> GetDetailAsync(string voucherNo, string cashBankAccount = "")
        {
            var url = PaymentEndpoint + "/" + Uri.EscapeDataString(voucherNo);
            if (!string.IsNullOrWhiteSpace(cashBankAccount))
                url += "?cashBankAccount=" + Uri.EscapeDataString(cashBankAccount);

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<PaymentLineDto>>>(json);
                return payload?.Body ?? new List<PaymentLineDto>();
            }
        }

        public async Task<decimal> GetAccountBalanceAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return 0;

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(PaymentEndpoint + "/accounts/" + Uri.EscapeDataString(accountId) + "/balance");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PaymentBalanceDto>>(json);
                return payload?.Body?.Balance ?? 0;
            }
        }

        public async Task<string> CreateAsync(PaymentCreateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(PaymentEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, PaymentUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(PaymentEndpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(PaymentEndpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteLineAsync(string voucherNo, int seq)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(
                    PaymentEndpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class PaymentDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

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

    internal class PaymentLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

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

    internal class PaymentBalanceDto
    {
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class PaymentCreateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("cashBankAccount")]
        public string CashBankAccount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<PaymentLineRequest> Lines { get; set; } = new List<PaymentLineRequest>();
    }

    internal class PaymentUpdateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("cashBankAccount")]
        public string CashBankAccount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<PaymentLineRequest> Lines { get; set; } = new List<PaymentLineRequest>();
    }

    internal class PaymentLineRequest
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
}
