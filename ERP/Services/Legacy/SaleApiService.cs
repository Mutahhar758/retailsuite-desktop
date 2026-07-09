using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class SaleApiService : ApiServiceBase
    {
        private const string SaleEndpoint = "/api/sales";

        public async Task<List<SaleDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string account = "", string voucherNo = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(account)) qs.Add("account=" + Uri.EscapeDataString(account));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));

            var url = SaleEndpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleDto>>>(json);
                return payload?.Body ?? new List<SaleDto>();
            }
        }

        public async Task<List<SaleLineDto>> GetDetailAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(SaleEndpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleLineDto>>>(json);
                return payload?.Body ?? new List<SaleLineDto>();
            }
        }

        public async Task<string> CreateAsync(SaleCreateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(SaleEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, SaleUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(SaleEndpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(SaleEndpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteLineAsync(string voucherNo, int seq)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(SaleEndpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class SaleDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class SaleLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemKey")]
        public string ItemKey { get; set; }

        [JsonProperty("itemCategoryCode")]
        public string ItemCategoryCode { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("secQty")]
        public decimal? SecQty { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }

        [JsonProperty("secUnit")]
        public string SecUnit { get; set; }

        [JsonProperty("cashReceipt")]
        public decimal CashReceipt { get; set; }

        [JsonProperty("cashBack")]
        public decimal CashBack { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class SaleCreateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("cashReceipt")]
        public decimal CashReceipt { get; set; }

        [JsonProperty("cashBack")]
        public decimal CashBack { get; set; }

        [JsonProperty("lines")]
        public List<SaleLineRequest> Lines { get; set; } = new List<SaleLineRequest>();
    }

    internal class SaleUpdateRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("cashReceipt")]
        public decimal CashReceipt { get; set; }

        [JsonProperty("cashBack")]
        public decimal CashBack { get; set; }

        [JsonProperty("lines")]
        public List<SaleLineRequest> Lines { get; set; } = new List<SaleLineRequest>();
    }

    internal class SaleLineRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("secQty")]
        public decimal? SecQty { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }

        [JsonProperty("secUnit")]
        public string SecUnit { get; set; }
    }
}
