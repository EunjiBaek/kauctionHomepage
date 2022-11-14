using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberActivity
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }
        
        [JsonProperty("activity_code")]
        public string ActivityCode { get; set; }

        [JsonIgnore]
        public string ActivityCodeName { get; set; }

        [JsonIgnore]
        public string ActivityCodeNameEn { get; set; }

        [JsonProperty("activity_code_name")]
        public string DisplayActivityCodeName => !IsKor && !string.IsNullOrWhiteSpace(ActivityCodeNameEn) ? ActivityCodeNameEn : ActivityCodeName;

        [JsonProperty("etc")]
        public string Etc { get; set; }

        /// <summary>
        /// 상태 D -> 요청
        /// 상태 R -> 거절
        /// 상태 A -> 승인
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonIgnore]
        public string StateName { get; set; }

        [JsonIgnore]
        public string StateNameEn { get; set; }

        [JsonProperty("state_name")]
        public string DisplayStateName => !IsKor && !string.IsNullOrWhiteSpace(StateNameEn) ? StateNameEn : StateName;

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("mod_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ModDate { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonProperty("bid_allow_yn")]
        public string BidAllowYn { get; set; }
    }
}
