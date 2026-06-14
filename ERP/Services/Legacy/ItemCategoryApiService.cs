using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class ItemCategoryApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/itemcategories";

        public async Task<List<ItemCategoryDto>> GetActiveAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ItemCategoryDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<ItemCategoryDto>();
            }
        }

        public async Task CreateAsync(string title, bool active)
        {
            using (var client = CreateClient())
            {
                var request = new ItemCategoryCreateRequest
                {
                    Title = title,
                    Active = active
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task UpdateAsync(string code, string title, bool active)
        {
            using (var client = CreateClient())
            {
                var request = new ItemCategoryUpdateRequest
                {
                    Title = title,
                    Active = active
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(code), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class ItemCategoryDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    internal class ItemCategoryCreateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    internal class ItemCategoryUpdateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}

