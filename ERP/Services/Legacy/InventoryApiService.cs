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

                public async Task<InventoryItemPageResult> GetPagedItemsAsync(int pageNumber = 1, int pageSize = 20, string search = null, string itemCategoryCode = null, string brandId = null, string itemType = null)
        {
            using (var client = CreateClient())
            {
                var qs = new List<string>
                {
                    $"pageNumber={pageNumber}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(search))
                    qs.Add($"search={Uri.EscapeDataString(search.Trim())}");

                if (!string.IsNullOrWhiteSpace(itemCategoryCode) && itemCategoryCode != "ALL")
                    qs.Add($"itemCategoryCode={Uri.EscapeDataString(itemCategoryCode.Trim())}");

                if (!string.IsNullOrWhiteSpace(brandId))
                    qs.Add($"brandId={Uri.EscapeDataString(brandId.Trim())}");

                if (!string.IsNullOrWhiteSpace(itemType))
                    qs.Add($"itemType={Uri.EscapeDataString(itemType.Trim())}");

                var url = Endpoint + "/items/paged?" + string.Join("&", qs);
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<InventoryItemPagedBody>>(json);
                if (payload?.Body?.Data != null)
                {
                    return new InventoryItemPageResult
                    {
                        Data = payload.Body.Data,
                        CurrentPage = payload.Body.Metadata?.CurrentPage ?? pageNumber,
                        TotalPages = payload.Body.Metadata?.TotalPages ?? 1,
                        TotalCount = payload.Body.Metadata?.TotalCount ?? payload.Body.Data.Count,
                        PageSize = payload.Body.Metadata?.PageSize ?? pageSize
                    };
                }
                return new InventoryItemPageResult { CurrentPage = pageNumber, PageSize = pageSize };
            }
        }

                public async Task<InventoryItemDto> GetItemAsync(string id)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/items/" + Uri.EscapeDataString(id));
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<InventoryItemDto>>(json);
                return payload?.Body;
            }
        }

        public async Task<List<InventoryItemDto>> GetLookupAsync(string itemCategoryCode, bool includeLastPurchaseRate = false)
        {
            using (var client = CreateClient())
            {
                var url = Endpoint + "/items/lookup";
                if (!string.IsNullOrWhiteSpace(itemCategoryCode))
                    url += "?itemCategoryCode=" + Uri.EscapeDataString(itemCategoryCode);
                if (includeLastPurchaseRate)
                    url += (url.Contains("?") ? "&" : "?") + "includeLastPurchaseRate=true";

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

        public async Task<List<ItemCustomerCategoryPriceDto>> GetCustomerCategoryPricesAsync(string customerCategoryId = null)
        {
            using (var client = CreateClient())
            {
                var url = Endpoint + "/customer-category-prices";
                if (!string.IsNullOrWhiteSpace(customerCategoryId))
                    url += "?customerCategoryId=" + Uri.EscapeDataString(customerCategoryId);

                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ItemCustomerCategoryPriceDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<ItemCustomerCategoryPriceDto>();
            }
        }

        public async Task<List<ItemCustomerCategoryPriceDto>> GetItemCategoryPricesAsync(string itemId)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/items/" + Uri.EscapeDataString(itemId) + "/customer-category-prices");
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ItemCustomerCategoryPriceDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<ItemCustomerCategoryPriceDto>();
            }
        }

        public async Task SaveItemCategoryPricesAsync(string itemId, List<ItemCustomerCategoryPriceUpsertRequest> request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/items/" + Uri.EscapeDataString(itemId) + "/customer-category-prices", content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    public class ItemCustomerCategoryPriceDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("customerCategoryId")]
        public string CustomerCategoryId { get; set; }

        [JsonProperty("customerCategoryTitle")]
        public string CustomerCategoryTitle { get; set; }

        [JsonProperty("priRate")]
        public decimal PriRate { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }
    }

    public class ItemCustomerCategoryPriceUpsertRequest
    {
        [JsonProperty("customerCategoryId")]
        public string CustomerCategoryId { get; set; }

        [JsonProperty("priRate")]
        public decimal PriRate { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }
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

        [JsonProperty("lastPurchaseRate")]
        public decimal? LastPurchaseRate { get; set; }

        [JsonProperty("lastPurchaseSecRate")]
        public decimal? LastPurchaseSecRate { get; set; }

        [JsonProperty("primaryUnit")]
        public string PrimaryUnit { get; set; }

        [JsonProperty("primaryUnitTitle")]
        public string PrimaryUnitTitle { get; set; }

        [JsonProperty("secondaryUnit")]
        public string SecondaryUnit { get; set; }

        [JsonProperty("secondaryUnitTitle")]
        public string SecondaryUnitTitle { get; set; }

        [JsonProperty("defaultUnit")]
        public string DefaultUnit { get; set; }

        [JsonProperty("defaultUnitTitle")]
        public string DefaultUnitTitle { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal? QtyInPack { get; set; }

        [JsonProperty("alert")]
        public bool Alert { get; set; }

        [JsonProperty("lowStockAlert")]
        public bool? LowStockAlert { get; set; }

        [JsonProperty("lowStockQty")]
        public decimal? LowStockQty { get; set; }

        [JsonProperty("minSalePrice")]
        public decimal? MinSalePrice { get; set; }

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

        [JsonProperty("requireImei")]
        public bool? RequireImei { get; set; }

        [JsonProperty("brandId")]
        public string BrandId { get; set; }

        [JsonProperty("brandTitle")]
        public string BrandTitle { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("storage")]
        public string Storage { get; set; }

        [JsonProperty("ram")]
        public string Ram { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("additionalUnits")]
        public string AdditionalUnits { get; set; }
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

        [JsonProperty("lowStockQty")]
        public decimal? LowStockQty { get; set; }

        [JsonProperty("minSalePrice")]
        public decimal? MinSalePrice { get; set; }

        [JsonProperty("opnStock")]
        public decimal? OpnStock { get; set; }

        [JsonProperty("opnRate")]
        public decimal? OpnRate { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("itemType")]
        public string ItemType { get; set; }

        [JsonProperty("requireImei")]
        public bool? RequireImei { get; set; }

        [JsonProperty("brandId")]
        public string BrandId { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("storage")]
        public string Storage { get; set; }

        [JsonProperty("ram")]
        public string Ram { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("additionalUnits")]
        public string AdditionalUnits { get; set; }
    }

    internal class InventoryItemPagedBody
    {
        [JsonProperty("data")]
        public List<InventoryItemDto> Data { get; set; }

        [JsonProperty("metadata")]
        public InventoryItemPageMetadata Metadata { get; set; }
    }

    internal class InventoryItemPageMetadata
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

    internal class InventoryItemPageResult
    {
        public List<InventoryItemDto> Data { get; set; } = new List<InventoryItemDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
    }
}

