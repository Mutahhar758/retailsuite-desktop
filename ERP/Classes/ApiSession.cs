using System;

namespace ERP.Classes
{
    public static class ApiSession
    {
        public static string TenantIdentifier { get; set; }
        public static string AccessToken { get; set; }
        public static string RefreshToken { get; set; }
        public static DateTime? RefreshTokenExpiryTime { get; set; }
        public static string DeviceId { get; set; }
        public static string UserEmail { get; set; }
        public static bool HasSupplyFeature { get; set; } = true;
        public static bool HasSecondaryQty { get; set; } = false;
    }
}
