using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionLiveRequest
    {
        [JsonProperty("auc_num")]
        public int AucNum { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("paddle_num")]
        public int PaddleNum { get; set; }

        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }
    }
}
