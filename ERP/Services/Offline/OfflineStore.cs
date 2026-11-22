using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ERP.Classes;
using Newtonsoft.Json;

namespace ERP.Services.Offline
{
    /// <summary>
    /// Tenant-scoped atomic JSON file store under AppData/OfflineData/&lt;TenantId&gt;/.
    /// Uses temp-file + atomic move so abrupt power loss cannot corrupt queued sales or sequences.
    /// Zero native dependencies (ClickOnce-safe) and zero credentials/tokens stored on disk.
    /// </summary>
    internal static class OfflineStore
    {
        private static readonly object _ioLock = new object();

        public static string ResolveTenantId(string tenantId = null)
        {
            string resolved = !string.IsNullOrWhiteSpace(tenantId)
                ? tenantId
                : (!string.IsNullOrWhiteSpace(ApiSession.TenantIdentifier)
                    ? ApiSession.TenantIdentifier
                    : new Authentication.LicenseService().GetStoredTenantIdentifier());

            if (string.IsNullOrWhiteSpace(resolved))
                resolved = "default";

            foreach (char c in Path.GetInvalidFileNameChars())
                resolved = resolved.Replace(c, '_');

            return resolved.Trim();
        }

        public static string GetTenantRootDir(string tenantId = null)
        {
            string root = Path.Combine(INIFile.GetAppDataDirectory(), "OfflineData", ResolveTenantId(tenantId));
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            return root;
        }

