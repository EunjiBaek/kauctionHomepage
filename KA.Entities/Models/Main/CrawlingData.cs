using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KA.Entities.Models.Main
{
    public class CrawlingData
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("no")]
        public int No { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("image_path")]
        public string ImagePath { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("mod_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime ModDate { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("click_count")]
        public int ClickCount { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("etc1")]
        public string Etc1 { get; set; }

        [JsonProperty("etc2")]
        public string Etc2 { get; set; }

        [JsonProperty("etc3")]
        public string Etc3 { get; set; }

        [JsonProperty("read_yn")]
        public string ReadYN { get; set; }
    }
}
