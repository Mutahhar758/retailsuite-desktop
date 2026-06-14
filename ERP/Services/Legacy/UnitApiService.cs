using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class UnitApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/units";

        public async Task<List<UnitLookupDto>> GetActiveAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<UnitLookupDto>>>(json);
                return payload?.Body ?? new List<UnitLookupDto>();
            }
        }

        public async Task UpsertAsync(string code, UnitUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + code, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class UnitLookupDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class UnitUpsertApiRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
