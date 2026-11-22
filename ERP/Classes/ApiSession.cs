using System;

namespace ERP.Classes
{
    public static class ApiSession
    {
        private static string _tenantIdentifier;
        public static string TenantIdentifier
        {
            get => _tenantIdentifier;
            set
            {
                if (_tenantIdentifier != value)
                {
                    _tenantIdentifier = value;
                    QrPaymentInfo.ClearCache();
                }
            }
        }
        public static string AccessToken { get; set; }
        public static string RefreshToken { get; set; }
        public static DateTime? RefreshTokenExpiryTime { get; set; }
        public static string DeviceId { get; set; }
        public static string UserEmail { get; set; }
        public static bool HasSupplyFeature { get; set; } = true;
        public static bool HasSecondaryQty { get; set; } = false;
        public static bool HasVariablePackFeature { get; set; } = false;
        public static bool HasMobileShopFeature { get; set; } = false;
        public static bool EnableCarriage { get; set; } = false;
        public static bool AllowSalePriceUpdateInPurchase { get; set; } = false;
        public static bool IsBarcodeFastScanEnabled { get; set; } = false;
        public static bool FastScanAutoIncrement { get; set; } = true;
        public static bool FastScanSound { get; set; } = true;

        // Offline Mode state (kept in memory only; never stores credentials/tokens on disk)
        private static bool _isOfflineMode = false;
        public static bool IsOfflineMode => _isOfflineMode;
        public static bool NeedsOnlineReauth { get; set; } = false;
        public static string TerminalCode { get; set; } = "T01";
        public static DateTime? LastOfflineCacheSyncUtc { get; set; }

        public static event EventHandler<bool> OfflineModeChanged;

        public static void SetOfflineMode(bool isOffline)
        {
            if (_isOfflineMode == isOffline)
                return;

            _isOfflineMode = isOffline;
            OfflineModeChanged?.Invoke(null, isOffline);
        }
    }
}
