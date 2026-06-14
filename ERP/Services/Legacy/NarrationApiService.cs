using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class NarrationApiService : ApiServiceBase
    {
        private const string NarrationEndpoint = "/api/narrations";

        public async Task<List<NarrationDto>> GetActiveNarrationsAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(NarrationEndpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<NarrationDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<NarrationDto>();
            }
        }

        public async Task CreateAsync(string title)
        {
            using (var client = CreateClient())
            {
                var request = new NarrationCreateRequest
                {
                    Title = title
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(NarrationEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task UpdateAsync(string code, string title)
        {
            using (var client = CreateClient())
            {
                var request = new NarrationUpdateRequest
                {
                    Title = title
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(NarrationEndpoint + "/" + Uri.EscapeDataString(code), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string code)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(NarrationEndpoint + "/" + Uri.EscapeDataString(code));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class NarrationDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    internal class NarrationCreateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class NarrationUpdateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
