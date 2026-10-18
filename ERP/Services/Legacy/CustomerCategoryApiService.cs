using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class CustomerCategoryApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/customercategories";

        public async Task<List<CustomerCategoryDto>> GetActiveAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<CustomerCategoryDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<CustomerCategoryDto>();
            }
        }

        public async Task<List<CustomerCategoryLookupDto>> GetLookupAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/lookup");
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<CustomerCategoryLookupDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<CustomerCategoryLookupDto>();
            }
        }

        public async Task<string> CreateAsync(string code, string title, string description, bool active)
        {
            using (var client = CreateClient())
            {
                var request = new CustomerCategoryCreateRequest
                {
                    Code = code,
                    Title = title,
                    Description = description,
                    Active = active
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body;
            }
        }

        public async Task UpdateAsync(string code, string title, string description, bool active)
        {
            using (var client = CreateClient())
            {
                var request = new CustomerCategoryUpdateRequest
                {
                    Title = title,
                    Description = description,
                    Active = active
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(code), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string code)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(code));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class CustomerCategoryDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    internal class CustomerCategoryLookupDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class CustomerCategoryCreateRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    internal class CustomerCategoryUpdateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
