using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Main
{
    public class SearchTerm
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("use_flag")]
        public string UseFlag { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
