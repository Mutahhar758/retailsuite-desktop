using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class BomApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/boms";

        public async Task<List<BomDto>> GetListAsync(string finishedItemId = "", string search = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(finishedItemId)) qs.Add("finishedItemId=" + Uri.EscapeDataString(finishedItemId));
            if (!string.IsNullOrWhiteSpace(search)) qs.Add("search=" + Uri.EscapeDataString(search));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<BomDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<BomDto>();
            }
        }

        public async Task<BomDto> GetByIdAsync(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + id);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<BomDto>>(json);
                return payload != null ? payload.Body : null;
            }
        }

        public async Task<int> CreateAsync(BomCreateUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                int id;
                if (payload != null && int.TryParse(payload.Body, out id)) return id;
                return 0;
            }
        }

        public async Task UpdateAsync(int id, BomCreateUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + id, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + id);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class BomDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string BomCode { get; set; }

        [JsonProperty("title")]
        public string BomName { get; set; }

        [JsonProperty("finishedItemId")]
        public string FinishedItemId { get; set; }

        [JsonProperty("finishedItemTitle")]
        public string FinishedItemName { get; set; }

        [JsonProperty("unitTitle")]
        public string FinishedUnit { get; set; }

        [JsonProperty("outputQty")]
        public decimal OutputQuantity { get; set; }

        [JsonProperty("laborCost")]
        public decimal EstimatedLaborCost { get; set; }

        [JsonProperty("overheadCost")]
        public decimal EstimatedOverheadCost { get; set; }

        [JsonProperty("totalEstimatedCost")]
        public decimal TotalEstimatedCost { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("lines")]
        public List<BomDetailDto> Items { get; set; } = new List<BomDetailDto>();
    }

    internal class BomDetailDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }

        [JsonProperty("rawItemTitle")]
        public string RawItemName { get; set; }

        [JsonProperty("unitTitle")]
        public string RawUnit { get; set; }

        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        [JsonProperty("currentRate")]
        public decimal StandardCost { get; set; }

        [JsonProperty("totalEstimatedCost")]
        public decimal LineCost { get; set; }

        [JsonProperty("scrapPercentage")]
        public decimal ScrapPercentage { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    internal class BomCreateUpdateRequest
    {
        [JsonProperty("code")]
        public string BomCode { get; set; }

        [JsonProperty("title")]
        public string BomName { get; set; }

        [JsonProperty("finishedItemId")]
        public string FinishedItemId { get; set; }

        [JsonProperty("outputQty")]
        public decimal OutputQuantity { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("laborCost")]
        public decimal EstimatedLaborCost { get; set; }

        [JsonProperty("overheadCost")]
        public decimal EstimatedOverheadCost { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("lines")]
        public List<BomDetailCreateUpdateRequest> Items { get; set; } = new List<BomDetailCreateUpdateRequest>();
    }

    internal class BomDetailCreateUpdateRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }

        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("scrapPercentage")]
        public decimal ScrapPercentage { get; set; }

        [JsonProperty("isOptional")]
        public bool IsOptional { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }
}
