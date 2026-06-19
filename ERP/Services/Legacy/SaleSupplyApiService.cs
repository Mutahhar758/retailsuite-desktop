using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class SaleSupplyApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/salesupplies";

        public async Task<List<SaleSupplyDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string itemId = "", string voucherNo = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(itemId)) qs.Add("itemId=" + Uri.EscapeDataString(itemId));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleSupplyDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<SaleSupplyDto>();
            }
        }

        public async Task<List<SaleSupplyLineDto>> GetDetailAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleSupplyLineDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<SaleSupplyLineDto>();
            }
        }

        public async Task<string> CreateAsync(SaleSupplyCreateApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload != null ? payload.Body : string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, SaleSupplyUpdateApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteLineAsync(string voucherNo, int seq)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class SaleSupplyDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class SaleSupplyLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("narrationId")]
        public string NarrationId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("addLess")]
        public decimal AddLess { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class SaleSupplyCreateApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("lines")]
        public List<SaleSupplyLineApiRequest> Lines { get; set; } = new List<SaleSupplyLineApiRequest>();
    }

    internal class SaleSupplyUpdateApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("lines")]
        public List<SaleSupplyLineApiRequest> Lines { get; set; } = new List<SaleSupplyLineApiRequest>();
    }

    internal class SaleSupplyLineApiRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("addLess")]
        public decimal AddLess { get; set; }
    }
}
