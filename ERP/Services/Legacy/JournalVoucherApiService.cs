using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class JournalVoucherApiService : ApiServiceBase
    {
        private const string JournalVoucherEndpoint = "/api/journalvouchers";

        public async Task<List<JournalVoucherDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string account = "", string narration = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(account)) qs.Add("account=" + Uri.EscapeDataString(account));
            if (!string.IsNullOrWhiteSpace(narration)) qs.Add("narration=" + Uri.EscapeDataString(narration));

            var url = JournalVoucherEndpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<JournalVoucherDto>>>(json);
                return payload?.Body ?? new List<JournalVoucherDto>();
            }
        }

        public async Task<List<JournalVoucherLineDto>> GetDetailAsync(string voucherNo, string account = "")
        {
            var url = JournalVoucherEndpoint + "/" + Uri.EscapeDataString(voucherNo);
            if (!string.IsNullOrWhiteSpace(account))
                url += "?account=" + Uri.EscapeDataString(account);

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<JournalVoucherLineDto>>>(json);
                return payload?.Body ?? new List<JournalVoucherLineDto>();
            }
        }

        public async Task<decimal> GetAccountBalanceAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return 0;

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(JournalVoucherEndpoint + "/accounts/" + Uri.EscapeDataString(accountId) + "/balance");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<JournalVoucherBalanceDto>>(json);
                return payload?.Body?.Balance ?? 0;
            }
        }

        public async Task<string> CreateAsync(JournalVoucherCreateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(JournalVoucherEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, JournalVoucherUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(JournalVoucherEndpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(JournalVoucherEndpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteLineAsync(string voucherNo, int seq)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(
                    JournalVoucherEndpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class JournalVoucherDto
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

    internal class JournalVoucherLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("drAccountId")]
        public string DrAccountId { get; set; }

        [JsonProperty("crAccountId")]
        public string CrAccountId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

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

    internal class JournalVoucherBalanceDto
    {
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }

    internal class JournalVoucherCreateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<JournalVoucherLineRequest> Lines { get; set; } = new List<JournalVoucherLineRequest>();
    }

    internal class JournalVoucherUpdateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<JournalVoucherLineRequest> Lines { get; set; } = new List<JournalVoucherLineRequest>();
    }

    internal class JournalVoucherLineRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("drAccount")]
        public string DrAccount { get; set; }

        [JsonProperty("crAccount")]
        public string CrAccount { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }
    }
}
