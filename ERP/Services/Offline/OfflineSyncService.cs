using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Services.Offline
{
    internal class OfflineSyncBatchResult
    {
        public int SucceededCount { get; set; }
        public int FailedCount { get; set; }
        public bool StoppedDueToNetwork { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }

    /// <summary>
    /// Handles connectivity detection and user-confirmed synchronization of offline sales.
    /// Never silently auto-pushes pending bills; instead notifies the UI to open the Sync Confirmation dialog.
    /// </summary>
    internal static class OfflineSyncService
    {
        private const string ApiSettingsSection = "ApiSettings";
        private const string DefaultApiBaseUrl = "https://retailer-api.bizgripsolutions.com";

        private static readonly SemaphoreSlim _syncGate = new SemaphoreSlim(1, 1);
        private static System.Threading.Timer _heartbeatTimer;
        private static int _heartbeatRunning = 0;
        private static bool _promptedForCurrentOnlineState = false;

        public static event EventHandler<int> PendingCountChanged;
        public static event EventHandler<int> ConnectivityRestoredWithPendingSales;
        public static event EventHandler<bool> ApiReachabilityChanged;

        public static bool IsLastKnownApiReachable { get; private set; } = true;

        public static bool IsNetworkException(Exception ex)
        {
            if (ex == null) return false;
            Exception cur = ex;
            while (cur != null)
            {
                if (cur is HttpRequestException ||
                    cur is TaskCanceledException ||
                    cur is TimeoutException ||
                    cur is WebException ||
                    cur is SocketException)
                {
                    return true;
                }
                cur = cur.InnerException;
            }
            return false;
        }

        public static void NotifyPendingCountChanged()
        {
            int count = OfflineStore.GetPendingCount();
            PendingCountChanged?.Invoke(null, count);
        }

        public static void StartMonitor(int intervalSeconds = 20)
        {
            StopMonitor();
            _promptedForCurrentOnlineState = false;
            int ms = Math.Max(5, intervalSeconds) * 1000;
            _heartbeatTimer = new System.Threading.Timer(async _ => await CheckConnectivityTickAsync(), null, ms, ms);
        }

        public static void StopMonitor()
        {
            var timer = Interlocked.Exchange(ref _heartbeatTimer, null);
            if (timer != null)
            {
                try { timer.Dispose(); } catch { }
            }
        }

        public static async Task<bool> IsApiReachableAsync(int timeoutSeconds = 4)
        {
            try
            {
                string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
                string apiBaseUrl = INIFile.ReadValue(ApiSettingsSection, "ApiBaseUrl", DefaultApiBaseUrl, iniPath);
                if (string.IsNullOrWhiteSpace(apiBaseUrl))
                    apiBaseUrl = DefaultApiBaseUrl;

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = delegate { return true; };

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                    using (var req = new HttpRequestMessage(HttpMethod.Get, apiBaseUrl.TrimEnd('/') + "/"))
                    {
                        var resp = await client.SendAsync(req);
                        // Any HTTP response (200, 401, 404, etc.) proves network + API server reachability
                        return resp != null;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private static async Task CheckConnectivityTickAsync()
        {
            if (Interlocked.CompareExchange(ref _heartbeatRunning, 1, 0) != 0)
                return;

            try
            {
                bool reachable = await IsApiReachableAsync(4);
                bool wasReachable = IsLastKnownApiReachable;
                IsLastKnownApiReachable = reachable;

                if (reachable != wasReachable)
                {
                    ApiReachabilityChanged?.Invoke(null, reachable);
                }

                if (!reachable)
                {
                    _promptedForCurrentOnlineState = false;
                }
                else
                {
                    int pending = OfflineStore.GetPendingCount();
                    if (pending > 0 && !_promptedForCurrentOnlineState && ApiSession.IsOfflineMode)
                    {
                        _promptedForCurrentOnlineState = true;
                        ConnectivityRestoredWithPendingSales?.Invoke(null, pending);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                Interlocked.Exchange(ref _heartbeatRunning, 0);
            }
        }

        /// <summary>
        /// Syncs ONLY the user-confirmed voucher numbers to retailsuite-api in chronological order.
        /// Requires an active authenticated online token (ApiSession.AccessToken).
        /// </summary>
        public static async Task<OfflineSyncBatchResult> SyncSelectedSalesAsync(IEnumerable<string> voucherNos)
        {
            var result = new OfflineSyncBatchResult();
            var selectedSet = new HashSet<string>(
                (voucherNos ?? Enumerable.Empty<string>()).Where(v => !string.IsNullOrWhiteSpace(v)),
                StringComparer.OrdinalIgnoreCase);

            if (selectedSet.Count == 0)
                return result;

            if (string.IsNullOrWhiteSpace(ApiSession.AccessToken))
            {
                result.Messages.Add("Online authentication is required before syncing offline bills.");
                return result;
            }

            await _syncGate.WaitAsync();
            try
            {
                var saleService = new SaleApiService();
                var pendingList = OfflineStore.GetPendingSales()
                    .Where(s => selectedSet.Contains(s.VoucherNo))
                    .OrderBy(s => s.CreatedAtLocal)
                    .ToList();

                string reviewer = !string.IsNullOrWhiteSpace(UserInfo.UserName) ? UserInfo.UserName : "Authenticated User";

                foreach (var sale in pendingList)
                {
                    try
                    {
                        if (sale.Request == null)
                        {
                            OfflineStore.MarkSaleFailed(sale, "Sale payload is missing.");
                            result.FailedCount++;
                            result.Messages.Add($"{sale.VoucherNo}: Sale payload is missing.");
                            continue;
                        }

                        sale.Request.OfflineVoucherNo = sale.VoucherNo;
                        sale.Request.Counter = string.IsNullOrWhiteSpace(sale.TerminalCode)
                            ? OfflineVoucherGenerator.GetTerminalCode()
                            : sale.TerminalCode;

                        await saleService.CreateAsync(sale.Request);
                        OfflineStore.MarkSaleSynced(sale, reviewer);
                        result.SucceededCount++;
                    }
                    catch (Exception ex)
                    {
                        if (IsNetworkException(ex))
                        {
                            result.StoppedDueToNetwork = true;
                            result.Messages.Add($"Network error while syncing {sale.VoucherNo}: {ex.Message}");
                            break;
                        }

                        OfflineStore.MarkSaleFailed(sale, ex.Message);
                        result.FailedCount++;
                        result.Messages.Add($"{sale.VoucherNo}: {ex.Message}");
                    }
                }
            }
            finally
            {
                _syncGate.Release();
                NotifyPendingCountChanged();
            }

            return result;
        }

        public static int IgnoreSelectedSales(IEnumerable<string> voucherNos, string reviewedBy)
        {
            var selectedSet = new HashSet<string>(
                (voucherNos ?? Enumerable.Empty<string>()).Where(v => !string.IsNullOrWhiteSpace(v)),
                StringComparer.OrdinalIgnoreCase);

            if (selectedSet.Count == 0)
                return 0;

            int ignoredCount = 0;
            var pendingList = OfflineStore.GetPendingSales()
                .Where(s => selectedSet.Contains(s.VoucherNo))
                .ToList();

            string reviewer = string.IsNullOrWhiteSpace(reviewedBy) ? (UserInfo.UserName ?? "User") : reviewedBy;

            foreach (var sale in pendingList)
            {
                OfflineStore.MarkSaleIgnored(sale, reviewer);
                ignoredCount++;
            }

            NotifyPendingCountChanged();
            return ignoredCount;
        }
    }
}
