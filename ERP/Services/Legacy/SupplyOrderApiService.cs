using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERP.Dtos.Authentication;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class SupplyOrderApiService : ApiServiceBase
    {
        private const string Endpoint = "/api/supplyorders";

        public async Task<List<SupplyOrderDto>> GetAsync()
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<List<SupplyOrderDto>>>(json);
                return payload != null && payload.Body != null ? payload.Body : new List<SupplyOrderDto>();
            }
        }

        public async Task<SupplyOrderDto> GetByIdAsync(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync(Endpoint + "/" + id);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SupplyOrderDto>>(json);
                return payload != null ? payload.Body : null;
            }
        }

        public async Task<int> CreateAsync(SupplyOrderUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                await EnsureSuccessWithServerMessageAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<HttpResponseDto<SupplyOrderDto>>(json);
                return payload != null && payload.Body != null ? payload.Body.Id : 0;
            }
        }

        public async Task UpdateAsync(int id, SupplyOrderUpsertApiRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(Endpoint + "/" + id, content);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.DeleteAsync(Endpoint + "/" + id);
                await EnsureSuccessWithServerMessageAsync(response);
            }
        }
    }

    internal class SupplyOrderDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("details")]
        public List<SupplyOrderDetailDto> Details { get; set; }
    }

    internal class SupplyOrderDetailDto
    {
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }
    }

    internal class SupplyOrderUpsertApiRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("details")]
        public List<SupplyOrderDetailUpsertApiRequest> Details { get; set; }
    }

    internal class SupplyOrderDetailUpsertApiRequest
    {
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }
    }
}
