using Newtonsoft.Json;

namespace KA.Entities.Models.Main
{
    public class BannerButton
    {
        [JsonIgnore]
        public int Uid { get; set; }

        [JsonIgnore]
        public int Order { get; set; }

        [JsonIgnore]
        public string Text { get; set; }

        [JsonIgnore]
        public string TextEn { get; set; }

        [JsonIgnore]
        public string LinkUrl { get; set; }

        [JsonIgnore]
        public string LinkUrlEn { get; set; }

        [JsonIgnore]
        public string TargetBlank { get; set; }

        [JsonIgnore]
        public string TargetBlankEn { get; set; }

        [JsonIgnore]
        public string UseFlag { get; set; }
    }
}
