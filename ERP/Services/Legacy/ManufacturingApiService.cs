using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class ManufacturingApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/manufacturing";

        public async Task<ProductionPageResult> GetPagedListAsync(
            string fromDate = "", string toDate = "",
            string status = "", string finishedItemId = "", string voucherNo = "",
            int pageNumber = 1, int pageSize = 20)
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(status)) qs.Add("status=" + Uri.EscapeDataString(status));
            if (!string.IsNullOrWhiteSpace(finishedItemId)) qs.Add("finishedItemId=" + Uri.EscapeDataString(finishedItemId));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));
            if (pageNumber > 0) qs.Add("pageNumber=" + pageNumber);
            if (pageSize > 0) qs.Add("pageSize=" + pageSize);

            var url = Endpoint + "/paged" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<ProductionPagedBody>>(json);
                var body = payload?.Body;
                var meta = body?.Metadata;
                return new ProductionPageResult
                {
                    Data = body?.Data ?? new List<ProductionMasterDto>(),
                    CurrentPage = meta?.CurrentPage ?? pageNumber,
                    TotalPages = meta?.TotalPages ?? 1,
                    TotalCount = meta?.TotalCount ?? 0,
                    PageSize = meta?.PageSize ?? pageSize
                };
            }
        }

        public async Task<List<ProductionMasterDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string status = "", string finishedItemId = "", string voucherNo = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(status)) qs.Add("status=" + Uri.EscapeDataString(status));
            if (!string.IsNullOrWhiteSpace(finishedItemId)) qs.Add("finishedItemId=" + Uri.EscapeDataString(finishedItemId));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ProductionMasterDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<ProductionMasterDto>();
            }
        }

        public async Task<ProductionMasterDto> GetByVoucherNoAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<ProductionMasterDto>>(json);
                return payload != null ? payload.Body : null;
            }
        }

        public async Task<string> CreateAsync(ProductionCreateUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload != null ? payload.Body : string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, ProductionCreateUpdateRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task IssueMaterialsAsync(string voucherNo, IssueMaterialsApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/issue-materials", content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task CompleteProductionAsync(string voucherNo, CompleteProductionApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/complete", content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task CancelProductionAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.PostAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/cancel", new StringContent("{}", Encoding.UTF8, "application/json"));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class ProductionMasterDto
    {
        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("bomMasterId")]
        public int? BomId { get; set; }

        [JsonProperty("bomId")]
        private int? BomIdAlias { set { if (value.HasValue && !BomId.HasValue) BomId = value; } }

        [JsonProperty("bomTitle")]
        public string BomName { get; set; }

        [JsonProperty("bomName")]
        private string BomNameAlias { set { if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(BomName)) BomName = value; } }

        [JsonProperty("finishedItemId")]
        public string FinishedItemId { get; set; }

        [JsonProperty("finishedItemTitle")]
        public string FinishedItemName { get; set; }

        [JsonProperty("finishedItemName")]
        private string FinishedItemNameAlias { set { if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(FinishedItemName)) FinishedItemName = value; } }

        [JsonProperty("targetQty")]
        public decimal PlannedQuantity { get; set; }

        [JsonProperty("plannedQuantity")]
        private decimal PlannedQuantityAlias { set { if (value != 0 && PlannedQuantity == 0) PlannedQuantity = value; } }

        [JsonProperty("producedQty")]
        public decimal ProducedQuantity { get; set; }

        [JsonProperty("producedQuantity")]
        private decimal ProducedQuantityAlias { set { if (value != 0 && ProducedQuantity == 0) ProducedQuantity = value; } }

        [JsonProperty("rejectedQuantity")]
        public decimal RejectedQuantity { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("unitTitle")]
        public string UnitTitle { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("currentStage")]
        public string CurrentStage { get; set; }

        [JsonProperty("issuedDate")]
        public DateTime? IssuedDate { get; set; }

        [JsonProperty("completedDate")]
        public DateTime? CompletedDate { get; set; }

        [JsonProperty("totalMaterialCost")]
        public decimal RawMaterialCost { get; set; }

        [JsonProperty("rawMaterialCost")]
        private decimal RawMaterialCostAlias { set { if (value != 0 && RawMaterialCost == 0) RawMaterialCost = value; } }

        [JsonProperty("laborCost")]
        public decimal ActualLaborCost { get; set; }

        [JsonProperty("actualLaborCost")]
        private decimal ActualLaborCostAlias { set { if (value != 0 && ActualLaborCost == 0) ActualLaborCost = value; } }

        [JsonProperty("overheadCost")]
        public decimal ActualOverheadCost { get; set; }

        [JsonProperty("actualOverheadCost")]
        private decimal ActualOverheadCostAlias { set { if (value != 0 && ActualOverheadCost == 0) ActualOverheadCost = value; } }

        [JsonProperty("totalProductionCost")]
        public decimal TotalProductionCost { get; set; }

        [JsonProperty("unitProductionCost")]
        public decimal UnitCost { get; set; }

        [JsonProperty("unitCost")]
        private decimal UnitCostAlias { set { if (value != 0 && UnitCost == 0) UnitCost = value; } }

        [JsonProperty("batchNo")]
        public string BatchNo { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("lines")]
        public List<ProductionDetailDto> Items { get; set; } = new List<ProductionDetailDto>();

        [JsonProperty("items")]
        private List<ProductionDetailDto> ItemsAlias { set { if (value != null && value.Count > 0 && Items.Count == 0) Items = value; } }
    }

    internal class ProductionDetailDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }

        [JsonProperty("rawItemTitle")]
        public string RawItemName { get; set; }

        [JsonProperty("rawItemName")]
        private string RawItemNameAlias { set { if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(RawItemName)) RawItemName = value; } }

        [JsonProperty("unitTitle")]
        public string RawUnit { get; set; }

        [JsonProperty("rawUnit")]
        private string RawUnitAlias { set { if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(RawUnit)) RawUnit = value; } }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("plannedQty")]
        public decimal PlannedQuantity { get; set; }

        [JsonProperty("plannedQuantity")]
        private decimal PlannedQuantityAlias { set { if (value != 0 && PlannedQuantity == 0) PlannedQuantity = value; } }

        [JsonProperty("actualQty")]
        public decimal ActualQuantity { get; set; }

        [JsonProperty("actualQuantity")]
        private decimal ActualQuantityAlias { set { if (value != 0 && ActualQuantity == 0) ActualQuantity = value; } }

        [JsonProperty("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonProperty("totalCost")]
        public decimal TotalCost { get; set; }

        [JsonProperty("wastageQuantity")]
        public decimal WastageQuantity { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    internal class ProductionCreateUpdateRequest
    {
        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("bomMasterId")]
        public int? BomId { get; set; }

        [JsonProperty("bomId")]
        private int? BomIdAlias { set { if (value.HasValue && !BomId.HasValue) BomId = value; } }

        [JsonProperty("finishedItemId")]
        public string FinishedItemId { get; set; }

        [JsonProperty("targetQty")]
        public decimal PlannedQuantity { get; set; }

        [JsonProperty("plannedQuantity")]
        private decimal PlannedQuantityAlias { set { if (value != 0 && PlannedQuantity == 0) PlannedQuantity = value; } }

        [JsonProperty("producedQty")]
        public decimal ProducedQuantity { get; set; }

        [JsonProperty("producedQuantity")]
        private decimal ProducedQuantityAlias { set { if (value != 0 && ProducedQuantity == 0) ProducedQuantity = value; } }

        [JsonProperty("rejectedQuantity")]
        public decimal RejectedQuantity { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("currentStage")]
        public string CurrentStage { get; set; }

        [JsonProperty("laborCost")]
        public decimal ActualLaborCost { get; set; }

        [JsonProperty("actualLaborCost")]
        private decimal ActualLaborCostAlias { set { if (value != 0 && ActualLaborCost == 0) ActualLaborCost = value; } }

        [JsonProperty("overheadCost")]
        public decimal ActualOverheadCost { get; set; }

        [JsonProperty("actualOverheadCost")]
        private decimal ActualOverheadCostAlias { set { if (value != 0 && ActualOverheadCost == 0) ActualOverheadCost = value; } }

        [JsonProperty("laborAccountCode")]
        public string LaborAccountCode { get; set; }

        [JsonProperty("overheadAccountCode")]
        public string OverheadAccountCode { get; set; }

        [JsonProperty("wipAccountCode")]
        public string WipAccountCode { get; set; }

        [JsonProperty("batchNo")]
        public string BatchNo { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("lines")]
        public List<ProductionDetailCreateUpdateRequest> Items { get; set; } = new List<ProductionDetailCreateUpdateRequest>();

        [JsonProperty("items")]
        private List<ProductionDetailCreateUpdateRequest> ItemsAlias { set { if (value != null && value.Count > 0 && Items.Count == 0) Items = value; } }
    }

    internal class ProductionDetailCreateUpdateRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }

        [JsonProperty("plannedQty")]
        public decimal PlannedQuantity { get; set; }

        [JsonProperty("plannedQuantity")]
        private decimal PlannedQuantityAlias { set { if (value != 0 && PlannedQuantity == 0) PlannedQuantity = value; } }

        [JsonProperty("actualQty")]
        public decimal ActualQuantity { get; set; }

        [JsonProperty("actualQuantity")]
        private decimal ActualQuantityAlias { set { if (value != 0 && ActualQuantity == 0) ActualQuantity = value; } }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonProperty("totalCost")]
        public decimal TotalCost { get; set; }

        [JsonProperty("wastageQuantity")]
        public decimal WastageQuantity { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    internal class IssueMaterialsApiRequest
    {
        [JsonProperty("issueDate")]
        public DateTime? IssueDate { get; set; }

        [JsonProperty("stage")]
        public string Stage { get; set; }

        [JsonProperty("materials")]
        public List<MaterialIssueItemDto> Materials { get; set; }
    }

    internal class MaterialIssueItemDto
    {
        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }

        [JsonProperty("actualQuantity")]
        public decimal ActualQuantity { get; set; }

        [JsonProperty("wastageQuantity")]
        public decimal WastageQuantity { get; set; }
    }

    internal class ProductionPagedBody
    {
        [JsonProperty("data")]
        public List<ProductionMasterDto> Data { get; set; }

        [JsonProperty("metadata")]
        public ProductionPageMetadata Metadata { get; set; }
    }

    internal class ProductionPageMetadata
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }

    internal class ProductionPageResult
    {
        public List<ProductionMasterDto> Data { get; set; } = new List<ProductionMasterDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
    }

    internal class CompleteProductionApiRequest
    {
        [JsonProperty("completionDate")]
        public DateTime? CompletionDate { get; set; }

        [JsonProperty("producedQuantity")]
        public decimal ProducedQuantity { get; set; }

        [JsonProperty("rejectedQuantity")]
        public decimal RejectedQuantity { get; set; }

        [JsonProperty("actualLaborCost")]
        public decimal? ActualLaborCost { get; set; }

        [JsonProperty("actualOverheadCost")]
        public decimal? ActualOverheadCost { get; set; }
    }
}
