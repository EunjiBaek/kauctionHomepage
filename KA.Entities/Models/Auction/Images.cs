using Newtonsoft.Json;

namespace KA.Entities.Models.Auction
{
    public class Image
    {
        [JsonIgnore]
        public string Code { get; set; }

        [JsonIgnore]
        public int ImgIdx { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }

        [JsonIgnore]
        public string FieldName { get; set; }

        [JsonIgnore]
        public string Path { get; set; }

        [JsonIgnore]
        public string Width { get; set; }

        [JsonIgnore]
        public string Height { get; set; }

        [JsonIgnore]
        public string Flag { get; set; }

        [JsonIgnore]
        public string ImagePath => $"Artist/Image/{Path}/{FileName}";
    }
}
