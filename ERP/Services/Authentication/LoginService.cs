using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ERP.Classes;
using ERP.Dtos.Authentication;

namespace ERP.Services.Authentication
{
    public class LoginService
    {
        private const string ApiSettingsSection = "ApiSettings";
        private const string DefaultApiBaseUrl = "https://retailer-api.bizgripsolutions.com";
        private const string LoginEndpoint       = "/api/auth/login";
        private const string ChangePasswordEndpoint = "/api/users/change-password";

        private readonly HttpClient _httpClient;
        private readonly string _tenantId;
        private readonly LicenseService _licenseService;

        public LoginService()
        {
            _licenseService = new LicenseService();

            // Read configurable values from Settings.ini
            string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
            new INIFile(iniPath);   // sets the static path in INIFile

            string apiBaseUrl = INIFile.ReadValue(ApiSettingsSection, "ApiBaseUrl", DefaultApiBaseUrl);
            
            // Get TenantIdentifier from ApiSession (selected org) or LicenseService
            _tenantId = !string.IsNullOrEmpty(ApiSession.TenantIdentifier) 
                        ? ApiSession.TenantIdentifier 
                        : _licenseService.GetStoredTenantIdentifier();

            if (string.IsNullOrEmpty(_tenantId))
            {
                _tenantId = string.Empty;
            }

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Required by Finbuckle MultiTenant on every request
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", _tenantId);
        }

        public async Task<LoginResultDto> LoginAsync(string username, string password)
        {
            ApiSession.TenantIdentifier = _tenantId;

            var request = new LoginRequestDto
            {
                Login      = username,
                LoginType  = LoginTypeDto.Username,
                Password   = password,
                DeviceId   = GetOrCreateDeviceId(),
                FcmToken   = string.Empty,
                AppVersion = "1.0.0",
                DeviceName = Environment.MachineName
            };

            var json     = JsonConvert.SerializeObject(request);
            var content  = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(LoginEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "Tenant '" + _tenantId + "' not found. Check TenantId in Settings.ini [ApiSettings]."
                };
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "Login failed (" + (int)response.StatusCode + "): " + responseContent
                };
            }

            var apiResponse = JsonConvert.DeserializeObject<HttpResponseDto<TokenResponseDto>>(responseContent);
            if (apiResponse == null || apiResponse.Body == null || string.IsNullOrEmpty(apiResponse.Body.Token))
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "Invalid response received from API."
                };
            }

            return new LoginResultDto
            {
                Success       = true,
                Message       = apiResponse.Metadata != null && !string.IsNullOrEmpty(apiResponse.Metadata.Message)
                                    ? apiResponse.Metadata.Message
                                    : "Login successful.",
                TokenResponse = apiResponse.Body
            };
        }

        public async Task<string> ChangePasswordAsync(string oldPassword, string newPassword, bool logOutOfAllAccounts)
        {
            if (string.IsNullOrWhiteSpace(ApiSession.AccessToken))
                throw new Exception("You are not authenticated. Please login again.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiSession.AccessToken);

            var request = new
            {
                oldPassword = oldPassword,
                newPassword = newPassword,
                logOutOfAllAccounts = logOutOfAllAccounts
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ChangePasswordEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractServerMessageAsync(response);
                throw new Exception(string.IsNullOrWhiteSpace(message)
                    ? "Change password failed with status code " + (int)response.StatusCode + "."
                    : message);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(responseContent);
            if (payload != null && payload.Metadata != null && !string.IsNullOrWhiteSpace(payload.Metadata.Message))
                return payload.Metadata.Message;

            return "Password changed successfully.";
        }

        private static async Task<string> ExtractServerMessageAsync(HttpResponseMessage response)
        {
            if (response.Content == null)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                return null;

            try
            {
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<object>>(content);
                if (payload != null && payload.Metadata != null)
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

        private string GetOrCreateDeviceId()
        {
            if (!string.IsNullOrEmpty(ApiSession.DeviceId))
                return ApiSession.DeviceId;

            string filePath = INIFile.GetAppDataFilePath("device.id");
            if (File.Exists(filePath))
            {
                ApiSession.DeviceId = File.ReadAllText(filePath);
                return ApiSession.DeviceId;
            }

            ApiSession.DeviceId = Environment.MachineName + "_" + DateTime.Now.ToString("yyyyMMdd");
            File.WriteAllText(filePath, ApiSession.DeviceId);
            return ApiSession.DeviceId;
        }
    }
}
