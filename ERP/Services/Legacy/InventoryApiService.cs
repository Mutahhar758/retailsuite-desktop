using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class InventoryApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/inventory";

        public async Task<List<InventoryItemDto>> GetItemsAsync(string itemCategoryCode)
        {
            using (var client = CreateClient())
            {
                var url = Endpoint + "/items";
                if (!string.IsNullOrWhiteSpace(itemCategoryCode))
                    url += "?itemCategoryCode=" + Uri.EscapeDataString(itemCategoryCode);

                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<InventoryItemDto>>>(json);
                return payload?.Body ?? new List<InventoryItemDto>();
            }
        }

        public async Task<string> CreateItemAsync(InventoryItemUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint + "/items", content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task<string> UpdateItemAsync(string id, InventoryItemUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/items/" + Uri.EscapeDataString(id), content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? id;
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/items/" + Uri.EscapeDataString(id));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<PresignedUploadUrlDto> GetPresignedUploadUrlAsync(string fileName)
        {
            using (var client = CreateClient())
            {
                var response = await client.PostAsync(Endpoint + "/items/presigned-upload-url?fileName=" + Uri.EscapeDataString(fileName), null);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PresignedUploadUrlDto>>(json);
                return payload?.Body;
            }
        }
    }

    internal class InventoryItemDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("itemCategoryCode")]
        public string ItemCategoryCode { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("itemKey")]
        public string ItemKey { get; set; }

        [JsonProperty("priRate")]
        public decimal PriRate { get; set; }

        [JsonProperty("secRate")]
        public decimal SecRate { get; set; }

        [JsonProperty("primaryUnit")]
        public string PrimaryUnit { get; set; }

        [JsonProperty("secondaryUnit")]
        public string SecondaryUnit { get; set; }

        [JsonProperty("defaultUnit")]
        public string DefaultUnit { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal? QtyInPack { get; set; }

        [JsonProperty("alert")]
        public bool Alert { get; set; }

        [JsonProperty("lowStockAlert")]
        public bool? LowStockAlert { get; set; }

        [JsonProperty("opnStock")]
        public decimal? OpnStock { get; set; }

        [JsonProperty("opnRate")]
        public decimal? OpnRate { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("mediaUrl")]
        public string MediaUrl { get; set; }

        [JsonProperty("itemType")]
        public string ItemType { get; set; }
    }

    internal class InventoryItemUpsertApiRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("itemCategoryCode")]
        public string ItemCategoryCode { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("itemKey")]
        public string ItemKey { get; set; }

        [JsonProperty("priRate")]
        public decimal PriRate { get; set; }

        [JsonProperty("secRate")]
        public decimal SecRate { get; set; }

        [JsonProperty("primaryUnit")]
        public string PrimaryUnit { get; set; }

        [JsonProperty("secondaryUnit")]
        public string SecondaryUnit { get; set; }

        [JsonProperty("defaultUnit")]
        public string DefaultUnit { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal? QtyInPack { get; set; }

        [JsonProperty("alert")]
        public bool Alert { get; set; }

        [JsonProperty("lowStockAlert")]
        public bool? LowStockAlert { get; set; }

        [JsonProperty("opnStock")]
        public decimal? OpnStock { get; set; }

        [JsonProperty("opnRate")]
        public decimal? OpnRate { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("itemType")]
        public string ItemType { get; set; }
    }
}
