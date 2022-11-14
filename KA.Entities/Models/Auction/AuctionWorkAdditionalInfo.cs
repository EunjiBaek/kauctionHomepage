using Newtonsoft.Json;

namespace KA.Entities.Models.Auction
{
    public class AuctionWorkAdditionalInfo
    {
        [JsonProperty("t_code")]
        public string TCode { get; set; }

        [JsonProperty("flag")]
        public string Flag { get; set; }

        [JsonProperty("info_idx")]
        public int InfoIdx { get; set; }

        [JsonProperty("value1")]
        public string Value1 { get; set; }

        [JsonProperty("value2")]
        public string Value2 { get; set; }

        [JsonProperty("value3")]
        public string Value3 { get; set; }

        [JsonProperty("value4")]
        public string Value4 { get; set; }

        [JsonProperty("value5")]
        public string Value5 { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonIgnore]
        public string SubPath {
            get
            {
                return TCode.Equals("A") ? "Artist" : "Work";
            }
        }

        [JsonIgnore]
        public string Href
        {
            get
            {
                return Value3 != null && Value3.Equals("3") ? Value2.Trim() : "javascript:void(0);";
            }
        }

        [JsonIgnore]
        public string Click
        {
            get
            {
                return Value3 != null && Value3.Equals("3") ? "javascript:void(0);" : $"javascript:$.commonUtils.openWindow('{Value2}', '{(Value3 != null && Value3.Equals("1") ? "F" : "")}', 'WorkInfoPage');";
            }
        }

        [JsonIgnore]
        public string ImageUrl
        {
            get
            {
                return Path != null && FileName != null && !string.IsNullOrWhiteSpace(Path) && !string.IsNullOrWhiteSpace(FileName) ? $"www/{SubPath}Banner/{Path}/{FileName}" : string.Empty;
            }
            set { }
        }

        [JsonIgnore]
        public string YoutubeThum
        {
            get
            {
                return Path != null && FileName != null && !string.IsNullOrWhiteSpace(Path) && !string.IsNullOrWhiteSpace(FileName) ? $"www/{SubPath}YoutubeThum/{Path}/{FileName}" : string.Empty;
            }
            set { }
        }
    }
}
