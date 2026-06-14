using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class CompanyApiService : ApiServiceBase
    {
        private const string CompanyEndpoint = "/api/companies/current";

        public async Task<CompanyInfoDto> GetCurrentAsync()
        {
            using (var client = CreateClient(includeTenantId: false))
            {
                var response = await client.GetAsync(CompanyEndpoint);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<CompanyInfoDto>>(json);
                return payload?.Body ?? new CompanyInfoDto();
            }
        }
    }

    internal class CompanyInfoDto
    {
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("urCompanyName")]
        public string UrCompanyName { get; set; }

        [JsonProperty("descr")]
        public string Descr { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("cell")]
        public string Cell { get; set; }

        [JsonProperty("cell2")]
        public string Cell2 { get; set; }

        [JsonProperty("contactHeader")]
        public string ContactHeader { get; set; }
    }
}
