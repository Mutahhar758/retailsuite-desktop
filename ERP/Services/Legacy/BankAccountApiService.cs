using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class BankAccountApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/bankaccounts";

        public async Task<List<BankAccountDto>> GetAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<BankAccountDto>>>(json);
                return payload?.Body ?? new List<BankAccountDto>();
            }
        }

        public async Task<BankAccountDto> GetByIdAsync(string account)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + Uri.EscapeDataString(account));
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<BankAccountDto>>(json);
                return payload?.Body;
            }
        }

        public async Task<string> CreateAsync(BankAccountCreateApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload?.Body;
            }
        }

        public async Task UpdateAsync(string account, BankAccountUpdateApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + Uri.EscapeDataString(account), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string account)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + Uri.EscapeDataString(account));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class BankAccountDto
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("accountTitle")]
        public string AccountTitle { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty("swiftCode")]
        public string SwiftCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("openingBalance")]
        public decimal OpeningBalance { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class BankAccountCreateApiRequest
    {
        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("accountTitle")]
        public string AccountTitle { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty("swiftCode")]
        public string SwiftCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; } = true;

        [JsonProperty("openingBalance")]
        public decimal? OpeningBalance { get; set; }
    }

    internal class BankAccountUpdateApiRequest
    {
        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("accountTitle")]
        public string AccountTitle { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty("swiftCode")]
        public string SwiftCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("openingBalance")]
        public decimal? OpeningBalance { get; set; }
    }
}
