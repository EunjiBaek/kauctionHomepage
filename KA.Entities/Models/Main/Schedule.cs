using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Main
{
    public class Schedule
    {
        [JsonProperty]
        public int Uid { get; set; }

        [JsonProperty("auc_kind")]
        public string AucKind { get; set; }

        [JsonProperty("auc_num")]
        public int AucNum { get; set; }

        [JsonProperty("title_kr")]
        public string Title { get; set; }

        [JsonProperty("title_en")]
        public string TitleEn { get; set; }

        [JsonProperty("title")]
        public string DisplayTitle => !IsKor && !string.IsNullOrWhiteSpace(TitleEn) ? TitleEn : Title;

        [JsonProperty("start_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EndDate { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("link_yn")]
        public string LinkYN { get; set; }

        [JsonIgnore]
        public string LinkUrl
        {
            get
            {
                switch (AucKind)
                {
                    case "1": return $"/Auction/Major/{AucNum}";
                    case "2": return $"/Auction/Premium/{AucNum}";
                    case "4": return $"/Auction/Weekly/{AucNum}";
                    default: return string.Empty;
                }
            }
        }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }

        /// <summary>
        /// Main 일정 웹파트에서 시작일(S)/종료일(E) 구분값
        /// </summary>
        [JsonProperty("date_mode")]
        public string DateMode { get; set; }
    }
}
