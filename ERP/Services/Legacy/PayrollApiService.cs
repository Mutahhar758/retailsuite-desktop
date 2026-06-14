using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class PayrollApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/payrolls";

        public async Task<List<PayrollVoucherDto>> GetListAsync(string fromDate = "", string toDate = "")
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(fromDate)) qs.Add("fromDate=" + Uri.EscapeDataString(fromDate));
            if (!string.IsNullOrWhiteSpace(toDate)) qs.Add("toDate=" + Uri.EscapeDataString(toDate));

            var url = Endpoint + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<PayrollVoucherDto>>>(json);
                return payload?.Body ?? new List<PayrollVoucherDto>();
            }
        }

        public async Task<List<PayrollDetailDto>> GetDetailAsync(string voucherNo)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<PayrollDetailDto>>>(json);
                return payload?.Body ?? new List<PayrollDetailDto>();
            }
        }

        public async Task<PayrollLookupsDto> GetLookupsAsync()
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(Endpoint + "/lookups");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<PayrollLookupsDto>>(json);
                return payload?.Body ?? new PayrollLookupsDto();
            }
        }

        public async Task<string> CreateAsync(PayrollUpsertApiRequest request)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body ?? string.Empty;
            }
        }

        public async Task UpdateAsync(string voucherNo, PayrollUpsertApiRequest request)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string voucherNo)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteLineAsync(string voucherNo, long seq)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(voucherNo) + "/lines/" + seq);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class PayrollVoucherDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("salaryType")]
        public string SalaryType { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class PayrollDetailDto
    {
        [JsonProperty("seq")]
        public long Seq { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("salaryType")]
        public string SalaryType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("hrId")]
        public string HrId { get; set; }

        [JsonProperty("hrName")]
        public string HrName { get; set; }

        [JsonProperty("payableAccount")]
        public string PayableAccount { get; set; }

        [JsonProperty("expenseAccount")]
        public string ExpenseAccount { get; set; }

        [JsonProperty("salary")]
        public decimal Salary { get; set; }

        [JsonProperty("noOfLeaves")]
        public decimal NoOfLeaves { get; set; }

        [JsonProperty("leaveCharges")]
        public decimal LeaveCharges { get; set; }

        [JsonProperty("overtime")]
        public decimal Overtime { get; set; }

        [JsonProperty("overtimeCharges")]
        public decimal OvertimeCharges { get; set; }

        [JsonProperty("bonus")]
        public decimal Bonus { get; set; }

        [JsonProperty("netSalary")]
        public decimal NetSalary { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class PayrollLookupItemDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class PayrollEmployeeDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("salaryType")]
        public string SalaryType { get; set; }

        [JsonProperty("salary")]
        public decimal Salary { get; set; }

        [JsonProperty("leaveCharges")]
        public decimal LeaveCharges { get; set; }

        [JsonProperty("overtime")]
        public decimal Overtime { get; set; }

        [JsonProperty("payableAccount")]
        public string PayableAccount { get; set; }

        [JsonProperty("expenseAccount")]
        public string ExpenseAccount { get; set; }
    }

    internal class PayrollLookupsDto
    {
        [JsonProperty("employees")]
        public List<PayrollEmployeeDto> Employees { get; set; } = new List<PayrollEmployeeDto>();

        [JsonProperty("expenseAccounts")]
        public List<PayrollLookupItemDto> ExpenseAccounts { get; set; } = new List<PayrollLookupItemDto>();

        [JsonProperty("payableAccounts")]
        public List<PayrollLookupItemDto> PayableAccounts { get; set; } = new List<PayrollLookupItemDto>();
    }

    internal class PayrollUpsertApiRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("salaryType")]
        public string SalaryType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("lines")]
        public List<PayrollLineApiRequest> Lines { get; set; } = new List<PayrollLineApiRequest>();
    }

    internal class PayrollLineApiRequest
    {
        [JsonProperty("seq")]
        public long Seq { get; set; }

        [JsonProperty("hrId")]
        public string HrId { get; set; }

        [JsonProperty("payableAccount")]
        public string PayableAccount { get; set; }

        [JsonProperty("expenseAccount")]
        public string ExpenseAccount { get; set; }

        [JsonProperty("salary")]
        public decimal Salary { get; set; }

        [JsonProperty("noOfLeaves")]
        public decimal NoOfLeaves { get; set; }

        [JsonProperty("leaveCharges")]
        public decimal LeaveCharges { get; set; }

        [JsonProperty("overtime")]
        public decimal Overtime { get; set; }

        [JsonProperty("overtimeCharges")]
        public decimal OvertimeCharges { get; set; }

        [JsonProperty("bonus")]
        public decimal Bonus { get; set; }

        [JsonProperty("netSalary")]
        public decimal NetSalary { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }
    }
}
