using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Content
{
    public class FileUpload
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fullpath")]
        public string Fullpath { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("explain_text")]
        public string ExplainText { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonIgnore]
        public string DelYn { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("no")]
        public int No { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
