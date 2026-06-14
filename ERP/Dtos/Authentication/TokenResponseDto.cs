using System;
using Newtonsoft.Json;

namespace ERP.Dtos.Authentication
{
    public class TokenResponseDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("refreshTokenExpiryTime")]
        public DateTime? RefreshTokenExpiryTime { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
