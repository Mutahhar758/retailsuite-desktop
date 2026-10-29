using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class VendorApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/vendors";

        public async Task<List<VendorDto>> GetAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<VendorDto>>>(json);
                return payload?.Body ?? new List<VendorDto>();
            }
        }

                public async Task<VendorPageResult> GetPagedListAsync(int pageNumber = 1, int pageSize = 20, string search = null, bool? active = null, bool? showInSales = null)
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

                if (active.HasValue)
                    queryParams.Add($"active={active.Value}");

                if (showInSales.HasValue)
                    queryParams.Add($"showInSales={showInSales.Value}");

                string url = Endpoint + "/paged?" + string.Join("&", queryParams);
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<VendorPagedBody>>(json);
                if (payload?.Body?.Data != null)
                {
                    return new VendorPageResult
                    {
                        Data = payload.Body.Data,
                        CurrentPage = payload.Body.Metadata?.CurrentPage ?? pageNumber,
                        TotalPages = payload.Body.Metadata?.TotalPages ?? 1,
                        TotalCount = payload.Body.Metadata?.TotalCount ?? payload.Body.Data.Count,
                        PageSize = payload.Body.Metadata?.PageSize ?? pageSize
                    };
                }
                return new VendorPageResult { CurrentPage = pageNumber, PageSize = pageSize };
            }
        }

        public async Task UpsertAsync(string account, VendorUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(account), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<string> CreateAsync(VendorUpsertApiRequest request)
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
    }

    internal class VendorDto
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

        [JsonProperty("showInSales")]
        public bool ShowInSales { get; set; }

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
    }

    internal class VendorUpsertApiRequest
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

        [JsonProperty("showInSales")]
        public bool ShowInSales { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }
    }

    internal class VendorPagedBody
    {
        [JsonProperty("data")]
        public List<VendorDto> Data { get; set; }

        [JsonProperty("metadata")]
        public VendorPageMetadata Metadata { get; set; }
    }

    internal class VendorPageMetadata
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

    internal class VendorPageResult
    {
        public List<VendorDto> Data { get; set; } = new List<VendorDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
