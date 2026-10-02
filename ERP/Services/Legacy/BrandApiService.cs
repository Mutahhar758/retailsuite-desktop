using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class BrandApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/brands";

        public async Task<List<BrandDto>> GetListAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<BrandDto>>>(json);
                return payload?.Body ?? new List<BrandDto>();
            }
        }

        public async Task<List<BrandLookupDto>> GetLookupAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/lookup");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<BrandLookupDto>>>(json);
                return payload?.Body ?? new List<BrandLookupDto>();
            }
        }

        public async Task<string> CreateAsync(BrandUpsertRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateAsync(string id, BrandUpsertRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(id), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string id)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(id));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class BrandDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    internal class BrandLookupDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class BrandUpsertRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; } = true;
    }
}
