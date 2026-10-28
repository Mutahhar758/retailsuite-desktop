using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class SaleSupplyApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/salesupplies";

        /// <summary>Returns a single server-side page of sale supply vouchers.</summary>
        public async Task<SaleSupplyPageResult> GetPagedListAsync(
            string fromDate = "", string toDate = "",
            string itemId = "", string voucherNo = "",
            int pageNumber = 1, int pageSize = 20)
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(itemId)) qs.Add("itemId=" + Uri.EscapeDataString(itemId));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));
            qs.Add("pageNumber=" + pageNumber);
            qs.Add("pageSize=" + pageSize);

            var url = Endpoint + "/paged?" + string.Join("&", qs);

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SaleSupplyPagedBody>>(json);
                var body = payload?.Body;
                var meta = body?.Metadata;
                return new SaleSupplyPageResult
                {
                    Data = body?.Data ?? new List<SaleSupplyDto>(),
                    CurrentPage = meta?.CurrentPage ?? pageNumber,
                    TotalPages = meta?.TotalPages ?? 1,
                    TotalCount = meta?.TotalCount ?? 0,
                    PageSize = meta?.PageSize ?? pageSize
                };
            }
        }
        public async Task<List<SaleSupplyDto>> GetListAsync(
            string fromDate = "", string toDate = "",
            string itemId = "", string voucherNo = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(itemId)) qs.Add("itemId=" + Uri.EscapeDataString(itemId));
            if (!string.IsNullOrWhiteSpace(voucherNo)) qs.Add("voucherNo=" + Uri.EscapeDataString(voucherNo));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleSupplyDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<SaleSupplyDto>();
            }
        }

        public async Task<List<SaleSupplyLineDto>> GetDetailAsync(string voucherNo)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleSupplyLineDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<SaleSupplyLineDto>();
            }
        }

        public async Task<string> CreateAsync(SaleSupplyCreateApiRequest request)
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

        public async Task UpdateAsync(string voucherNo, SaleSupplyUpdateApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo), content);
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

        public async Task DeleteLineAsync(string voucherNo, int seq)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<List<SaleSupplyLineDto>> GetCustomerLinesAsync(
            string customerId, string fromDate = "", string toDate = "", string itemId = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(customerId)) qs.Add("customerId=" + Uri.EscapeDataString(customerId));
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));
            if (!string.IsNullOrWhiteSpace(itemId)) qs.Add("itemId=" + Uri.EscapeDataString(itemId));

            var url = Endpoint + "/customer-records" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SaleSupplyLineDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<SaleSupplyLineDto>();
            }
        }

        public async Task UpdateLineAsync(string voucherNo, int seq, SaleSupplyLineApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task UpdateCustomerLinesAsync(List<SaleSupplyCustomerLineUpdateRequest> requests)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(requests), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/customer-lines", content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<SaleSupplyScanResponseDto> ScanSheetAsync(List<string> filePaths, string targetDate = null)
        {
            using (var client = CreateClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                using (var content = new MultipartFormDataContent())
                {
                    foreach (var path in filePaths)
                    {
                        if (!System.IO.File.Exists(path)) continue;
                        var fileBytes = System.IO.File.ReadAllBytes(path);
                        var fileContent = new ByteArrayContent(fileBytes);
                        var fileName = System.IO.Path.GetFileName(path);
                        var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
                        string mediaType = "image/jpeg";
                        if (ext == ".png") mediaType = "image/png";
                        else if (ext == ".pdf") mediaType = "application/pdf";
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
                        content.Add(fileContent, "files", fileName);
                    }

                    if (!string.IsNullOrWhiteSpace(targetDate))
                    {
                        content.Add(new StringContent(targetDate), "targetDate");
                    }

                    var response = await client.PostAsync(Endpoint + "/scan-sheet", content);
                    await EnsureSuccessWithServerMessageAsync(response);
                    var json = await response.Content.ReadAsStringAsync();
                    var payload = JsonConvert.DeserializeObject<HttpResponseDto<SaleSupplyScanResponseDto>>(json);
                    return payload != null && payload.Body != null ? payload.Body : new SaleSupplyScanResponseDto();
                }
            }
        }

        public async Task<PresignedUploadUrlDto> GetAttachmentUploadUrlAsync(string fileName)
        {
            using (var client = CreateClient())
            {
                var response = await client.PostAsync(Endpoint + "/attachments/presigned-upload-url?fileName=" + Uri.EscapeDataString(fileName), null);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PresignedUploadUrlDto>>(json);
                return payload != null ? payload.Body : null;
            }
        }

        public async Task<string> UploadAttachmentDirectAsync(string filePath)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var presigned = await GetAttachmentUploadUrlAsync(fileName);
            if (presigned == null || string.IsNullOrWhiteSpace(presigned.UploadUrl))
                throw new Exception("Failed to retrieve upload URL for attachment.");

            await UploadFileAsync(presigned.UploadUrl, filePath);
            return presigned.FileId;
        }

        public async Task<SaleSupplyAttachmentDto> AddAttachmentAsync(string voucherNo, string mediaId, string fileName, string description = "Scanned paper slip")
        {
            using (var client = CreateClient())
            {
                var body = new
                {
                    vNo = voucherNo,
                    vType = "SP",
                    mediaId = mediaId,
                    fileName = fileName,
                    description = description
                };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/attachments", content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SaleSupplyAttachmentDto>>(json);
                return payload != null ? payload.Body : null;
            }
        }
    }

    internal class SaleSupplyDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("supplyOrderTitle")]
        public string SupplyOrderTitle { get; set; }
    }

    internal class SaleSupplyLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        [JsonProperty("customerTitle")]
        public string CustomerTitle { get; set; }

        [JsonProperty("subAccountId")]
        public string SubAccountId { get; set; }

        [JsonProperty("subAccountTitle")]
        public string SubAccountTitle { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal? QtyInPack { get; set; }

        [JsonProperty("packing")]
        public decimal? Packing { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("narrationId")]
        public string NarrationId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("addLess")]
        public decimal AddLess { get; set; }

        [JsonProperty("carriage")]
        public decimal? Carriage { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("secQty")]
        public decimal? SecQty { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }

        [JsonProperty("secUnit")]
        public string SecUnit { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class SaleSupplyCreateApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("lines")]
        public List<SaleSupplyLineApiRequest> Lines { get; set; } = new List<SaleSupplyLineApiRequest>();

        [JsonProperty("attachmentMediaIds")]
        public List<string> AttachmentMediaIds { get; set; } = new List<string>();
    }

    internal class SaleSupplyUpdateApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("supplyOrderMasterId")]
        public int? SupplyOrderMasterId { get; set; }

        [JsonProperty("lines")]
        public List<SaleSupplyLineApiRequest> Lines { get; set; } = new List<SaleSupplyLineApiRequest>();

        [JsonProperty("attachmentMediaIds")]
        public List<string> AttachmentMediaIds { get; set; } = new List<string>();
    }

    internal class SaleSupplyLineApiRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("addLess")]
        public decimal AddLess { get; set; }

        [JsonProperty("carriage")]
        public decimal? Carriage { get; set; }

        [JsonProperty("secQty")]
        public decimal? SecQty { get; set; }

        [JsonProperty("secRate")]
        public decimal? SecRate { get; set; }

        [JsonProperty("secUnit")]
        public string SecUnit { get; set; }

        [JsonProperty("qtyInPack")]
        public decimal? QtyInPack { get; set; }

        [JsonProperty("packing")]
        public decimal? Packing { get; set; }
    }

    internal class SaleSupplyCustomerLineUpdateRequest
    {
        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("line")]
        public SaleSupplyLineApiRequest Line { get; set; }
    }

    public class ScannedSaleSupplyLineDto
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("rawCustomerName")]
        public string RawCustomerName { get; set; }

        [JsonProperty("matchedCustomerId")]
        public string MatchedCustomerId { get; set; }

        [JsonProperty("matchedCustomerName")]
        public string MatchedCustomerName { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("addLess")]
        public decimal AddLess { get; set; }

        [JsonProperty("carriage")]
        public decimal Carriage { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("isCustomerMatched")]
        public bool IsCustomerMatched { get; set; }
    }

    public class SaleSupplyScanResponseDto
    {
        [JsonProperty("lines")]
        public List<ScannedSaleSupplyLineDto> Lines { get; set; } = new List<ScannedSaleSupplyLineDto>();

        [JsonProperty("remainingScansThisMonth")]
        public int RemainingScansThisMonth { get; set; }

        [JsonProperty("monthlyLimit")]
        public int MonthlyLimit { get; set; }

        [JsonProperty("rawDetectedText")]
        public string RawDetectedText { get; set; }
    }

    public class SaleSupplyAttachmentDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("vType")]
        public string VType { get; set; }

        [JsonProperty("vNo")]
        public string VNo { get; set; }

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }
    }

    internal class SaleSupplyPagedBody
    {
        [JsonProperty("data")]
        public List<SaleSupplyDto> Data { get; set; }

        [JsonProperty("metadata")]
        public SaleSupplyPageMetadata Metadata { get; set; }
    }

    internal class SaleSupplyPageMetadata
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

    internal class SaleSupplyPageResult
    {
        public List<SaleSupplyDto> Data { get; set; } = new List<SaleSupplyDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
    }}
