using System;
using Newtonsoft.Json;

namespace ERP.Services.Legacy
{
    internal class PaymentRecordRequest
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("checkNum")]
        public string CheckNum { get; set; }

        [JsonProperty("checkDate")]
        public string CheckDate { get; set; }
    }

    internal class PaymentRecordResponse
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("accountTitle")]
        public string AccountTitle { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("checkNum")]
        public string CheckNum { get; set; }

        [JsonProperty("checkDate")]
        public string CheckDate { get; set; }
    }
}
