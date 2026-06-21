using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ERP.Classes;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ERP.Services.Legacy
{
    internal abstract class ApiServiceBase
    {
        private const string ApiSettingsSection = "ApiSettings";
        private const string DefaultApiBaseUrl = "https://retailer-api.bizgripsolutions.com";

        protected static async Task EnsureSuccessWithServerMessageAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var message = await ExtractServerMessageAsync(response);
            throw new Exception(string.IsNullOrWhiteSpace(message)
                ? "Request failed with status code " + (int)response.StatusCode + "."
                : message);
        }

        protected static async Task<string> ExtractServerMessageAsync(HttpResponseMessage response)
        {
            if (response.Content == null)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                return null;

            try
            {
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<object>>(content);
                if (payload?.Metadata != null)
                {
                    if (!string.IsNullOrWhiteSpace(payload.Metadata.Message))
                        return payload.Metadata.Message;

                    if (payload.Metadata.ValidationErrors != null)
                        return payload.Metadata.ValidationErrors.ToString();
                }
            }
            catch
            {
            }

            try
            {
                var obj = JObject.Parse(content);
                var message = (string)obj["metadata"]?["message"]
                              ?? (string)obj["message"]
                              ?? (string)obj["detail"]
                              ?? (string)obj["title"];
                return message;
            }
            catch
            {
                return null;
            }
        }

        protected HttpClient CreateClient(bool includeTenantId = false)
        {
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.ini");
            new INIFile(iniPath);

            string apiBaseUrl = INIFile.ReadValue(ApiSettingsSection, "ApiBaseUrl", DefaultApiBaseUrl);

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (includeTenantId)
            {
                string tenantId = !string.IsNullOrWhiteSpace(ApiSession.TenantIdentifier)
                    ? ApiSession.TenantIdentifier
                    : new Authentication.LicenseService().GetStoredTenantIdentifier();

                if (!string.IsNullOrWhiteSpace(tenantId))
                    client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId);
            }

            if (!string.IsNullOrWhiteSpace(ApiSession.AccessToken))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiSession.AccessToken);

            return client;
        }

        public async Task UploadFileAsync(string uploadUrl, string filePath)
        {
            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            using (var fileStream = System.IO.File.OpenRead(filePath))
            using (var streamContent = new StreamContent(fileStream))
            {
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                form.Add(streamContent, "File", System.IO.Path.GetFileName(filePath));
                var response = await client.PostAsync(uploadUrl, form);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    public class PresignedUploadUrlDto
    {
        [JsonProperty("fileId")]
        public string FileId { get; set; }

        [JsonProperty("uploadUrl")]
        public string UploadUrl { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }
    }
}
