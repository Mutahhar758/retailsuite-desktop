using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class BankReconciliationApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/bankreconciliations";

        public async Task<BankReconciliationSnapshotDto> GetSnapshotAsync(string bankAccount, DateTime fromDate, DateTime toDate)
        {
            var url = Endpoint
                + "?bankAccount=" + Uri.EscapeDataString(bankAccount)
                + "&fromDate=" + Uri.EscapeDataString(fromDate.ToString("yyyy-MM-dd"))
                + "&toDate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd"));

            using (var client = CreateClient(includeTenantId: true))
            {
                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<BankReconciliationSnapshotDto>>(json);
                return payload?.Body ?? new BankReconciliationSnapshotDto();
            }
        }

        public async Task SaveAsync(List<BankReconciliationLineSaveRequestDto> lines)
        {
            using (var client = CreateClient(includeTenantId: true))
            {
                var request = new BankReconciliationSaveRequestDto { Lines = lines ?? new List<BankReconciliationLineSaveRequestDto>() };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class BankReconciliationLineDto
    {
        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("checkDate")]
        public DateTime? CheckDate { get; set; }

        [JsonProperty("checkNum")]
        public string CheckNum { get; set; }

        [JsonProperty("reconcileDate")]
        public DateTime? ReconcileDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("dr")]
        public decimal Dr { get; set; }

        [JsonProperty("cr")]
        public decimal Cr { get; set; }

        [JsonProperty("clear")]
        public bool Clear { get; set; }
    }

    internal class BankReconciliationSnapshotDto
    {
        [JsonProperty("lines")]
        public List<BankReconciliationLineDto> Lines { get; set; } = new List<BankReconciliationLineDto>();

        [JsonProperty("reconcileBalance")]
        public decimal ReconcileBalance { get; set; }

        [JsonProperty("statementBalance")]
        public decimal StatementBalance { get; set; }
    }

    internal class BankReconciliationSaveRequestDto
    {
        [JsonProperty("lines")]
        public List<BankReconciliationLineSaveRequestDto> Lines { get; set; } = new List<BankReconciliationLineSaveRequestDto>();
    }

    internal class BankReconciliationLineSaveRequestDto
    {
        [JsonProperty("voucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("clear")]
        public bool Clear { get; set; }

        [JsonProperty("reconcileDate")]
        public string ReconcileDate { get; set; }
    }
}
