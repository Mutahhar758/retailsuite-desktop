using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class DailyStockClosingApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/daily-stock-closings";

        public async Task<List<DailyStockClosingListItemDto>> GetListAsync(string fromDate = "", string toDate = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<DailyStockClosingListItemDto>>>(json);
                return payload?.Body ?? new List<DailyStockClosingListItemDto>();
            }
        }

        public async Task<DailyStockClosingSheetDto> GetSheetAsync(string date, string categoryCode = "")
        {
            var url = Endpoint + "/sheet?date=" + Uri.EscapeDataString(date);
            if (!string.IsNullOrWhiteSpace(categoryCode))
            {
                url += "&categoryCode=" + Uri.EscapeDataString(categoryCode);
            }

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<DailyStockClosingSheetDto>>(json);
                return payload?.Body ?? new DailyStockClosingSheetDto();
            }
        }

        public async Task<DailyStockClosingSaveResponseDto> SaveAsync(DailyStockClosingSaveRequestDto request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<DailyStockClosingSaveResponseDto>>(json);
                return payload?.Body ?? new DailyStockClosingSaveResponseDto();
            }
        }

        public async Task DeleteAsync(string date)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(date));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class DailyStockClosingListItemDto
    {
        [JsonProperty("closingKey")]
        public string ClosingKey { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("purchaseVoucherNo")]
        public string PurchaseVoucherNo { get; set; }

        [JsonProperty("purchaseAmount")]
        public decimal PurchaseAmount { get; set; }

        [JsonProperty("saleVoucherNo")]
        public string SaleVoucherNo { get; set; }

        [JsonProperty("saleAmount")]
        public decimal SaleAmount { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }
    }

    internal class DailyStockClosingSheetDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("isExisting")]
        public bool IsExisting { get; set; }

        [JsonProperty("purchaseVoucherNo")]
        public string PurchaseVoucherNo { get; set; }

        [JsonProperty("saleVoucherNo")]
        public string SaleVoucherNo { get; set; }

        [JsonProperty("customerAccountId")]
        public string CustomerAccountId { get; set; }

        [JsonProperty("vendorAccountId")]
        public string VendorAccountId { get; set; }

        [JsonProperty("cashAccountId")]
        public string CashAccountId { get; set; }

        [JsonProperty("reconciliationMode")]
        public string ReconciliationMode { get; set; } = "DiscountAddLess";

        [JsonProperty("enteredSaleAmount")]
        public decimal? EnteredSaleAmount { get; set; }

        [JsonProperty("enteredPurchaseAmount")]
        public decimal? EnteredPurchaseAmount { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("items")]
        public List<DailyStockClosingItemLineDto> Items { get; set; } = new List<DailyStockClosingItemLineDto>();
    }

    internal class DailyStockClosingItemLineDto
    {
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemKey")]
        public string ItemKey { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("categoryCode")]
        public string CategoryCode { get; set; }

        [JsonProperty("categoryTitle")]
        public string CategoryTitle { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("unitTitle")]
        public string UnitTitle { get; set; }

        [JsonProperty("openingQty")]
        public decimal OpeningQty { get; set; }

        [JsonProperty("closingQty")]
        public decimal ClosingQty { get; set; }

        [JsonProperty("purchasedQty")]
        public decimal PurchasedQty { get; set; }

        [JsonProperty("soldQty")]
        public decimal SoldQty { get; set; }

        [JsonProperty("purchaseRate")]
        public decimal PurchaseRate { get; set; }

        [JsonProperty("saleRate")]
        public decimal SaleRate { get; set; }

        [JsonIgnore]
        public bool ManualPurchased { get; set; }
    }

    internal class DailyStockClosingSaveRequestDto
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("customerAccountId")]
        public string CustomerAccountId { get; set; }

        [JsonProperty("vendorAccountId")]
        public string VendorAccountId { get; set; }

        [JsonProperty("cashAccountId")]
        public string CashAccountId { get; set; }

        [JsonProperty("reconciliationMode")]
        public string ReconciliationMode { get; set; } = "DiscountAddLess";

        [JsonProperty("enteredSaleAmount")]
        public decimal? EnteredSaleAmount { get; set; }

        [JsonProperty("enteredPurchaseAmount")]
        public decimal? EnteredPurchaseAmount { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("lines")]
        public List<DailyStockClosingSaveLineRequestDto> Lines { get; set; } = new List<DailyStockClosingSaveLineRequestDto>();
    }

    internal class DailyStockClosingSaveLineRequestDto
    {
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("openingQty")]
        public decimal OpeningQty { get; set; }

        [JsonProperty("closingQty")]
        public decimal ClosingQty { get; set; }

        [JsonProperty("purchasedQty")]
        public decimal PurchasedQty { get; set; }

        [JsonProperty("soldQty")]
        public decimal SoldQty { get; set; }

        [JsonProperty("purchaseRate")]
        public decimal PurchaseRate { get; set; }

        [JsonProperty("saleRate")]
        public decimal SaleRate { get; set; }
    }

    internal class DailyStockClosingSaveResponseDto
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("purchaseVoucherNo")]
        public string PurchaseVoucherNo { get; set; }

        [JsonProperty("saleVoucherNo")]
        public string SaleVoucherNo { get; set; }

        [JsonProperty("totalPurchaseAmount")]
        public decimal TotalPurchaseAmount { get; set; }

        [JsonProperty("totalSaleAmount")]
        public decimal TotalSaleAmount { get; set; }

        [JsonProperty("cashReceived")]
        public decimal CashReceived { get; set; }

        [JsonProperty("cashPaid")]
        public decimal CashPaid { get; set; }
    }
}
