using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Content
{
    public class BoardInfo
    {
        [JsonProperty("board_seq")]
        public int BoardSeq { get; set; }

        [JsonProperty("board_name")]
        public string BoardName { get; set; }

        [JsonProperty("board_key")]
        public string BoardKey { get; set; }

        [JsonProperty("board_type")]
        public string BoardType { get; set; }

        [JsonProperty("page_size")]
        public int PageSize { get; set; }

        [JsonProperty("notice_yn")]
        public string NoticeYN { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonIgnore]
        public int RegUID { get; set; }

        [JsonIgnore]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }
    }
}
