using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class CustomerApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/customers";

        public async Task<List<CustomerDto>> GetAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<CustomerDto>>>(json);
                return payload?.Body ?? new List<CustomerDto>();
            }
        }

        public async Task<CustomerPageResult> GetPagedListAsync(int pageNumber = 1, int pageSize = 20, string search = null, string customerCategoryId = null, bool? active = null)
        {
            using (var client = CreateClient())
            {
                var queryParams = new List<string>
                {
                    $"pageNumber={pageNumber}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(search))
                    queryParams.Add($"search={Uri.EscapeDataString(search.Trim())}");

                if (!string.IsNullOrWhiteSpace(customerCategoryId))
                    queryParams.Add($"customerCategoryId={Uri.EscapeDataString(customerCategoryId.Trim())}");

                if (active.HasValue)
                    queryParams.Add($"active={active.Value}");

                string url = Endpoint + "/paged?" + string.Join("&", queryParams);
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<CustomerPagedBody>>(json);
                if (payload?.Body?.Data != null)
                {
                    return new CustomerPageResult
                    {
                        Data = payload.Body.Data,
                        CurrentPage = payload.Body.Metadata?.CurrentPage ?? pageNumber,
                        TotalPages = payload.Body.Metadata?.TotalPages ?? 1,
                        TotalCount = payload.Body.Metadata?.TotalCount ?? payload.Body.Data.Count,
                        PageSize = payload.Body.Metadata?.PageSize ?? pageSize
                    };
                }
                return new CustomerPageResult { CurrentPage = pageNumber, PageSize = pageSize };
            }
        }

        public async Task UpsertAsync(string account, CustomerUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(account), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<string> CreateAsync(CustomerUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body;
            }
        }

        public async Task DeleteAsync(string account)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(account));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<PresignedUploadUrlDto> GetPresignedUploadUrlAsync(string fileName)
        {
            using (var client = CreateClient())
            {
                var response = await client.PostAsync(Endpoint + "/presigned-upload-url?fileName=" + Uri.EscapeDataString(fileName), null);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PresignedUploadUrlDto>>(json);
                return payload?.Body;
            }
        }

        public async Task<List<CustomerSupplyItemDto>> GetSupplyItemsAsync(string customerId = null, string itemId = null)
        {
            using (var client = CreateClient())
            {
                var query = new List<string>();
                if (!string.IsNullOrWhiteSpace(customerId))
                    query.Add("customerId=" + Uri.EscapeDataString(customerId));
                if (!string.IsNullOrWhiteSpace(itemId))
                    query.Add("itemId=" + Uri.EscapeDataString(itemId));

                var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";
                var response = await client.GetAsync(Endpoint + "/supply-items" + qs);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<CustomerSupplyItemDto>>>(json);
                return payload?.Body ?? new List<CustomerSupplyItemDto>();
            }
        }
    }

    internal class CustomerSupplyItemDto
    {
        [JsonProperty("customerAccountId")]
        public string CustomerAccountId { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("secQty")]
        public decimal? SecQty { get; set; }

        [JsonProperty("rate")]
        public decimal? Rate { get; set; }

        [JsonProperty("addLess")]
        public decimal? AddLess { get; set; }

        [JsonProperty("carriage")]
        public decimal? Carriage { get; set; }

        [JsonProperty("discount")]
        public decimal? Discount { get; set; }
    }

    internal class CustomerDto
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("cnic")]
        public string Cnic { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("qualification")]
        public string Qualification { get; set; }

        [JsonProperty("phone1")]
        public string Phone1 { get; set; }

        [JsonProperty("phone2")]
        public string Phone2 { get; set; }

        [JsonProperty("smsNumber")]
        public string SmsNumber { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("smsAlert")]
        public bool SmsAlert { get; set; }

        [JsonProperty("emailAlert")]
        public bool EmailAlert { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("mediaUrl")]
        public string MediaUrl { get; set; }

        [JsonProperty("customerCategoryId")]
        public string CustomerCategoryId { get; set; }

        [JsonProperty("customerCategoryTitle")]
        public string CustomerCategoryTitle { get; set; }

        [JsonProperty("parentCustomerAccountId")]
        public string ParentCustomerAccountId { get; set; }

        [JsonProperty("parentCustomerTitle")]
        public string ParentCustomerTitle { get; set; }

        [JsonProperty("isSubAccount")]
        public bool IsSubAccount { get; set; }

        [JsonProperty("supplyItems")]
        public List<CustomerSupplyItemDto> SupplyItems { get; set; } = new List<CustomerSupplyItemDto>();
    }

    internal class CustomerUpsertApiRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("cnic")]
        public string Cnic { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("qualification")]
        public string Qualification { get; set; }

        [JsonProperty("phone1")]
        public string Phone1 { get; set; }

        [JsonProperty("phone2")]
        public string Phone2 { get; set; }

        [JsonProperty("smsNumber")]
        public string SmsNumber { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("smsAlert")]
        public bool SmsAlert { get; set; }

        [JsonProperty("emailAlert")]
        public bool EmailAlert { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("customerCategoryId")]
        public string CustomerCategoryId { get; set; }

        [JsonProperty("parentCustomerAccountId")]
        public string ParentCustomerAccountId { get; set; }

        [JsonProperty("supplyItems")]
        public List<CustomerSupplyItemDto> SupplyItems { get; set; } = new List<CustomerSupplyItemDto>();
    }

    internal class CustomerPagedBody
    {
        [JsonProperty("data")]
        public List<CustomerDto> Data { get; set; }

        [JsonProperty("metadata")]
        public CustomerPageMetadata Metadata { get; set; }
    }

    internal class CustomerPageMetadata
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

    internal class CustomerPageResult
    {
        public List<CustomerDto> Data { get; set; } = new List<CustomerDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
