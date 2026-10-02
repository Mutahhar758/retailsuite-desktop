using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class MobileShopApiService : ApiServiceBase
    {
        public async Task<List<ImeiStockDto>> GetAvailableImeisAsync(string itemId = null)
        {
            using (var client = CreateClient())
            {
                var url = "/api/imei/available";
                if (!string.IsNullOrWhiteSpace(itemId))
                    url += "?itemId=" + Uri.EscapeDataString(itemId);

                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ImeiStockDto>>>(json);
                return payload?.Body ?? new List<ImeiStockDto>();
            }
        }

        public async Task<ImeiHistoryDto> GetImeiHistoryAsync(string imei)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync("/api/imei/history/" + Uri.EscapeDataString(imei));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<ImeiHistoryDto>>(json);
                return payload?.Body;
            }
        }

        public async Task<long> AddImeiCostAsync(ImeiCostAdditionRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/imei/cost-addition", content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                long.TryParse(payload?.Body, out var id);
                return id;
            }
        }

        public async Task<List<RepairJobDto>> GetRepairJobsAsync(
            string fromDate = "", string toDate = "",
            string status = "", string search = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(status)) qs.Add("status=" + Uri.EscapeDataString(status));
            if (!string.IsNullOrWhiteSpace(search)) qs.Add("search=" + Uri.EscapeDataString(search));

            var url = "/api/repair-jobs" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<RepairJobDto>>>(json);
                return payload?.Body ?? new List<RepairJobDto>();
            }
        }

        public async Task<RepairJobDto> GetRepairJobDetailAsync(string jobNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync("/api/repair-jobs/" + Uri.EscapeDataString(jobNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<RepairJobDto>>(json);
                return payload?.Body;
            }
        }

        public async Task<string> CreateRepairJobAsync(RepairJobCreateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/repair-jobs", content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateRepairJobAsync(string jobNo, RepairJobUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("/api/repair-jobs/" + Uri.EscapeDataString(jobNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task UpdateRepairJobStatusAsync(string jobNo, RepairJobStatusUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/repair-jobs/" + Uri.EscapeDataString(jobNo) + "/status", content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<string> BillRepairJobAsync(string jobNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.PostAsync("/api/repair-jobs/" + Uri.EscapeDataString(jobNo) + "/bill", null);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }
    }

    internal class ImeiStockDto
    {
        [JsonProperty("imei")]
        public string Imei { get; set; }

        [JsonProperty("imei2")]
        public string Imei2 { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        [JsonProperty("brandTitle")]
        public string BrandTitle { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("storage")]
        public string Storage { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("ptaStatus")]
        public string PtaStatus { get; set; }

        [JsonProperty("conditionNote")]
        public string ConditionNote { get; set; }

        [JsonProperty("batteryHealth")]
        public int? BatteryHealth { get; set; }

        [JsonProperty("purchaseVoucherNo")]
        public string PurchaseVoucherNo { get; set; }

        [JsonProperty("purchaseDate")]
        public DateTime PurchaseDate { get; set; }

        [JsonProperty("purchaseCost")]
        public decimal PurchaseCost { get; set; }

        [JsonProperty("addedCost")]
        public decimal AddedCost { get; set; }

        [JsonProperty("totalCost")]
        public decimal TotalCost { get; set; }

        [JsonProperty("sellingPrice")]
        public decimal SellingPrice { get; set; }

        [JsonProperty("sellerCnic")]
        public string SellerCnic { get; set; }

        [JsonProperty("sellerContact")]
        public string SellerContact { get; set; }
    }

    internal class ImeiHistoryDto
    {
        [JsonProperty("imei")]
        public string Imei { get; set; }

        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        [JsonProperty("brandTitle")]
        public string BrandTitle { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("isCurrentlyInStock")]
        public bool IsCurrentlyInStock { get; set; }

        [JsonProperty("totalCost")]
        public decimal TotalCost { get; set; }

        [JsonProperty("timeline")]
        public List<ImeiTimelineEventDto> Timeline { get; set; } = new List<ImeiTimelineEventDto>();

        [JsonProperty("costAdditions")]
        public List<ImeiCostAdditionDto> CostAdditions { get; set; } = new List<ImeiCostAdditionDto>();
    }

    internal class ImeiTimelineEventDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("partyAccount")]
        public string PartyAccount { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("ptaStatus")]
        public string PtaStatus { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    internal class ImeiCostAdditionDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("costType")]
        public string CostType { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("accountTitle")]
        public string AccountTitle { get; set; }
    }

    internal class ImeiCostAdditionRequest
    {
        [JsonProperty("imei")]
        public string Imei { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("costType")]
        public string CostType { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("paidFromAccountId")]
        public string PaidFromAccountId { get; set; }
    }

    internal class RepairJobDto
    {
        [JsonProperty("jobNo")]
        public string JobNo { get; set; }

        [JsonProperty("jobDate")]
        public DateTime JobDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("customerAccountId")]
        public string CustomerAccountId { get; set; }

        [JsonProperty("customerAccountTitle")]
        public string CustomerAccountTitle { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("brandTitle")]
        public string BrandTitle { get; set; }

        [JsonProperty("brandId")]
        public string BrandId { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("imei")]
        public string Imei { get; set; }

        [JsonProperty("passcode")]
        public string Passcode { get; set; }

        [JsonProperty("problemDescription")]
        public string ProblemDescription { get; set; }

        [JsonProperty("physicalCondition")]
        public string PhysicalCondition { get; set; }

        [JsonProperty("accessoriesReceived")]
        public string AccessoriesReceived { get; set; }

        [JsonProperty("technicianNotes")]
        public string TechnicianNotes { get; set; }

        [JsonProperty("technicianId")]
        public string TechnicianId { get; set; }

        [JsonProperty("technicianName")]
        public string TechnicianName { get; set; }

        [JsonProperty("estimatedCost")]
        public decimal EstimatedCost { get; set; }

        [JsonProperty("advanceReceived")]
        public decimal AdvanceReceived { get; set; }

        [JsonProperty("serviceCharges")]
        public decimal ServiceCharges { get; set; }

        [JsonProperty("partsTotal")]
        public decimal PartsTotal { get; set; }

        [JsonProperty("netTotal")]
        public decimal NetTotal { get; set; }

        [JsonProperty("balanceDue")]
        public decimal BalanceDue { get; set; }

        [JsonProperty("saleVoucherNo")]
        public string SaleVoucherNo { get; set; }

        [JsonProperty("parts")]
        public List<RepairJobPartDto> Parts { get; set; } = new List<RepairJobPartDto>();

        [JsonProperty("services")]
        public List<RepairJobServiceItemDto> Services { get; set; } = new List<RepairJobServiceItemDto>();
    }

    internal class RepairJobPartDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("totalPrice")]
        public decimal TotalPrice { get; set; }
    }

    internal class RepairJobServiceItemDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    internal class RepairJobCreateRequest
    {
        [JsonProperty("jobDate")]
        public string JobDate { get; set; }

        [JsonProperty("customerAccountId")]
        public string CustomerAccountId { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("brandId")]
        public string BrandId { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("imei")]
        public string Imei { get; set; }

        [JsonProperty("passcode")]
        public string Passcode { get; set; }

        [JsonProperty("problemDescription")]
        public string ProblemDescription { get; set; }

        [JsonProperty("physicalCondition")]
        public string PhysicalCondition { get; set; }

        [JsonProperty("accessoriesReceived")]
        public string AccessoriesReceived { get; set; }

        [JsonProperty("technicianNotes")]
        public string TechnicianNotes { get; set; }

        [JsonProperty("technicianId")]
        public string TechnicianId { get; set; }

        [JsonProperty("estimatedCost")]
        public decimal EstimatedCost { get; set; }

        [JsonProperty("advanceReceived")]
        public decimal AdvanceReceived { get; set; }

        [JsonProperty("advanceAccountId")]
        public string AdvanceAccountId { get; set; }

        [JsonProperty("parts")]
        public List<RepairJobPartRequest> Parts { get; set; } = new List<RepairJobPartRequest>();

        [JsonProperty("services")]
        public List<RepairJobServiceItemRequest> Services { get; set; } = new List<RepairJobServiceItemRequest>();
    }

    internal class RepairJobUpdateRequest
    {
        [JsonProperty("customerAccountId")]
        public string CustomerAccountId { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("brandId")]
        public string BrandId { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("imei")]
        public string Imei { get; set; }

        [JsonProperty("passcode")]
        public string Passcode { get; set; }

        [JsonProperty("problemDescription")]
        public string ProblemDescription { get; set; }

        [JsonProperty("physicalCondition")]
        public string PhysicalCondition { get; set; }

        [JsonProperty("accessoriesReceived")]
        public string AccessoriesReceived { get; set; }

        [JsonProperty("technicianNotes")]
        public string TechnicianNotes { get; set; }

        [JsonProperty("technicianId")]
        public string TechnicianId { get; set; }

        [JsonProperty("estimatedCost")]
        public decimal EstimatedCost { get; set; }

        [JsonProperty("parts")]
        public List<RepairJobPartRequest> Parts { get; set; } = new List<RepairJobPartRequest>();

        [JsonProperty("services")]
        public List<RepairJobServiceItemRequest> Services { get; set; } = new List<RepairJobServiceItemRequest>();
    }

    internal class RepairJobStatusUpdateRequest
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    internal class RepairJobPartRequest
    {
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }
    }

    internal class RepairJobServiceItemRequest
    {
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
