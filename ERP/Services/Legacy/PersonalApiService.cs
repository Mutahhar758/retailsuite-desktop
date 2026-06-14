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
    }

    internal class UserProfileDto
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
