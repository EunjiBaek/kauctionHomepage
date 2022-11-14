using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberClause
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonIgnore]
        public string CodeName { get; set; }

        [JsonIgnore]
        public string CodeName2 { get; set; }

        [JsonIgnore]
        public string DisplayCodeName => IsKor ? CodeName : CodeName2;

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("content_en")]
        public string ContentEn { get; set; }

        [JsonIgnore]
        public string DisplayContent => IsKor ? Content : ContentEn;

        [JsonIgnore]
        public string RequiredYN { get; set; }

        [JsonIgnore]
        public string SmsMailYN { get; set; }

        [JsonIgnore]
        public string ClauseType { get; set; }

        [JsonIgnore]
        public int UpdateUID { get; set; }

        [JsonIgnore]
        public DateTime UpdateDate { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }
    }
}
