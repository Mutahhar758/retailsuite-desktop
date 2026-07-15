using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ERP.Services.Legacy
{
    internal class PersonalApiService : ApiServiceBase
    {
        private const string ProfileEndpoint = "/api/personal/profile";

        public async Task<UserProfileDto> GetProfileAsync()
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(ProfileEndpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<UserProfileDto>>(json);
                return payload?.Body ?? new UserProfileDto();
            }
        }

        private const string PermissionsEndpoint = "/api/personal/permissions";

        public async Task<System.Collections.Generic.List<string>> GetPermissionsAsync()
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(PermissionsEndpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<System.Collections.Generic.List<string>>>(json);
                return payload?.Body ?? new System.Collections.Generic.List<string>();
            }
        }
    }

    internal class UserProfileDto
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }
    }
}
