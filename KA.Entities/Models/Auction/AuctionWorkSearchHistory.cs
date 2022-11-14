using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionWorkSearchHistory
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("auc_kind")]
        public string AucKind { get; set; }

        [JsonProperty("auc_num")]
        public int AucNum { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        [JsonProperty("reg_date")]
        public DateTime RegDate { get; set; }
    }
}
