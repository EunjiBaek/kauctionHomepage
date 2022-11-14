using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class TermAndCondition
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        // [JsonProperty("description")]
        [JsonIgnore]
        public string Description { get; set; }

        // [JsonProperty("description_en")]
        [JsonIgnore]
        public string DescriptionEn { get; set; }

        [JsonProperty("description")]
        public string DisplayDescription => !IsKor && !string.IsNullOrWhiteSpace(DescriptionEn) ? DescriptionEn : Description;

        [JsonProperty("use_yn")]
        public string UseYn { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }

        [JsonIgnore]
        public string Content { get; set; }

        [JsonIgnore]
        public string ContentEn { get; set; }

        [JsonProperty("content")]
        public string DisplayContent => !IsKor && !string.IsNullOrWhiteSpace(ContentEn) ? ContentEn : Content;

        [JsonProperty("start_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EndDate { get; set; }

        [JsonProperty("date_primary_yn")]
        public string DatePrimaryYn { get; set; }

        [JsonProperty("code_name")]
        public string CodeName { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }
    }
}
