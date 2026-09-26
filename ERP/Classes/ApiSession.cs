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
    }
}
