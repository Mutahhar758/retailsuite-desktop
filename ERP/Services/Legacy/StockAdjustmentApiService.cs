using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class StockAdjustmentApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/stockadjustments";

        public async Task<List<StockAdjustmentDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string itemCategoryCode = "", string voucherNo = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(itemCategoryCode)) qs.Add("itemCategoryCode=" + Uri.EscapeDataString(itemCategoryCode));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<StockAdjustmentDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<StockAdjustmentDto>();
            }
        }

        public async Task<List<StockAdjustmentLineDto>> GetDetailAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<StockAdjustmentLineDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<StockAdjustmentLineDto>();
            }
        }

        public async Task<string> CreateAsync(StockAdjustmentCreateApiRequest request)
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

        public async Task UpdateAsync(string voucherNo, StockAdjustmentUpdateApiRequest request)
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

    internal class StockAdjustmentDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class StockAdjustmentLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("itemCategoryCode")]
        public string ItemCategoryCode { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemKey")]
        public string ItemKey { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qtyIn")]
        public decimal QtyIn { get; set; }

        [JsonProperty("qtyOut")]
        public decimal QtyOut { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("secQtyIn")]
        public decimal? SecQtyIn { get; set; }

        [JsonProperty("secQtyOut")]
        public decimal? SecQtyOut { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }

        [JsonProperty("secUnit")]
        public string SecUnit { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class StockAdjustmentCreateApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<StockAdjustmentLineApiRequest> Lines { get; set; } = new List<StockAdjustmentLineApiRequest>();
    }

    internal class StockAdjustmentUpdateApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("lines")]
        public List<StockAdjustmentLineApiRequest> Lines { get; set; } = new List<StockAdjustmentLineApiRequest>();
    }

    internal class StockAdjustmentLineApiRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("itemCategoryCode")]
        public string ItemCategoryCode { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qtyIn")]
        public decimal QtyIn { get; set; }

        [JsonProperty("qtyOut")]
        public decimal QtyOut { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("secQtyIn")]
        public decimal? SecQtyIn { get; set; }

        [JsonProperty("secQtyOut")]
        public decimal? SecQtyOut { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }

        [JsonProperty("secUnit")]
        public string SecUnit { get; set; }
    }
}