        private static string GetQueueDir(string folderName, string tenantId = null)
        {
            string dir = Path.Combine(GetTenantRootDir(tenantId), "queue", folderName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetCompanySnapshotPath(string tenantId = null)
            => Path.Combine(GetTenantRootDir(tenantId), "company_cache.json");

        public static string GetCatalogCachePath(string tenantId = null)
            => Path.Combine(GetTenantRootDir(tenantId), "catalog_cache.json");

        public static string GetSequencePath(string tenantId = null)
            => Path.Combine(GetTenantRootDir(tenantId), "sequence.json");

        public static void WriteJsonAtomic<T>(string path, T data)
        {
            lock (_ioLock)
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(tempPath, json, Encoding.UTF8);

                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, null);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
        }

        public static T ReadJson<T>(string path) where T : class
        {
            lock (_ioLock)
            {
                if (!File.Exists(path))
                    return null;

                try
                {
                    string json = File.ReadAllText(path, Encoding.UTF8);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static void SaveCompanySnapshot(OfflineCompanySnapshot snapshot, string tenantId = null)
            => WriteJsonAtomic(GetCompanySnapshotPath(tenantId), snapshot);

        public static OfflineCompanySnapshot LoadCompanySnapshot(string tenantId = null)
            => ReadJson<OfflineCompanySnapshot>(GetCompanySnapshotPath(tenantId));

        public static void SaveCatalogCache(OfflineCatalogCache cache, string tenantId = null)
            => WriteJsonAtomic(GetCatalogCachePath(tenantId), cache);

        public static OfflineCatalogCache LoadCatalogCache(string tenantId = null)
            => ReadJson<OfflineCatalogCache>(GetCatalogCachePath(tenantId));

        public static bool HasCatalogCache(string tenantId = null)
            => File.Exists(GetCatalogCachePath(tenantId));

        private static string SanitizeFileName(string voucherNo)
        {
            string safe = (voucherNo ?? "unknown").Trim();
            foreach (char c in Path.GetInvalidFileNameChars())
                safe = safe.Replace(c, '_');
            return safe + ".json";
        }

        public static void EnqueueSale(OfflineQueuedSale sale, string tenantId = null)
        {
            if (sale == null || string.IsNullOrWhiteSpace(sale.VoucherNo))
                throw new ArgumentException("Offline sale must have a voucher number.");

            string path = Path.Combine(GetQueueDir("pending", tenantId), SanitizeFileName(sale.VoucherNo));
            WriteJsonAtomic(path, sale);
        }

        public static OfflineQueuedSale GetPendingSale(string voucherNo, string tenantId = null)
        {
            if (string.IsNullOrWhiteSpace(voucherNo)) return null;
            string clean = voucherNo.StartsWith("SL-", StringComparison.OrdinalIgnoreCase) ? voucherNo.Substring(3) : voucherNo;
            string pendingPath = Path.Combine(GetQueueDir("pending", tenantId), SanitizeFileName(clean));
            var found = ReadJson<OfflineQueuedSale>(pendingPath);
            if (found != null) return found;

            string syncedPath = Path.Combine(GetQueueDir("synced", tenantId), SanitizeFileName(clean));
            found = ReadJson<OfflineQueuedSale>(syncedPath);
            if (found != null) return found;

            string ignoredPath = Path.Combine(GetQueueDir("ignored", tenantId), SanitizeFileName(clean));
            return ReadJson<OfflineQueuedSale>(ignoredPath);
        }

        public static List<OfflineQueuedSale> GetSalesByFolder(string folderName, string tenantId = null)
        {
            lock (_ioLock)
            {
                var list = new List<OfflineQueuedSale>();
                string dir = GetQueueDir(folderName, tenantId);
                if (!Directory.Exists(dir)) return list;

                foreach (string file in Directory.GetFiles(dir, "*.json"))
                {
                    try
                    {
                        var item = JsonConvert.DeserializeObject<OfflineQueuedSale>(File.ReadAllText(file, Encoding.UTF8));
                        if (item != null)
                            list.Add(item);
                    }
                    catch
                    {
                    }
                }

                return list.OrderBy(x => x.CreatedAtLocal).ThenBy(x => x.VoucherNo).ToList();
            }
        }

        public static List<OfflineQueuedSale> GetPendingSales(string tenantId = null)
            => GetSalesByFolder("pending", tenantId);

        public static List<OfflineQueuedSale> GetSyncedSales(string tenantId = null)
            => GetSalesByFolder("synced", tenantId).OrderByDescending(x => x.SyncedAtUtc ?? x.CreatedAtLocal).ToList();

        public static List<OfflineQueuedSale> GetIgnoredSales(string tenantId = null)
            => GetSalesByFolder("ignored", tenantId).OrderByDescending(x => x.IgnoredAtUtc ?? x.CreatedAtLocal).ToList();

        public static int GetPendingCount(string tenantId = null)
        {
            lock (_ioLock)
            {
                string dir = GetQueueDir("pending", tenantId);
                return Directory.Exists(dir) ? Directory.GetFiles(dir, "*.json").Length : 0;
            }
        }

        public static void MarkSaleSynced(OfflineQueuedSale sale, string reviewedBy, string tenantId = null)
        {
            lock (_ioLock)
            {
                sale.Status = OfflineSaleStatus.Synced;
                sale.LastError = null;
                sale.SyncedAtUtc = DateTime.UtcNow;
                sale.ReviewedBy = reviewedBy;

                string fileName = SanitizeFileName(sale.VoucherNo);
                string pendingPath = Path.Combine(GetQueueDir("pending", tenantId), fileName);
                string syncedPath = Path.Combine(GetQueueDir("synced", tenantId), fileName);

                WriteJsonAtomic(syncedPath, sale);
                if (File.Exists(pendingPath))
                    File.Delete(pendingPath);
            }
        }

        public static void MarkSaleIgnored(OfflineQueuedSale sale, string reviewedBy, string tenantId = null)
        {
            lock (_ioLock)
            {
                sale.Status = OfflineSaleStatus.Ignored;
                sale.IgnoredAtUtc = DateTime.UtcNow;
                sale.ReviewedBy = reviewedBy;

                string fileName = SanitizeFileName(sale.VoucherNo);
                string pendingPath = Path.Combine(GetQueueDir("pending", tenantId), fileName);
                string ignoredPath = Path.Combine(GetQueueDir("ignored", tenantId), fileName);

                WriteJsonAtomic(ignoredPath, sale);
                if (File.Exists(pendingPath))
                    File.Delete(pendingPath);
            }
        }

        public static void MarkSaleFailed(OfflineQueuedSale sale, string errorMessage, string tenantId = null)
        {
            lock (_ioLock)
            {
                sale.Status = OfflineSaleStatus.Failed;
                sale.RetryCount++;
                sale.LastError = errorMessage;

                string fileName = SanitizeFileName(sale.VoucherNo);
                string pendingPath = Path.Combine(GetQueueDir("pending", tenantId), fileName);
                WriteJsonAtomic(pendingPath, sale);
            }
        }
    }
}
