using KA.Entities.Attributes;
using KA.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace KA.Entities.Models.Auction
{
    public class AuctionWork : Artist
    {
        #region # AuctionWork #

        /// <summary>
        /// 작품 고유번호
        /// </summary>
        [JsonProperty("uid")]
        public new int Uid { get; set; }

        /// <summary>
        /// 작품 코드
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 경매 유형
        /// </summary>
        [JsonProperty("auc_kind")]
        public string AucKind { get; set; }

        /// <summary>
        /// 경매 유형 이름
        /// </summary>
        [JsonIgnore]
        public string AucKindName => string.IsNullOrWhiteSpace(AucKind) ? string.Empty : GetAucKindName(AucKind);

        /// <summary>
        /// 경매 유형 단축 이름 (메이저/프리미엄/위클리)
        /// </summary>
        [JsonIgnore]
        public string AucKindShortName { get; set; }

        /// <summary>
        /// 경매 고유번호
        /// </summary>
        [JsonProperty("auc_num")]
        public int AucNum { get; set; }

        /// <summary>
        /// Lot 번호
        /// </summary>
        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        /// <summary>
        /// 섹션 고유번호
        /// </summary>
        [JsonProperty("work_type_uid")]
        public int WorkTypeUid { get; set; }

        /// <summary>
        /// 카테고리 번호
        /// </summary>
        [JsonIgnore]
        public string CategoryNum { get; set; }

        /// <summary>
        /// 작가 고유번호
        /// </summary>
        [JsonProperty("artist_uid")]
        public int ArtistUid { get; set; }

        /// <summary>
        /// (삭제예정)
        /// </summary>
        //[JsonProperty("artist")]
        //public string Artist { get; set; }

        /// <summary>
        /// 작가명 (다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("artist_name")]
        public string DisplayArtistName => !IsKor && !string.IsNullOrWhiteSpace(ArtistNameEn) ? ArtistNameEn : ArtistName;

        /// <summary>
        /// 참고 정보(다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("reference_info")]
        public string DisplayReferenceInfo => !IsKor && !string.IsNullOrWhiteSpace(ReferenceInfoEn) ? ReferenceInfoEn : ReferenceInfo;

        /// <summary>
        /// 학력(다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("school_career")]
        public string DisplaySchoolCareer => !IsKor && !string.IsNullOrWhiteSpace(SchoolCareerEn) ? SchoolCareerEn : SchoolCareer;

        /// <summary>
        /// 전시(다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("exhibition")]
        public string DisplayExhibition => !IsKor && !string.IsNullOrWhiteSpace(ExhibitionEn) ? ExhibitionEn : Exhibition;

        /// <summary>
        /// 기타 정보(다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("etc")]
        public string DisplayEtc => !IsKor && !string.IsNullOrWhiteSpace(EtcEn) ? EtcEn : Etc;

        /// <summary>
        /// 작가 부가 정보
        /// </summary>
        [JsonIgnore]
        public string ArtistExtraInfo
        {
            get
            {
                if (CategoryNum != null && (CategoryNum.Equals("030000") || CategoryNum.Equals("040000") || CategoryNum.Equals("050000") || CategoryNum.Equals("060000") || CategoryNum.Equals("070000") || CategoryNum.Equals("090000")))
                {
                    if (!string.IsNullOrWhiteSpace(ArtistNameCn))
                    {
                        if (!string.IsNullOrWhiteSpace(DisplayBirth))
                        {
                            return string.Format("({0}, {1})", ArtistNameCn, DisplayBirth + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                        }
                        else
                        {
                            return string.Format("({0})", ArtistNameCn + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(DisplayBirth))
                        {
                            return string.Format("({0})", DisplayBirth + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                        }
                        else
                        {
                            return (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality);
                        }
                    }
                }
                else if (CategoryNum != null && CategoryNum.Equals("020000"))
                {
                    if (!string.IsNullOrWhiteSpace(ArtistNameEn) && IsKor)
                    {
                        if (!string.IsNullOrWhiteSpace(DisplayBirth))
                        {
                            return string.Format("({0}, {1})", ArtistNameEn, DisplayBirth + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                        }
                        else
                        {
                            return string.Format("({0})", ArtistNameEn + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(DisplayBirth))
                        {
                            return string.Format("({0})", DisplayBirth + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                        }
                        else
                        {
                            return (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(DisplayBirth))
                    {
                        return string.Format("({0})", DisplayBirth + (string.IsNullOrWhiteSpace(Nationality) ? "" : " " + Nationality));
                    }
                    else
                    {
                        return (string.IsNullOrWhiteSpace(Nationality) ? "" : Nationality);
                    }
                }
            }
        }

        /// <summary>
        /// 기타 정보 펼쳐보기 여부
        /// </summary>
        [JsonIgnore]
        public string EtcAllYN { get; set; }

        /// <summary>
        /// 기타 정보 펼쳐보기 여부
        /// </summary>
        [JsonIgnore]
        public string EtcDisplayAllYN => !string.IsNullOrWhiteSpace(Etc) ? EtcAllYN : "N";

        /// <summary>
        /// 작품 정보
        /// </summary>
        [JsonIgnore]
        public string Title { get; set; }

        /// <summary>
        /// 작품 정보 (영문)
        /// </summary>
        [JsonIgnore]
        public string TitleEn { get; set; }

        /// <summary>
        /// 작품 정보 (다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("title")]
        public string DisplayTitle => !IsKor && !string.IsNullOrWhiteSpace(TitleEn) ? TitleEn : Title;

        /// <summary>
        /// 작품 크기
        /// </summary>
        [JsonIgnore]
        public string Size { get; set; }

        /// <summary>
        /// 작품 크기 (영문)
        /// </summary>
        [JsonIgnore]
        public string SizeEn { get; set; }

        /// <summary>
        /// 작품 크기 (다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("size")]
        public string DisplaySize => !IsKor && !string.IsNullOrWhiteSpace(SizeEn) ? SizeEn : Size;

        /// <summary>
        /// 작품 크기 (가로)
        /// </summary>
        [JsonProperty("size_width")]
        public string SizeWidth { get; set; }

        /// <summary>
        /// 뷰인룸 활성화 시 SizeWidth 값 리턴
        /// </summary>
        [JsonProperty("work_width")]
        public string WorkWidth => !string.IsNullOrWhiteSpace(ViewInRoomYN) && ViewInRoomYN.Equals("Y") ? SizeWidth : "";

        /// <summary>
        /// 작품 크기 (세로)
        /// </summary>
        [JsonProperty("size_length")]
        public string SizeLength { get; set; }

        /// <summary>
        /// 뷰인룸 활성화 시 SizeLength 값 리턴
        /// </summary>
        [JsonProperty("work_length")]
        public string WorkLength => !string.IsNullOrWhiteSpace(ViewInRoomYN) && ViewInRoomYN.Equals("Y") ? SizeLength : "";

        /// <summary>
        /// Edition
        /// </summary>
        [JsonProperty("edition")]
        public string Edition { get; set; }

        /// <summary>
        /// Edition
        /// </summary>
        [JsonProperty("display_edition")]
        public string DisplayEdition => !string.IsNullOrWhiteSpace(Edition) ? string.Format(" ({0})", Edition) : "";

        /// <summary>
        /// 작품 설명
        /// </summary>
        [JsonIgnore]
        public string Desc { get; set; }

        /// <summary>
        /// 작품 설명 (영문)
        /// </summary>
        [JsonIgnore]
        public string DescEn { get; set; }

        /// <summary>
        /// 작품 추가정보(한글)
        /// </summary>
        [JsonIgnore]
        public string WorkTextKo { get; set; }

        /// <summary>
        /// 작품 추가정보(영문)
        /// </summary>
        [JsonIgnore]
        public string WorkTextEn { get; set; }

        /// <summary>
        /// 작품 설명 (다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("desc")]
        public string DisplayDesc => (IsKor ? string.Concat(string.IsNullOrWhiteSpace(Desc)
                                                            ? "" : string.Format("{0}</br/>", Desc), WorkTextKo)
                                            : string.Concat(string.IsNullOrWhiteSpace(!string.IsNullOrWhiteSpace(DescEn) ? DescEn : Desc)
                                                            ? "" : string.Format("{0}</br/>", !string.IsNullOrWhiteSpace(DescEn) ? DescEn : Desc), !string.IsNullOrWhiteSpace(WorkTextEn) ? WorkTextEn : WorkTextKo))
                                        .Replace("</div>", "</p>")
                                        .Replace("<div", "<p")
                                        .Replace("\n", "<br />");

        /// <summary>
        /// 기타 정보 펼쳐보기 여부
        /// </summary>
        [JsonIgnore]
        public string WorkTextKoAllYN { get; set; }

        /// <summary>
        /// 기타 정보 펼쳐보기 여부
        /// </summary>
        [JsonIgnore]
        public string WorkTextKoDisplayAllYN => !string.IsNullOrWhiteSpace(WorkTextKo) ? WorkTextKoAllYN : "N";

        /// <summary>
        /// 재료
        /// </summary>
        [JsonIgnore]
        public string Material { get; set; }

        /// <summary>
        /// 재료 (영문)
        /// </summary>
        [JsonIgnore]
        public string MaterialEn { get; set; }

        /// <summary>
        /// 재료 (다국어 선택 조건 처리)
        /// </summary>
        [JsonProperty("material")]
        public string DisplayMaterial => !IsKor && !string.IsNullOrWhiteSpace(MaterialEn) ? MaterialEn : Material;

        /// <summary>
        /// 제작연도
        /// </summary>
        [JsonIgnore]
        public string MakeDate { get; set; }

        /// <summary>
        /// 제작연도
        /// </summary>
        [JsonIgnore]
        public string MakeDateEn { get; set; }

        /// <summary>
        /// 제작연도 (다국어 선택 조건 처리
        /// </summary>
        [JsonProperty("make_date")]
        public string DisplayMakeDate => !IsKor && !string.IsNullOrWhiteSpace(MakeDateEn) ? MakeDateEn : MakeDate;

        [JsonIgnore]
        public string Condition { get; set; }

        [JsonProperty("condition")]
        public string DisplayCondition => string.IsNullOrWhiteSpace(Condition) ? "" : Condition.Trim().Replace("\n", "<br />");

        /// <summary>
        /// 낮은 추정가
        /// </summary>
        [JsonProperty("price_estimated_low")]
        public decimal PriceEstimatedLow { get; set; }

        /// <summary>
        /// 낮은 추정가 (검색결과 최소값)
        /// </summary>
        [JsonProperty("price_estimated_low_min")]
        public decimal PriceEstimatedLowMin { get; set; }

        /// <summary>
        /// 낮은 추정가 (검색결과 최대값)
        /// </summary>
        [JsonProperty("price_estimated_low_max")]
        public decimal PriceEstimatedLowMax { get; set; }

        /// <summary>
        /// 높은 추정가
        /// </summary>
        [JsonProperty("price_estimated_high")]
        public decimal PriceEstimatedHigh { get; set; }

        /// <summary>
        /// 높은 추정가 (검색결과 최소값)
        /// </summary>
        [JsonProperty("price_estimated_high_min")]
        public decimal PriceEstimatedHighMin { get; set; }

        /// <summary>
        /// 높은 추정가 (검색결과 최대값)
        /// </summary>
        [JsonProperty("price_estimated_high_max")]
        public decimal PriceEstimatedHighMax { get; set; }

        /// <summary>
        /// 추정가
        /// </summary>
        [JsonProperty("price_estimated_format")]
        public string PriceEstimatedFormat
        {
            get
            {
                return string.Format("{0} ~ {1}", PriceEstimatedLow.ToString("##0,0"), PriceEstimatedHigh.ToString("##0,0"));
            }
        }

        /// <summary>
        /// 시작가
        /// </summary>
        [JsonProperty("price_start")]
        public decimal PriceStart { get; set; }

        /// <summary>
        /// 시작가 (검색결과 최소값)
        /// </summary>
        [JsonProperty("price_start_min")]
        public decimal PriceStartMin { get; set; }

        /// <summary>
        /// 시작가 (검색결과 최대값)
        /// </summary>
        [JsonProperty("price_start_max")]
        public decimal PriceStartMax { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        [JsonProperty("price_max")]
        public decimal PriceMax { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        [JsonProperty("price_max_format")]
        public string PriceMaxFormat
        {
            get
            {
                return PriceMax.ToString("#,##0");
            }
        }

        [JsonProperty("display_price_max")]
        public string DisplayPriceMax => (IsKor ? "현재가" : "Current") + (PriceMax.Equals(0) ? $"&#xa;{Currency} {StringHelper.GetMoneyFormat(PriceStart)}" : $"&#xa;{Currency} {StringHelper.GetMoneyFormat(PriceMax)}");

        /// <summary>
        /// 현재가 표기 방식 (응찰이 없을때는 - 표기 처리)
        /// - 2021.05.06 : 응찰이 없을때는 시작가 표기 처리
        /// </summary>
        [JsonProperty("display_price_max_home")]
        // public string DisplayPriceMaxHome => PriceMax.Equals(0) ? "-" : $"{StringHelper.GetMoneyFormat(PriceMax)}";
        public string DisplayPriceMaxHome => PriceMax > 0 ? $"{StringHelper.GetMoneyFormat(PriceMax)}" : $"{StringHelper.GetMoneyFormat(PriceStart)}";

        /// <summary>
        /// 응찰단위
        /// </summary>
        [JsonProperty("price_ascend")]
        public decimal PriceAscend { get; set; }

        /// <summary>
        /// 예상 낙찰 수수료
        /// </summary>
        [JsonProperty("price_comm")]
        public decimal PriceComm { get; set; }

        /// <summary>
        /// 낙찰가
        /// </summary>
        [JsonProperty("price_hammer")]
        public decimal PriceHammer { get; set; }

        /// <summary>
        /// 낙찰가(콤마 형식)
        /// </summary>
        [JsonProperty("price_hammer_format")]
        public string PriceHammerFormat
        {
            get
            {
                return PriceHammer.ToString("#,##0");
            }
        }

        /// <summary>
        /// 시작 시간
        /// </summary>
        [JsonProperty("start_time")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 마감 시간
        /// </summary>
        [JsonProperty("end_time")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 마감 시간 (UI 처리)
        /// </summary>
        [JsonIgnore]
        public string DisplayEndTime
        {
            get
            {
                return EndTime.Second > 0 ? EndTime.ToString("MM/dd - hh:mm:ss tt", new CultureInfo("en-US")) : EndTime.ToString("MM/dd - hh:mm tt", new CultureInfo("en-US"));
            }
        }

        /// <summary>
        /// 마감 시간 (UI 처리)
        /// </summary>
        [JsonProperty("horizonal_end_time")]
        public string HorizonalEndTime
        {
            get
            {
                return EndTime.ToString("yyyy-MM-dd HH:mm");
            }
        }

        /// <summary>
        /// 응찰 홧수
        /// </summary>
        [JsonProperty("bid_cnt")]
        public int BidCnt { get; set; }

        /// <summary>
        /// 노출 여부
        /// </summary>
        [JsonProperty("view_yn")]
        public string ViewYN { get; set; }

        /// <summary>
        /// 출품 여부
        /// </summary>
        [JsonProperty("exhi_yn")]
        public string ExhiYN { get; set; }

        /// <summary>
        /// 숨김 여부
        /// </summary>
        [JsonProperty("hide_yn")]
        public string HideYN { get; set; }

        /// <summary>
        /// 마감 처리 여부 
        /// </summary>
        [JsonProperty("finish_yn")]
        public string FinishYN { get; set; }

        /// <summary>
        /// 썸네일 이미지
        /// </summary>
        [JsonProperty("thum_file_name")]
        public string ThumFileName { get; set; }

        /// <summary>
        /// 썸네일 이미지 URL
        /// </summary>
        [JsonProperty("thum_file_url")]
        public string ThumFileURL { get; set; }

        /// <summary>
        /// 작품 이미지
        /// </summary>
        [JsonProperty("img_file_name")]
        public string ImgFileName { get; set; }

        /// <summary>
        /// 작품 이미지 URL
        /// </summary>
        [JsonProperty("img_file_url")]
        public string ImgFileURL { get; set; }
        /// <summary>
        /// 경매 기간 여부 (종료 전이면 Y)
        /// </summary>
        [JsonProperty("live_yn")]
        public string LiveYN { get; set; }

        /// <summary>
        /// 별도문의 여부
        /// </summary>
        [JsonProperty("separate_inquiry_yn")]
        public string SeparateInquiryYN { get; set; }

        /// <summary>
        /// 국세청 위탁작품(임직원 응찰불가 작품) 여부
        /// </summary>
        [JsonProperty("con_nts_yn")]
        public string ConNTSYN { get; set; }

        /// <summary>
        /// 낙찰자 고유번호
        /// </summary>
        [JsonProperty("nak_mem_uid")]
        public int NakMemUid { get; set; }

        /// <summary>
        /// 낙찰자명
        [JsonIgnore]
        public string NakMemName { get; set; }

        [JsonProperty("nak_mem_name")]
        public string DisplayNakMemName => StringHelper.GetPrivateInfoMask(NakMemName, "N");

        /// <summary>
        /// 낙찰자 이메일 주소
        /// </summary>
        [JsonIgnore]
        public string NakMemEmail { get; set; }

        [JsonProperty("nak_mem_email")]
        public string DisplayNakMemEmail => StringHelper.GetPrivateInfoMask(NakMemEmail, "E");

        /// <summary>
        /// 낙찰자 모바일
        /// </summary>
        [JsonIgnore]
        public string NakMemMobile { get; set; }

        [JsonProperty("nak_mem_mobile")]
        public string DisplayNakMemMobile => StringHelper.GetPrivateInfoMask(NakMemMobile, "M");

        [JsonProperty("nak_mng_name")]
        public string NakMngName { get; set; }

        /// <summary>
        /// 로그인 회원 응찰 홧수
        /// </summary>
        [JsonIgnore]
        public int MemBidCnt { get; set; }

        [JsonIgnore]
        public string WorkInfo
        {
            get
            {
                var data = DisplayMakeDate;

                if (!string.IsNullOrWhiteSpace(data)) data += ", ";
                data += DisplayMaterial;

                if (!string.IsNullOrWhiteSpace(data)) data += ", ";
                data += DisplaySize;

                return data;
            }
        }

        /// <summary>
        /// 작품이미지 축소 여부 플래그
        /// </summary>
        [JsonProperty("minimal_image_yn")]
        public string MinimalImageYN { get; set; }

        /// <summary>
        /// 뷰인룸 활성화 여부 플래그
        /// </summary>
        [JsonProperty("view_in_room_yn")]
        public string ViewInRoomYN { get; set; }

        /// <summary>
        /// 목록 카드 배지 (국문)
        /// </summary>
        [JsonIgnore]
        public string Badge { get; set; }

        /// <summary>
        /// 목록 카드 배지 (영문)
        /// </summary>
        [JsonIgnore]
        public string BadgeEn { get; set; }

        [JsonProperty("badge")]
        public string DisplayBadge => !IsKor && !string.IsNullOrWhiteSpace(BadgeEn) ? BadgeEn : Badge;

        /// <summary>
        /// 목록 카드 배지 상세 정보 (국문)
        /// </summary>
        [JsonIgnore]
        public string BadgeDesc { get; set; }

        /// <summary>
        /// 목록 카드 배지 상세 정보 (영문)
        /// </summary>
        [JsonIgnore]
        public string BadgeDescEn { get; set; }

        [JsonProperty("badge_desc")]
        public string DisplayBadgeDesc => !IsKor && !string.IsNullOrWhiteSpace(BadgeDescEn) ? BadgeDescEn : BadgeDesc;

        #endregion

        #region # WishWork #

        [JsonProperty("wish_count")]
        public int WishCount { get; set; }

        [JsonProperty("wish_reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime WishRegDate { get; set; }

        #endregion

        #region # AuctionSchedule #        

        [JsonIgnore]
        public string AucTitle { get; set; }

        [JsonIgnore]
        public string AucTitleEn { get; set; }

        [JsonProperty("auc_title")]
        public string DisplayAucTitle => !IsKor ? AucTitleEn : AucTitle;

        [JsonIgnore]
        public string DisplayAucShortTitle => MessageHelper.GetShortTitleFromAucKind(AucKind, !IsKor ? "ENG" : "KOR");

        [JsonProperty("auc_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucDate { get; set; }

        [JsonProperty("auc_start_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucStartDate { get; set; }

        [JsonProperty("auc_end_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucEndDate { get; set; }

        [JsonProperty("auc_preview_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucPreviewDate { get; set; }

        [JsonIgnore]
        public string DisplayPreviewDate => !IsKor ? string.Format("{0:D}", AucPreviewDate) : AucPreviewDate.ToString("yyyy년 MM월 dd일");

        [JsonIgnore]
        // public string DisplayPreviewTime => !IsKor ? string.Format("{0:t}", AucPreviewDate) : (AucPreviewDate.Minute > 0 ? AucPreviewDate.ToString("tt HH시 mm분") : AucPreviewDate.ToString("tt HH시"));
        public string DisplayPreviewTime => !IsKor ? string.Format("{0:t}", AucPreviewDate) : (AucPreviewDate.Minute > 0 ? AucPreviewDate.ToString("HH시 mm분") : AucPreviewDate.ToString("HH시"));

        [JsonProperty("auc_bid_end_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucBidEndDate { get; set; }

        [JsonProperty("auc_online_bid_end_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime AucOnlineBidEndDate { get; set; }

        [JsonProperty("hongkong_yn")]
        public string HongkongYN { get; set; }

        #endregion

        #region # AuctionWorkCategoryDesc #

        /// <summary>
        /// 카테고리 별 설명 
        /// - 작품 상세 페이지 작품 설명 하단에 표기 처리
        /// </summary>
        [JsonProperty("category_desc")]
        public string DisplayCategoryDesc => IsKor ? CategoryDesc : CategoryDescEn;

        [JsonIgnore]
        public string CategoryDesc { get; set; }

        [JsonProperty]
        public string CategoryDescEn { get; set; }

        #endregion

        #region # UI #

        [JsonProperty("is_login")]
        public bool IsLogin { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("total_exhi_count")]
        public int TotalExhiCount { get; set; }

        [JsonProperty("card_message")]
        public string CardMessage
        {
            get
            {
                try
                {
                    if (AucKind == null) return string.Empty;

                    if (AucKind.Equals("1") && !LiveYN.Equals("Y"))
                    {
                        //if (IsLogin)
                        //{
                        //    if (PriceHammer > 0)
                        //    {
                        //        return IsKor ? "낙찰가 " + Currency + " " + StringHelper.GetMoneyFormat(PriceHammer) : "Hammer Price " + Currency + " " + StringHelper.GetMoneyFormat(PriceHammer);
                        //    }
                        //    else
                        //    {
                        //        return string.Empty;
                        //    }
                        //}
                        //else
                        //{
                        //    return IsKor ? "로그인 하시면 낙찰가를 확인하실 수 있습니다." : "Log in to view the auction result.";
                        //}
                        return string.Empty;
                    }
                    else
                    {
                        var tempAucDate = AucKind.Equals("1") ? AucBidEndDate : EndTime;
                        var tempEndText = AucKind.Equals("1") ? "신청마감" : "마감";
                        var tempEndEnText = AucKind.Equals("1") ? "Application Closes" : "Lot Closes";

                        if (tempAucDate.Second > 0)
                        {
                            return IsKor ? $"{tempEndText} {tempAucDate:MM/dd tt h:mm:ss}" : $"{tempEndEnText} {tempAucDate:MM/dd hh:mm:ss tt}";
                        }
                        else
                        {
                            return IsKor ? $"{tempEndText} {tempAucDate:MM/dd tt h:mm}" : $"{tempEndEnText} {tempAucDate:MM/dd hh:mm tt}";
                        }
                    }
                }
                catch (Exception) { return string.Empty; }
            }
        }

        [JsonProperty("currency")]
        public string Currency
        {
            get
            {
                return (HongkongYN != null && HongkongYN.Equals("Y")) ? "HKD" : "KRW";
            }
        }

        /// <summary>
        /// 경매 활성화 기간 (시작일과 종료일 사이 조건 만족)
        /// </summary>
        [JsonProperty("active_yn")]
        public string ActiveYN { get; set; }

        [JsonProperty("bid_yn")]
        public string BidYN { get; set; }

        [JsonProperty("preview_yn")]
        public string PreviewYN { get; set; }
        
        public bool PreviewTodayYN 
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PreviewYN) && PreviewYN.Equals("Y") && AucStartDate.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd"));
            }
        }

        [JsonProperty("work_link")]
        public string WorkLink
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ActiveYN) && (ActiveYN.Equals("Y") || PreviewYN.Equals("Y")) ? $"/Auction/{GetAucKindName(AucKind)}/{AucNum}/{Uid}" : string.Empty;
            }
        }

        [JsonProperty("wish_yn")]
        public string WishYN { get; set; }

        [JsonProperty("prev_uid")]
        public int PrevUid { get; set; }

        [JsonProperty("next_uid")]
        public int NextUid { get; set; }

        [JsonProperty("auc_rank")]
        public int AucRank { get; set; }

        [JsonProperty("disp_type")]
        public string DispType { get; set; }

        [JsonProperty("admin_yn")]
        public string AdminYN { get; set; }

        /// <summary>
        /// 통합 검색 시 경매별 결과 건수 정보
        /// </summary>
        [JsonProperty("search_result")]
        public string SearchResult { get; set; }

        /// <summary>
        /// 메인 하이라이트 웹파트 클릭 수 정보
        /// </summary>
        [JsonProperty("click_count")]
        public int ClickCount { get; set; }

        /// <summary>
        /// 목록 페이지 최대 LotNum 표기
        /// </summary>
        [JsonProperty("max_lot_num")]
        public int MaxLotNum { get; set; }

        #endregion

        #region # Function #

        public string GetAucKindName(string aucKind)
        {
            if (aucKind.Equals("2")) return "Premium";
            else if (aucKind.Equals("4")) return "Weekly";
            else return "Major";
        }

        #endregion
    }
}
