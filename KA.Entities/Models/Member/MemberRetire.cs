using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberRetire : Member
    {
        [JsonProperty("retire_reason")]
        public string RetireReason { get; set; }

        [JsonIgnore]
        public string RetireOption { get; set; }

        [JsonProperty("retire_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RetireDate { get; set; }

        [JsonIgnore]
        public new int MngUid { get; set; }
    }
}
