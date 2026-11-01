using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class CompanyApiService : ApiServiceBase
    {
        private const string CompanyEndpoint = "/api/companies/current";
        private const string UploadUrlEndpoint = "/api/companies/presigned-upload-url";

        public async Task<CompanyInfoDto> GetCurrentAsync()
        {
            using (var client = CreateClient(includeTenantId: false))
            {
                var response = await client.GetAsync(CompanyEndpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<CompanyInfoDto>>(json);
                return payload?.Body ?? new CompanyInfoDto();
            }
        }

        public async Task<CompanyInfoDto> UpdateCurrentAsync(CompanyUpdateRequestDto request)
        {
            using (var client = CreateClient(includeTenantId: false))
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(CompanyEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<CompanyInfoDto>>(json);
                return payload?.Body ?? new CompanyInfoDto();
            }
        }

        public async Task<PresignedUploadUrlDto> GetLogoUploadUrlAsync(string fileName)
        {
            using (var client = CreateClient(includeTenantId: false))
            {
                var response = await client.PostAsync(UploadUrlEndpoint + "?fileName=" + Uri.EscapeDataString(fileName), null);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PresignedUploadUrlDto>>(json);
                return payload?.Body;
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

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("ntn")]
        public string Ntn { get; set; }

        [JsonProperty("strn")]
        public string Strn { get; set; }

        [JsonProperty("logoMediaId")]
        public string LogoMediaId { get; set; }

        [JsonProperty("logoUrl")]
        public string LogoUrl { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class CompanyUpdateRequestDto
    {
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

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("ntn")]
        public string Ntn { get; set; }

        [JsonProperty("strn")]
        public string Strn { get; set; }

        [JsonProperty("logoMediaId")]
        public string LogoMediaId { get; set; }
    }
}
