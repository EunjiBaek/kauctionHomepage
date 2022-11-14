using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Content
{
    public class BoardDoc
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("doc_no")]
        public int DocNo { get; set; }

        [JsonProperty("doc_title")]
        public string DocTitle { get; set; }

        [JsonIgnore]
        public string DocContents { get; set; }

        [JsonIgnore]
        public string HtmlYN { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("mem_id")]
        public string MemID { get; set; }

        [JsonProperty("mem_nick")]
        public string MemNick { get; set; }

        [JsonProperty("writer")]
        public string Writer => MemNick != null && !string.IsNullOrWhiteSpace(MemNick) ? MemNick : MemID;

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("read_cnt")]
        public int ReadCnt { get; set; }

        [JsonProperty("period_yn")]
        public string PeriodYN { get; set; }

        [JsonProperty("start_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EndDate { get; set; }

        [JsonProperty("del_yn")]
        public string DelYN { get; set; }

        [JsonIgnore]
        public int TotalCount { get; set; }

        [JsonProperty("display_no")]
        public int DisplayNo { get; set; }

        [JsonProperty("display_mode")]
        public string DisplayMode { get; set; }

        [JsonIgnore]
        public string BoardName { get; set; }

        [JsonProperty("notice_yn")]
        public string NoticeYN { get; set; }

        [JsonProperty("attach_yn")]
        public string AttachYN { get; set; }
    }
}
