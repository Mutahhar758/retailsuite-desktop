using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class HRInfoApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/hrinfo";

        public async Task<List<HRInfoDto>> GetAsync()
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<HRInfoDto>>>(json);
                return payload?.Body ?? new List<HRInfoDto>();
            }
        }

        public async Task<HRInfoDto> GetByIdAsync(string id)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(id));
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<HRInfoDto>>(json);
                return payload?.Body;
            }
        }

        public async Task UpdateAsync(string id, HRInfoUpsertApiRequest request)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(id), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task CreateAsync(HRInfoUpsertApiRequest request)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string id)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(id));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    public class HRInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public string MaritialStatus { get; set; }
        public string Cnic { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime JoiningDate { get; set; }
        public string Designation { get; set; }
        public string SalaryType { get; set; }
        public decimal Salary { get; set; }
        public decimal LeaveCharges { get; set; }
        public decimal Overtime { get; set; }
        public string ExpenseAccount { get; set; }
        public string PayableAccount { get; set; }
    }

    public class HRInfoUpsertApiRequest
    {
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public string MaritialStatus { get; set; }
        public string Cnic { get; set; }
        public string AppointmentDate { get; set; }
        public string JoiningDate { get; set; }
        public string Designation { get; set; }
        public string SalaryType { get; set; }
        public decimal Salary { get; set; }
        public decimal LeaveCharges { get; set; }
        public decimal Overtime { get; set; }
        public string ExpenseAccount { get; set; }
        public string PayableAccount { get; set; }
    }
}
