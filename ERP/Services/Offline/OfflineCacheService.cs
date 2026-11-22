using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Services.Offline
{
    /// <summary>
    /// Manages caching of non-sensitive company/settings metadata and POS catalog lookups,
    /// and hydrates offline POS sessions without ever storing credentials or auth tokens.
    /// </summary>
    internal static class OfflineCacheService
    {
        private static readonly SemaphoreSlim _refreshGate = new SemaphoreSlim(1, 1);

        public static void SaveCompanyAndSettingsSnapshot()
        {
            try
            {
                var snapshot = new OfflineCompanySnapshot
                {
                    TenantIdentifier = OfflineStore.ResolveTenantId(),
                    SavedAtUtc = DateTime.UtcNow,

                    CompanyName = CompanyInfo.CompanyName,
                    UrCompanyName = CompanyInfo.UrCompanyName,
                    Description = CompanyInfo.Description,
                    Address = CompanyInfo.Address,
                    Phone = CompanyInfo.Phone,
                    Cell = CompanyInfo.Cell,
                    Cell2 = CompanyInfo.Cell2,
                    ContactHead = CompanyInfo.ContactHead,
                    Email = CompanyInfo.Email,
                    Website = CompanyInfo.Website,
                    Ntn = CompanyInfo.Ntn,
                    Strn = CompanyInfo.Strn,

                    ThankyouLine = ConfigInfo.ThankyouLine,
                    DefaultBillFormat = ConfigInfo.DefaultBillFormat,
                    ThermalPrinterName = ConfigInfo.ThermalPrinterName,
                    A4PrinterName = ConfigInfo.A4PrinterName,

                    HasSupplyFeature = ApiSession.HasSupplyFeature,
                    HasSecondaryQty = ApiSession.HasSecondaryQty,
                    HasVariablePackFeature = ApiSession.HasVariablePackFeature,
                    HasMobileShopFeature = ApiSession.HasMobileShopFeature,
                    EnableCarriage = ApiSession.EnableCarriage,
                    AllowSalePriceUpdateInPurchase = ApiSession.AllowSalePriceUpdateInPurchase,
                    IsBarcodeFastScanEnabled = ApiSession.IsBarcodeFastScanEnabled,
                    FastScanAutoIncrement = ApiSession.FastScanAutoIncrement,
                    FastScanSound = ApiSession.FastScanSound
                };

                OfflineStore.SaveCompanySnapshot(snapshot);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SaveCompanyAndSettingsSnapshot failed: " + ex.Message);
            }
        }

        public static async Task<OfflineCatalogCache> RefreshCatalogAsync()
        {
            if (ApiSession.IsOfflineMode || string.IsNullOrWhiteSpace(ApiSession.AccessToken))
                return null;

            if (!await _refreshGate.WaitAsync(0))
                return null;

            try
            {
                var chartService = new ChartOfAccountApiService();
                var inventoryService = new InventoryApiService();
                var categoryService = new ItemCategoryApiService();
                var unitService = new UnitApiService();
                var narrationService = new NarrationApiService();

                var customersTask = chartService.GetCustomerAccountsAsync();
                var cashBankTask = chartService.GetCashBankAccountsAsync();
                var itemsTask = inventoryService.GetLookupAsync(null);
                var categoriesTask = categoryService.GetLookupAsync();
                var unitsTask = unitService.GetLookupAsync();
                var narrationsTask = narrationService.GetLookupAsync();
                var catPricesTask = inventoryService.GetCustomerCategoryPricesAsync();

                await Task.WhenAll(customersTask, cashBankTask, itemsTask, categoriesTask, unitsTask, narrationsTask, catPricesTask);

                var cache = new OfflineCatalogCache
                {
                    TenantIdentifier = OfflineStore.ResolveTenantId(),
                    SyncedAtUtc = DateTime.UtcNow,
                    Customers = customersTask.Result ?? new List<ChartOfAccountHeadDto>(),
                    CashBankAccounts = cashBankTask.Result ?? new List<ChartOfAccountHeadDto>(),
                    Items = itemsTask.Result ?? new List<InventoryItemDto>(),
                    Categories = categoriesTask.Result ?? new List<ItemCategoryDto>(),
                    Units = unitsTask.Result ?? new List<UnitLookupDto>(),
                    Narrations = narrationsTask.Result ?? new List<NarrationDto>(),
                    CustomerCategoryPrices = catPricesTask.Result ?? new List<ItemCustomerCategoryPriceDto>()
                };

                OfflineStore.SaveCatalogCache(cache);
                ApiSession.LastOfflineCacheSyncUtc = cache.SyncedAtUtc;
                SaveCompanyAndSettingsSnapshot();
                return cache;
            }
            finally
            {
                _refreshGate.Release();
            }
        }

        public static OfflineCatalogCache LoadCatalog(string tenantId = null)
        {
            var cache = OfflineStore.LoadCatalogCache(tenantId);
            if (cache != null)
            {
                ApiSession.LastOfflineCacheSyncUtc = cache.SyncedAtUtc;
            }
            return cache;
        }

        /// <summary>
        /// Hydrates an unauthenticated Offline POS-only session using previously cached non-sensitive company and catalog data.
        /// Grants only Sales View/Create in memory; syncing back to the server still requires a real online login.
        /// </summary>
        public static bool TryStartOfflinePosSession(string tenantId, string cashierDisplayName, out string errorMessage)
        {
            errorMessage = null;
            string resolvedTenant = OfflineStore.ResolveTenantId(tenantId);

            if (!OfflineStore.HasCatalogCache(resolvedTenant))
            {
                errorMessage = "No offline product catalog is cached for this organization yet.\n\nPlease log in online once on this PC so items, customers, and company settings can be downloaded for offline POS use.";
                return false;
            }

            var catalog = OfflineStore.LoadCatalogCache(resolvedTenant);
            if (catalog == null || catalog.Items == null || catalog.Items.Count == 0)
            {
                errorMessage = "The local offline catalog is empty or unreadable. Please log in online once to refresh the catalog.";
                return false;
            }

            ApiSession.TenantIdentifier = resolvedTenant;
            ApiSession.AccessToken = null;
            ApiSession.RefreshToken = null;
            ApiSession.RefreshTokenExpiryTime = null;
            ApiSession.LastOfflineCacheSyncUtc = catalog.SyncedAtUtc;
            ApiSession.TerminalCode = OfflineVoucherGenerator.GetTerminalCode();

            var snapshot = OfflineStore.LoadCompanySnapshot(resolvedTenant);
            if (snapshot != null)
            {
                CompanyInfo.CompanyName = snapshot.CompanyName;
                CompanyInfo.UrCompanyName = snapshot.UrCompanyName;
                CompanyInfo.Description = snapshot.Description;
                CompanyInfo.Address = snapshot.Address;
                CompanyInfo.Phone = snapshot.Phone;
                CompanyInfo.Cell = snapshot.Cell;
                CompanyInfo.Cell2 = snapshot.Cell2;
                CompanyInfo.ContactHead = snapshot.ContactHead ?? "";
                CompanyInfo.Email = snapshot.Email;
                CompanyInfo.Website = snapshot.Website;
                CompanyInfo.Ntn = snapshot.Ntn;
                CompanyInfo.Strn = snapshot.Strn;

                ConfigInfo.Reset();
                if (!string.IsNullOrWhiteSpace(snapshot.ThankyouLine)) ConfigInfo.ThankyouLine = snapshot.ThankyouLine;
                if (!string.IsNullOrWhiteSpace(snapshot.DefaultBillFormat)) ConfigInfo.DefaultBillFormat = snapshot.DefaultBillFormat;
                if (!string.IsNullOrWhiteSpace(snapshot.ThermalPrinterName)) ConfigInfo.ThermalPrinterName = snapshot.ThermalPrinterName;
                if (!string.IsNullOrWhiteSpace(snapshot.A4PrinterName)) ConfigInfo.A4PrinterName = snapshot.A4PrinterName;

                ApiSession.HasSupplyFeature = snapshot.HasSupplyFeature;
                ApiSession.HasSecondaryQty = snapshot.HasSecondaryQty;
                ApiSession.HasVariablePackFeature = snapshot.HasVariablePackFeature;
                ApiSession.HasMobileShopFeature = snapshot.HasMobileShopFeature;
                ApiSession.EnableCarriage = snapshot.EnableCarriage;
                ApiSession.AllowSalePriceUpdateInPurchase = snapshot.AllowSalePriceUpdateInPurchase;
                ApiSession.IsBarcodeFastScanEnabled = snapshot.IsBarcodeFastScanEnabled;
                ApiSession.FastScanAutoIncrement = snapshot.FastScanAutoIncrement;
                ApiSession.FastScanSound = snapshot.FastScanSound;
            }

            string cashier = string.IsNullOrWhiteSpace(cashierDisplayName) ? "Offline Cashier" : cashierDisplayName.Trim();
            UserInfo.UserId = cashier;
            UserInfo.UserName = cashier;
            UserInfo.Email = string.Empty;
            UserInfo.IsOwner = false;
            UserInfo.LogInDateTime = DateTime.Now;
            UserInfo.Permissions = new List<string>
            {
                $"Permissions.{AppResource.Sales}.{AppAction.View}",
                $"Permissions.{AppResource.Sales}.{AppAction.Create}",
                $"Permissions.{AppResource.PrinterSettings}.{AppAction.View}"
            };

            ApiSession.NeedsOnlineReauth = true;
            ApiSession.SetOfflineMode(true);
            return true;
        }
    }
}
