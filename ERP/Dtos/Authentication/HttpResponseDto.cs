using Newtonsoft.Json;

namespace ERP.Dtos.Authentication
{
    public class HttpResponseDto<T>
    {
        [JsonProperty("body")]
        public T Body { get; set; }

        [JsonProperty("metadata")]
        public HttpResponseMetadataDto Metadata { get; set; }
    }
}
