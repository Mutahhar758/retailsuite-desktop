using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ERP.Classes;
using ERP.Dtos.Authentication;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ERP.Services.Authentication
{
    public class OrganizationLicense
    {
        public string LicenseKey { get; set; }
        public string TenantIdentifier { get; set; }
        public string Name { get; set; }
    }

    public class LicenseService
    {
        private const string ApiSettingsSection = "ApiSettings";
        private const string DefaultApiBaseUrl = "https://retailer-api.bizgripsolutions.com";
        private const string LicenseFileName = "license.sec";

        private readonly HttpClient _httpClient;
        private readonly string _licensePath;

        public LicenseService()
        {
            _licensePath = INIFile.GetAppDataFilePath(LicenseFileName, true);
            
            // Still read API URL from Settings.ini
            string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
            new INIFile(iniPath);
            string apiBaseUrl = INIFile.ReadValue(ApiSettingsSection, "ApiBaseUrl", DefaultApiBaseUrl);

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public List<OrganizationLicense> GetAllLicenses()
        {
            var licenses = new List<OrganizationLicense>();
            if (!File.Exists(_licensePath)) return licenses;

            try
            {
                var sections = INIFile.ReadSections(_licensePath);
                if (sections == null) return licenses;

                foreach (var section in sections)
                {
                    if (section.StartsWith("License_"))
                    {
                        string encryptedKey = INIFile.ReadValue(section, "LicenseKey", "", _licensePath);
                        string encryptedTenantIdentifier = INIFile.ReadValue(section, "TenantIdentifier", "", _licensePath);
                        string encryptedName = INIFile.ReadValue(section, "Name", "", _licensePath);

                        if (!string.IsNullOrEmpty(encryptedKey) && !string.IsNullOrEmpty(encryptedTenantIdentifier))
                        {
                            licenses.Add(new OrganizationLicense
                            {
                                LicenseKey = SecurityHelper.Decrypt(encryptedKey),
                                TenantIdentifier = SecurityHelper.Decrypt(encryptedTenantIdentifier),
                                Name = SecurityHelper.Decrypt(encryptedName) ?? "Unknown Organization"
                            });
                        }
                    }
                }
            }
            catch { }

            return licenses;
        }

        public async Task<(bool Success, OrganizationLicense License, string Message)> VerifyLicenseKeyAsync(string licenseKey)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/license/verify/{licenseKey}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Update this to match the new API response (TenantDto)
                    var apiResponse = JsonConvert.DeserializeObject<HttpResponseDto<JObject>>(content);
                    if (apiResponse != null && apiResponse.Body != null)
                    {
                        var tenantData = apiResponse.Body;
                        var license = new OrganizationLicense
                        {
                            LicenseKey = licenseKey,
                            TenantIdentifier = tenantData["identifier"]?.ToString() ?? tenantData["id"]?.ToString(),
                            Name = tenantData["name"]?.ToString()
                        };

                        StoreLicense(license);
                        return (true, license, "License verified successfully.");
                    }
                }

                return (false, null, "Invalid or inactive license key.");
            }
            catch (Exception ex)
            {
                return (false, null, "Error verifying license: " + ex.Message);
            }
        }

        public void StoreLicense(OrganizationLicense license)
        {
            string section = "License_" + license.LicenseKey.GetHashCode().ToString("X");
            
            string encryptedKey = SecurityHelper.Encrypt(license.LicenseKey);
            string encryptedTenantIdentifier = SecurityHelper.Encrypt(license.TenantIdentifier);
            string encryptedName = SecurityHelper.Encrypt(license.Name);

            INIFile.WriteValue(section, "LicenseKey", encryptedKey, _licensePath);
            INIFile.WriteValue(section, "TenantIdentifier", encryptedTenantIdentifier, _licensePath);
            INIFile.WriteValue(section, "Name", encryptedName, _licensePath);
        }

        public bool HasAnyLicense()
        {
            return GetAllLicenses().Any();
        }

        // Keep this for backward compatibility or ease of use in services
        public string GetStoredTenantIdentifier()
        {
            var licenses = GetAllLicenses();
            return licenses.FirstOrDefault()?.TenantIdentifier;
        }
    }
}
