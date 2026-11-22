using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERP.Classes;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Offline
{
    internal class MachineTerminalConfig
    {
        public string MachineId { get; set; }
        public string TerminalCode { get; set; }
        public Dictionary<string, string> TenantTerminalCodes { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public string MachineName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }

    internal class TerminalRegisterResponseDto
    {
        [JsonProperty("terminalCode")]
        public string TerminalCode { get; set; }

        [JsonProperty("isNewlyAssigned")]
        public bool IsNewlyAssigned { get; set; }
    }

    /// <summary>
    /// Generates terminal-scoped, monotonically increasing offline voucher numbers (e.g., T01-00001).
    /// Each PC generates a unique MachineId stored at machine level (C:\ProgramData\RetailSuite\terminal.json)
    /// and registers with retailsuite-api (POST /api/terminals/register) to receive a guaranteed-unique
    /// sequential Terminal ID (T01, T02, T03, ...) per tenant.
    /// </summary>
    internal static class OfflineVoucherGenerator
    {
        private const string ApiSettingsSection = "ApiSettings";
        private const string DefaultApiBaseUrl = "https://retailer-api.bizgripsolutions.com";
        private const string OfflineSettingsSection = "OfflineSettings";
        private const string TerminalCodeKey = "TerminalCode";

        private static string GetMachineConfigPath()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(commonAppData, "RetailSuite", "terminal.json");
        }

        public static string SanitizeTerminalCode(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "T01";

            var chars = raw.Trim().ToUpperInvariant().Where(char.IsLetterOrDigit).Take(6).ToArray();
            if (chars.Length < 2)
                return "T01";

            return new string(chars);
        }

        private static MachineTerminalConfig LoadOrInitMachineConfig()
        {
            string machinePath = GetMachineConfigPath();
            var cfg = OfflineStore.ReadJson<MachineTerminalConfig>(machinePath);
            bool needsSave = false;

            if (cfg == null)
            {
                cfg = new MachineTerminalConfig
                {
                    MachineId = Guid.NewGuid().ToString("N"),
                    MachineName = Environment.MachineName,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };
                needsSave = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cfg.MachineId))
                {
                    cfg.MachineId = Guid.NewGuid().ToString("N");
                    needsSave = true;
                }
                if (cfg.TenantTerminalCodes == null)
                {
                    cfg.TenantTerminalCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
            }

            if (needsSave)
            {
                try { OfflineStore.WriteJsonAtomic(machinePath, cfg); } catch { }
            }

            return cfg;
        }

        public static string GetMachineId()
        {
            return LoadOrInitMachineConfig().MachineId;
        }

        public static bool HasAssignedTerminalCode(string tenantId = null)
        {
            var cfg = LoadOrInitMachineConfig();
            string resolvedTenant = OfflineStore.ResolveTenantId(tenantId);
            if (cfg.TenantTerminalCodes != null && cfg.TenantTerminalCodes.TryGetValue(resolvedTenant, out string tenantCode) && !string.IsNullOrWhiteSpace(tenantCode))
                return true;

            return !string.IsNullOrWhiteSpace(cfg.TerminalCode);
        }

        public static string GetTerminalCode(string tenantId = null)
        {
            var cfg = LoadOrInitMachineConfig();
            string resolvedTenant = OfflineStore.ResolveTenantId(tenantId);

            if (cfg.TenantTerminalCodes != null && cfg.TenantTerminalCodes.TryGetValue(resolvedTenant, out string tenantCode) && !string.IsNullOrWhiteSpace(tenantCode))
            {
                string sanitized = SanitizeTerminalCode(tenantCode);
                ApiSession.TerminalCode = sanitized;
                return sanitized;
            }

            if (!string.IsNullOrWhiteSpace(cfg.TerminalCode))
            {
                string sanitized = SanitizeTerminalCode(cfg.TerminalCode);
                ApiSession.TerminalCode = sanitized;
                return sanitized;
            }

            // Fallback to legacy Settings.ini if present
            string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
            string stored = INIFile.ReadValue(OfflineSettingsSection, TerminalCodeKey, "", iniPath);
            if (!string.IsNullOrWhiteSpace(stored))
            {
                string sanitized = SanitizeTerminalCode(stored);
                ApiSession.TerminalCode = sanitized;
                return sanitized;
            }

            return "T01";
        }

        public static void SaveTerminalCode(string terminalCode, string tenantId = null)
        {
            string sanitized = SanitizeTerminalCode(terminalCode);
            string resolvedTenant = OfflineStore.ResolveTenantId(tenantId);
            ApiSession.TerminalCode = sanitized;

            try
            {
                string machinePath = GetMachineConfigPath();
                var cfg = LoadOrInitMachineConfig();
                cfg.TerminalCode = sanitized;
                cfg.TenantTerminalCodes[resolvedTenant] = sanitized;
                cfg.MachineName = Environment.MachineName;
                cfg.UpdatedAtUtc = DateTime.UtcNow;
                OfflineStore.WriteJsonAtomic(machinePath, cfg);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Machine-level terminal config write fallback: " + ex.Message);
            }

            try
            {
                string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
                INIFile.WriteValue(OfflineSettingsSection, TerminalCodeKey, sanitized, iniPath);
            }
            catch { }
        }

        /// <summary>
        /// Registers this PC's unique MachineId with retailsuite-api to receive a guaranteed-unique
        /// sequential Terminal ID (T01, T02, T03, ...) for the active tenant, or validates a custom requestedCode.
        /// </summary>
        public static async Task<string> RegisterTerminalWithServerAsync(string requestedCode = null)
        {
            if (string.IsNullOrWhiteSpace(ApiSession.AccessToken))
                return GetTerminalCode();

            string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
            string apiBaseUrl = INIFile.ReadValue(ApiSettingsSection, "ApiBaseUrl", DefaultApiBaseUrl, iniPath);
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                apiBaseUrl = DefaultApiBaseUrl;

            string machineId = GetMachineId();

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiSession.AccessToken);

                if (!string.IsNullOrWhiteSpace(ApiSession.TenantIdentifier))
                    client.DefaultRequestHeaders.Add("X-Tenant-ID", ApiSession.TenantIdentifier);

                var payload = new
                {
                    machineId = machineId,
                    machineName = Environment.MachineName,
                    requestedCode = string.IsNullOrWhiteSpace(requestedCode) ? null : SanitizeTerminalCode(requestedCode)
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/terminals/register", content);
                string body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errObj = JsonConvert.DeserializeObject<HttpResponseDto<object>>(body);
                    string msg = errObj?.Metadata?.Message ?? $"Terminal registration failed ({(int)response.StatusCode}).";
                    throw new Exception(msg);
                }

                var parsed = JsonConvert.DeserializeObject<HttpResponseDto<TerminalRegisterResponseDto>>(body);
                string assigned = parsed?.Body?.TerminalCode;
                if (!string.IsNullOrWhiteSpace(assigned))
                {
                    SaveTerminalCode(assigned);
                    return assigned;
                }
            }

            return GetTerminalCode();
        }

        public static string NextVoucherNo(out string terminalCode, string tenantId = null)
        {
            terminalCode = GetTerminalCode(tenantId);
            string resolvedTenant = OfflineStore.ResolveTenantId(tenantId);
            string mutexName = "Global\\RetailSuite_OfflineSeq_" + resolvedTenant;

            bool createdNew;
            using (var mutex = new Mutex(false, mutexName, out createdNew))
            {
                bool acquired = false;
                try
                {
                    try
                    {
                        acquired = mutex.WaitOne(TimeSpan.FromSeconds(10));
                    }
                    catch (AbandonedMutexException)
                    {
                        acquired = true;
                    }

                    string seqPath = OfflineStore.GetSequencePath(resolvedTenant);
                    var state = OfflineStore.ReadJson<OfflineSequenceState>(seqPath) ?? new OfflineSequenceState
                    {
                        TerminalCode = terminalCode,
                        LastSequence = 0
                    };

                    // Integer monotonic sequence per terminal
                    state.TerminalCode = terminalCode;
                    state.LastSequence++;
                    state.UpdatedAtUtc = DateTime.UtcNow;

                    OfflineStore.WriteJsonAtomic(seqPath, state);
                    return string.Format("{0}-{1:D5}", terminalCode, state.LastSequence);
                }
                finally
                {
                    if (acquired)
                    {
                        try { mutex.ReleaseMutex(); } catch { }
                    }
                }
            }
        }
    }
}
