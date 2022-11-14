using KA.Entities.Attributes;
using KA.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace KA.Entities.Models.Auction
{
    public class AuctionSchedule
    {
        #region # AuctionSchedule #

        /// <summary>
        /// 고유번호
        /// </summary>
        [JsonProperty("uid")]
        public int Uid { get; set; }

        /// <summary>
        /// 경매유형
        /// </summary>
        [JsonProperty("auc_kind")]
        public string AucKind { get; set; }

        [JsonProperty("auc_kind_short_name")]
        public string AucKindShortName { get; set; }

        /// <summary>
        /// 경매번호
        /// </summary>
        [JsonProperty("auc_num")]
        public int AucNum { get; set; }

        /// <summary>
        /// 경매명(국문)
        /// </summary>
        [JsonProperty("auc_title_kr")]
        public string AucTitle { get; set; }

        /// <summary>
        /// 경매명(영문)
        /// </summary>
        [JsonProperty("auc_title_en")]
        public string AucTitleEn { get; set; }

        /// <summary>
        /// 경매명을 가공하는 경우가 있어 원본 경매명 (국문)
        /// </summary>
        [JsonIgnore]
        public string AucTitleOrg { get; set; }

        /// <summary>
        /// 경매명을 가공하는 경우가 있어 원본 경매명 (영문)
        /// </summary>
        [JsonIgnore]
        public string AucTitleEnOrg { get; set; }

        /// <summary>
        /// 경매명(표기)
        /// </summary>
        [JsonProperty("auc_title")]
        public string DisplayAucTitle => !IsKor && !string.IsNullOrWhiteSpace(AucTitleEn) ? AucTitleEn : AucTitle;

        /// <summary>
        /// 원본 경매명(표기)
        /// </summary>
        [JsonIgnore]
        public string DisplayAucTitleOrg => !IsKor && !string.IsNullOrWhiteSpace(AucTitleEnOrg) ? AucTitleEnOrg : AucTitleOrg;

        [JsonIgnore]
        public string DisplayAucShortTitle => MessageHelper.GetShortTitleFromAucKind(AucKind, !IsKor ? "ENG" : "KOR");

        /// <summary>
        /// 경매일시
        /// </summary>
        [JsonProperty("auc_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucDate { get; set; }

        /// <summary>
        /// 경매시작일시
        /// </summary>
        [JsonProperty("auc_start_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucStartDate { get; set; }

        /// <summary>
        /// 경매종료일시
        /// </summary>
        [JsonProperty("auc_end_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucEndDate { get; set; }

        /// <summary>
        /// 경매프리뷰 일시
        /// </summary>
        [JsonProperty("auc_preview_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucPreviewDate { get; set; }

        /// <summary>
        /// 메이저 서면/전화 응찰 마감일시
        /// </summary>
        [JsonProperty("auc_bid_end_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucBidEndDate { get; set; }

        /// <summary>
        /// 온라인 응찰 신청 마감일시
        /// </summary>
        [JsonProperty("auc_online_bid_end_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucOnlineBidEndDate { get; set; }

        [JsonIgnore]
        public string DisplayPreviewDate => !IsKor ? string.Format("{0:D}", AucPreviewDate) : AucPreviewDate.ToString("yyyy년 MM월 dd일");

        [JsonIgnore]
        // public string DisplayPreviewTime => !IsKor ? string.Format("{0:t}", AucPreviewDate) : (AucPreviewDate.Minute > 0 ? AucPreviewDate.ToString("tt HH시 mm분") : AucPreviewDate.ToString("tt HH시"));
        public string DisplayPreviewTime => !IsKor ? string.Format("{0:t}", AucPreviewDate) : (AucPreviewDate.Minute > 0 ? AucPreviewDate.ToString("HH시 mm분") : AucPreviewDate.ToString("HH시"));

        /// <summary>
        /// 경매설명(국문)
        /// </summary>
        [JsonIgnore]
        public string AucDesc { get; set; }

        /// <summary>
        /// 경매설명(영문)
        /// </summary>
        [JsonIgnore]
        public string AucDescEn { get; set; }

        /// <summary>
        /// 경매장소(국문)
        /// </summary>
        [JsonIgnore]
        public string AucPlace { get; set; }

        /// <summary>
        /// 경매장소(영문)
        /// </summary>
        [JsonIgnore]
        public string AucPlaceEn { get; set; }

        /// <summary>
        /// 경매장소(표기)
        /// </summary>
        [JsonProperty("auc_place")]
        public string DisplayAucPlace => !IsKor && !string.IsNullOrWhiteSpace(AucPlaceEn) ? AucPlaceEn : AucPlace;

        /// <summary>
        /// 조회여부 (Y/N)
        /// </summary>
        [JsonProperty("view_yn")]
        public string ViewYN { get; set; }

        /// <summary>
        /// 라이브 경매 여부 (Y/N)
        /// </summary>
        [JsonProperty("live_yn")]
        public string LiveYN { get; set; }

        /// <summary>
        /// 출품 작품 수
        /// </summary>
        [JsonProperty("work_count")]
        public int WorkCount { get; set; }

        /// <summary>
        /// 온라인 응찰 가능 여부 (Y/N)
        /// </summary>
        [JsonProperty("online_bid_yn")]
        public string OnlineBidYN { get; set; }

        /// <summary>
        /// 온라인 경매 입장 여부 (Y/N) 
        /// </summary>
        [JsonProperty("online_start_yn")]
        public string OnlineStartYN { get; set; }

        /// <summary>
        /// 라이브경매 상태 코드 (W-신청받을수 있음/A-입장/S-시작/P-휴식/F-마감/E-종료)
        /// </summary>
        [JsonProperty("auc_stat_cd")]
        public string AucStatCd { get; set; }

        #endregion

        #region # AuctionWorkType #

        [JsonProperty("work_type")]
        public string WorkType { get; set; }

        #endregion

        #region # AuctionWork #

        [JsonProperty("price_estimated_low")]
        public decimal PriceEstimatedLow { get; set; }

        [JsonProperty("price_estimated_low_min")]
        public decimal PriceEstimatedLowMin { get; set; }

        [JsonProperty("price_estimated_low_max")]
        public decimal PriceEstimatedLowMax { get; set; }

        [JsonProperty("price_estimated_high")]
        public decimal PriceEstimatedHigh { get; set; }

        [JsonProperty("price_estimated_high_min")]
        public decimal PriceEstimatedHighMin { get; set; }

        [JsonProperty("price_estimated_high_max")]
        public decimal PriceEstimatedHighMax { get; set; }

        [JsonProperty("price_start")]
        public decimal PriceStart { get; set; }

        [JsonProperty("price_start_min")]
        public decimal PriceStartMin { get; set; }

        [JsonProperty("price_start_max")]
        public decimal PriceStartMax { get; set; }

        [JsonProperty("img_file_name")]
        public string ImgFileName { get; set; }

        #endregion

        #region # UI #

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("online_bid_period")]
        public string OnlineBidPeriod
        {
            get
            {
                //return IsKor 
                //    ? $"{AucStartDate:MM월 dd일} ({StringHelper.GetDay(AucStartDate)}) ~ {AucOnlineBidEndDate:MM월 dd일} ({StringHelper.GetDay(AucOnlineBidEndDate)}) {AucOnlineBidEndDate:HH시까지}" 
                //    : $"{AucStartDate: MM.dd} ~ {AucOnlineBidEndDate: MM.dd}";
                return IsKor
                    ? $"{AucOnlineBidEndDate:MM월 dd일} ({StringHelper.GetDay(AucOnlineBidEndDate)}) {AucOnlineBidEndDate:HH시mm분까지}"
                    : AucOnlineBidEndDate.ToString("f", DateTimeFormatInfo.InvariantInfo);
            }
        }

        [JsonProperty("auth_yn")]
        public string AuthYN { get; set; }

        #endregion
    }
}
