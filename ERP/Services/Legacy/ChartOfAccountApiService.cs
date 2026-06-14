using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class ChartOfAccountApiService : ApiServiceBase
    {
        private const string ChartOfAccountEndpoint = "/api/chartofaccounts";

        public async Task<List<ChartOfAccountHeadDto>> GetHeadsAsync(int level)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ChartOfAccountEndpoint + "/heads?level=" + level);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountHeadDto>>>(json);
                return payload?.Body ?? new List<ChartOfAccountHeadDto>();
            }
        }

        public async Task<List<ChartOfAccountDto>> GetActiveAccountsAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ChartOfAccountEndpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<ChartOfAccountDto>();
            }
        }

        public async Task<string> CreateAsync(string parentId, string title)
        {
            using (var client = CreateClient())
            {
                var request = new ChartOfAccountCreateRequest
                {
                    ParentId = parentId,
                    Title = title
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ChartOfAccountEndpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<string>>(json);
                return payload != null ? payload.Body : null;
            }
        }

        public async Task UpdateAsync(string account, string title)
        {
            using (var client = CreateClient())
            {
                var request = new ChartOfAccountUpdateRequest
                {
                    Title = title
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(ChartOfAccountEndpoint + "/" + Uri.EscapeDataString(account), content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(string account)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(ChartOfAccountEndpoint + "/" + Uri.EscapeDataString(account));
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task<List<ChartOfAccountHeadDto>> GetAccountsByPrefixAsync(string prefix, int? level)
        {
            using (var client = CreateClient())
            {
                var query = "prefix=" + Uri.EscapeDataString(prefix ?? string.Empty);
                if (level.HasValue)
                    query += "&level=" + level.Value;

                var response = await client.GetAsync(ChartOfAccountEndpoint + "/lookup?" + query);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountHeadDto>>>(json);
                return payload?.Body ?? new List<ChartOfAccountHeadDto>();
            }
        }

        public async Task<List<ChartOfAccountHeadDto>> GetDetailAccountsAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ChartOfAccountEndpoint + "/detail");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountHeadDto>>>(json);
                return payload?.Body ?? new List<ChartOfAccountHeadDto>();
            }
        }

        public async Task<List<ChartOfAccountHeadDto>> GetCashBankAccountsAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ChartOfAccountEndpoint + "/cashbanks");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountHeadDto>>>(json);
                return payload?.Body ?? new List<ChartOfAccountHeadDto>();
            }
        }

        public async Task<List<ChartOfAccountHeadDto>> GetSupplierAccountsAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ChartOfAccountEndpoint + "/suppliers");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountHeadDto>>>(json);
                return payload?.Body ?? new List<ChartOfAccountHeadDto>();
            }
        }

        public async Task<List<ChartOfAccountHeadDto>> GetCustomerAccountsAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(ChartOfAccountEndpoint + "/customers");
                await EnsureSuccessWithServerMessageAsync(response);
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<ChartOfAccountHeadDto>>>(json);
                return payload?.Body ?? new List<ChartOfAccountHeadDto>();
            }
        }
    }

    internal class ChartOfAccountHeadDto
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class ChartOfAccountDto
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("accType")]
        public string AccType { get; set; }

        [JsonProperty("accLevel")]
        public int AccLevel { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }
    }

    internal class ChartOfAccountCreateRequest
    {
        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal class ChartOfAccountUpdateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
