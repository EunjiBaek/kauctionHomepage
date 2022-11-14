using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Email
{
    public class EmailHistory
    {
        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("type_name")]
        public string TypeName { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonIgnore]
        public string Cc { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonProperty("reg_name")]
        public string RegName { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
