using Newtonsoft.Json;

namespace ERP.Dtos.Authentication
{
    public class HttpResponseMetadataDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("validationErrors")]
        public object ValidationErrors { get; set; }

        [JsonProperty("errorId")]
        public string ErrorId { get; set; }
    }
}
