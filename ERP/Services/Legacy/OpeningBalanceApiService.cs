using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class OpeningBalanceApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/openingbalances";

        public async Task<List<OpeningBalanceDto>> GetAsync(string parentAccountId = null)
        {
            using (var client = CreateClient())
            {
                var url = Endpoint;
                if (!string.IsNullOrWhiteSpace(parentAccountId))
                    url += "?parentAccountId=" + Uri.EscapeDataString(parentAccountId);

                var response = await client.GetAsync(url);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<OpeningBalanceDto>>>(json);
                return payload?.Body ?? new List<OpeningBalanceDto>();
            }
        }

        public async Task UpsertAsync(string account, decimal? drAmount, decimal? crAmount)
        {
            using (var client = CreateClient())
            {
                var request = new OpeningBalanceUpsertRequest
                {
                    Account = account,
                    DrAmount = drAmount,
                    CrAmount = crAmount
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class OpeningBalanceDto
    {
        [JsonProperty("parentCode")]
        public string ParentCode { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("bal")]
        public decimal Bal { get; set; }
    }

    internal class OpeningBalanceUpsertRequest
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("drAmount")]
        public decimal? DrAmount { get; set; }

        [JsonProperty("crAmount")]
        public decimal? CrAmount { get; set; }
    }
}
