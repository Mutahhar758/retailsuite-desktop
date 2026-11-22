using System;
using System.Collections.Generic;
using ERP.Reporting.Models;
using ERP.Services.Legacy;

namespace ERP.Services.Offline
{
    /// <summary>
    /// Non-sensitive company branding, printer config, and feature flags cached for Offline POS Mode.
    /// Zero passwords, password hashes, or auth tokens are stored here.
    /// </summary>
    internal class OfflineCompanySnapshot
    {
        public string TenantIdentifier { get; set; }
        public DateTime SavedAtUtc { get; set; }

        // CompanyInfo
        public string CompanyName { get; set; }
        public string UrCompanyName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public string Cell2 { get; set; }
        public string ContactHead { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Ntn { get; set; }
        public string Strn { get; set; }

        // ConfigInfo
        public string ThankyouLine { get; set; }
        public string DefaultBillFormat { get; set; }
        public string ThermalPrinterName { get; set; }
        public string A4PrinterName { get; set; }

        // ApiSession feature flags
        public bool HasSupplyFeature { get; set; } = true;
        public bool HasSecondaryQty { get; set; }
        public bool HasVariablePackFeature { get; set; }
        public bool HasMobileShopFeature { get; set; }
        public bool EnableCarriage { get; set; }
        public bool AllowSalePriceUpdateInPurchase { get; set; }
        public bool IsBarcodeFastScanEnabled { get; set; }
        public bool FastScanAutoIncrement { get; set; } = true;
        public bool FastScanSound { get; set; } = true;
    }

    /// <summary>
    /// Snapshot of all lookup data needed to operate Touch POS, Normal Sale, and Wanda Sale offline.
    /// </summary>
    internal class OfflineCatalogCache
    {
        public string TenantIdentifier { get; set; }
        public DateTime SyncedAtUtc { get; set; }
        public List<ChartOfAccountHeadDto> Customers { get; set; } = new List<ChartOfAccountHeadDto>();
        public List<ChartOfAccountHeadDto> CashBankAccounts { get; set; } = new List<ChartOfAccountHeadDto>();
        public List<InventoryItemDto> Items { get; set; } = new List<InventoryItemDto>();
        public List<ItemCategoryDto> Categories { get; set; } = new List<ItemCategoryDto>();
        public List<UnitLookupDto> Units { get; set; } = new List<UnitLookupDto>();
        public List<NarrationDto> Narrations { get; set; } = new List<NarrationDto>();
        public List<ItemCustomerCategoryPriceDto> CustomerCategoryPrices { get; set; } = new List<ItemCustomerCategoryPriceDto>();
    }

    internal class OfflineSequenceState
    {
        public string TerminalCode { get; set; }
        public long LastSequence { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }

    internal enum OfflineSaleStatus
    {
        Pending = 0,
        Failed = 1,
        Synced = 2,
        Ignored = 3
    }

    /// <summary>
    /// A single offline sale bill persisted locally for review, receipt reprinting, and sync/ignore.
    /// </summary>
    internal class OfflineQueuedSale
    {
        public string VoucherNo { get; set; }
        public string TerminalCode { get; set; }
        public string SourceForm { get; set; } // "TouchPOS", "NormalSale", "WandaSale"
        public DateTime CreatedAtLocal { get; set; }
        public string CashierName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string NarrationTitle { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CashReceipt { get; set; }
        public decimal CashBack { get; set; }

        public SaleCreateRequest Request { get; set; }
        public List<SaleReceiptLineItem> ReceiptLines { get; set; } = new List<SaleReceiptLineItem>();

        public OfflineSaleStatus Status { get; set; } = OfflineSaleStatus.Pending;
        public int RetryCount { get; set; }
        public string LastError { get; set; }
        public DateTime? SyncedAtUtc { get; set; }
        public DateTime? IgnoredAtUtc { get; set; }
        public string ReviewedBy { get; set; }

        public SaleReceiptDataResult ToReceiptDataResult(bool isDuplicate = false)
        {
            var summary = new SaleReceiptSummary
            {
                VoucherNo = !string.IsNullOrWhiteSpace(VoucherNo) && VoucherNo.StartsWith("SL-", StringComparison.OrdinalIgnoreCase)
                    ? VoucherNo
                    : ("SL-" + VoucherNo),
                Date = CreatedAtLocal != DateTime.MinValue ? CreatedAtLocal : DateTime.Now,
                CustomerName = string.IsNullOrWhiteSpace(CustomerName) ? "Walk-in Customer" : CustomerName,
                CustomerAddress = CustomerAddress ?? string.Empty,
                Narration = NarrationTitle ?? string.Empty,
                Description = Request?.Description ?? string.Empty,
                Cashier = string.IsNullOrWhiteSpace(CashierName) ? (UserInfo.UserName ?? "Cashier") : CashierName,
                CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                CompanyAddress = CompanyInfo.Address ?? string.Empty,
                CompanyPhone = CompanyInfo.ContactHead ?? string.Empty,
                ThankyouLine = !string.IsNullOrWhiteSpace(ConfigInfo.ThankyouLine) ? ConfigInfo.ThankyouLine : "Thank you for your business!",
                GrossTotal = GrossTotal,
                TotalDiscount = TotalDiscount,
                NetAmount = NetAmount,
                CashReceipt = CashReceipt,
                CashBack = CashBack,
                Balance = Math.Max(0m, NetAmount - (CashReceipt - CashBack)),
                IsDuplicate = isDuplicate
            };

            return new SaleReceiptDataResult(summary, ReceiptLines ?? new List<SaleReceiptLineItem>());
        }
    }
}
