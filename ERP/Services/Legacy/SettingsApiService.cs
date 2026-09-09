using System;
using System.Net.Http;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class SettingsApiService : ApiServiceBase
    {
        private const string SettingsEndpoint = "/api/settings";

        public async Task<string> GetSettingValueAsync(string key)
        {
            try
            {
                using (var client = CreateClient(includeTenantId: true))
                {
                    var response = await client.GetAsync(SettingsEndpoint + "/" + Uri.EscapeDataString(key));
                    if (!response.IsSuccessStatusCode)
                        return null;

                    var json = await response.Content.ReadAsStringAsync();
                    var payload = JsonConvert.DeserializeObject<HttpResponseDto<SettingItemDto>>(json);
                    return payload?.Body?.Value;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    internal class SettingItemDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}
