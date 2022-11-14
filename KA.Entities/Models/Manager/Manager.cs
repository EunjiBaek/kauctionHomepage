using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Manager
{
    public class Manager
    {
        [JsonProperty("uid")]
        public int UID { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonIgnore]
        public string Pw { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string Email { get; set; }

        [JsonProperty("h_tel")]
        public string HTel { get; set; }

        [JsonProperty("ex_tel")]
        public string ExTel { get; set; }

        [JsonIgnore]
        public int GrpSeq { get; set; }

        [JsonProperty("grp_name")]
        public string GrpName { get; set; }

        [JsonIgnore]
        public int Grp2Seq { get; set; }

        [JsonIgnore]
        public string Grp2Name { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("auth")]
        public string Auth { get; set; }

        [JsonProperty("create_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CreateDate { get; set; }

        [JsonIgnore]
        public DateTime ModifyDate { get; set; }

        [JsonIgnore]
        public string Token { get; set; }

        [JsonProperty("last_login_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime LastLoginDate { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
