using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Main
{
    /// <summary>
    /// [#729/개선] 홈페이지 메인 하드코딩구간 관리 컨텐츠로 변경 - SubTitle/Property 변수 추가
    /// </summary>
    public class Notice
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title_kr")]
        public string Title { get; set; }

        [JsonProperty("title_en")]
        public string TitleEn { get; set; }

        [JsonProperty("title")]
        public string DisplayTitle => !IsKor && !string.IsNullOrWhiteSpace(TitleEn) ? TitleEn : Title;

        [JsonProperty("sub_title_kr")]
        public string SubTitle { get; set; }

        [JsonProperty("sub_title_en")]
        public string SubTitleEn { get; set; }

        [JsonProperty("sub_title")]
        public string DisplaySubTitle => !IsKor && !string.IsNullOrWhiteSpace(SubTitleEn) ? SubTitleEn : SubTitle;

        [JsonProperty("property_kr")]
        public string Property { get; set; }

        [JsonProperty("property_en")]
        public string PropertyEn { get; set; }

        [JsonProperty("property")]
        public string DisplayProperty => !IsKor && !string.IsNullOrWhiteSpace(PropertyEn) ? PropertyEn : Property;

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("start_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm")]
        public DateTime EndDate { get; set; }

        [JsonProperty("use_flag")]
        public string UseFlag { get; set; }

        [JsonProperty("today_flag")]
        public string TodayFlag { get; set; }

        [JsonProperty("target_menu_uid")]
        public int TargetMenuUid { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("active_flag")]
        public string ActiveFlag { get; set; }

        [JsonProperty("mng_name")]
        public string MngName { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("is_kor")]
        public bool IsKor => !string.IsNullOrWhiteSpace(Lang) && Lang.Equals("K");

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("read_count")]
        public int ReadCount { get; set; }
    }
}
