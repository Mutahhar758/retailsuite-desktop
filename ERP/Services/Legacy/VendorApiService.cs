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
    }
}
