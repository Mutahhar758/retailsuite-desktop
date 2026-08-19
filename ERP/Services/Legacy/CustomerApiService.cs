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

        [JsonProperty("supplyItems")]
        public List<CustomerSupplyItemDto> SupplyItems { get; set; } = new List<CustomerSupplyItemDto>();
    }
}

