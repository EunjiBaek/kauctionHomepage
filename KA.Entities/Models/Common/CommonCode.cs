using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Common
{
    public class CommonCode
    {
        [JsonProperty("code_seq")]
        public int CodeSeq { get; set; }

        [JsonProperty("main_code")]
        public string MainCode { get; set; }

        [JsonProperty("sub_code")]
        public string SubCode { get; set; }

        [JsonProperty("code_name")]
        public string CodeName { get; set; }

        [JsonProperty("code_name2")]
        public string CodeName2 { get; set; }

        [JsonProperty("display_code_name")]
        public string DisplayCodeName { get; set; }

        [JsonProperty("extra1")]
        public string Extra1 { get; set; }

        [JsonProperty("extra2")]
        public string Extra2 { get; set; }

        [JsonProperty("extra3")]
        public string Extra3 { get; set; }

        [JsonProperty("extra4")]
        public string Extra4 { get; set; }

        [JsonProperty("extra5")]
        public string Extra5 { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("sort")]
        public int Sort { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUID { get; set; }

        [JsonProperty("reg_name")]
        public string RegName { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("auth_yn")]
        public string AuthYN { get; set; }
    }
}
