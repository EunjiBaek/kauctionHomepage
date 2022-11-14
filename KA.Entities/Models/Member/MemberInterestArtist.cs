using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.05.10 :: ArtistReference 변수 추가 - 마이페이지 개편으로 인하여, 작가설명 SWIPE에 사용할 컨텐츠 추가
    /// 2022.05.20 :: ArtistDesc, Etc, EtcEn 변수 추가 - 작가 팔로우에 작가 설명 변수 추가
    /// 2022.05.25 :: ArtistPastWorkInfo 변수 추가 - 작가의 과거 낙찰 착품 변수 추가
    /// </summary>
    public class MemberInterestArtist
    {
        /// <summary>
        /// 작가 고유번호
        /// </summary>
        [JsonProperty("uid")]
        public int Uid { get; set; }

        /// <summary>
        /// 회원 고유번호
        /// </summary>
        [JsonIgnore]
        public int MemUid { get; set; }

        /// <summary>
        /// 등록일자
        /// </summary>
        [JsonProperty("reg_date")]
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 작가명(국문)
        /// </summary>
        [JsonIgnore]
        public string Name { get; set; }

        /// <summary>
        /// 작가명(영문)
        /// </summary>
        [JsonIgnore]
        public string NameEn { get; set; }

        [JsonProperty("name")]
        public string DisplayName => !IsKor && !string.IsNullOrWhiteSpace(NameEn) ? NameEn : Name;

        /// <summary>
        /// 생몰년
        /// </summary>
        [JsonProperty("direct_date")]
        public string DirectDate { get; set; }

        /// <summary>
        /// 작가 사진
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// 작가 사진 경로
        /// </summary>
        [JsonProperty("artist_file_url")]
        public string ArtistFileUrl { get; set; }

        [JsonProperty("artist_youtube")]
        public string ArtistYouTube { get; set; }

        /// <summary>
        /// 작가 참고자료
        /// 2022.05.10 :: 마이페이지 개편으로 인하여, 작가설명 SWIPE에 사용할 컨텐츠 추가
        /// </summary>
        [JsonProperty("artist_reference")]
        public string ArtistReference { get; set; }

        [JsonProperty("follow_yn")]
        public string FollowYn { get; set; }

        [JsonProperty("auc_work_info")]
        public string AucWorkInfo { get; set; }

        /// <summary>
        /// 작가의 과거 낙찰 작품
        /// </summary>
        [JsonProperty("auc_past_work_info")]
        public string AucPastWorkInfo { get; set; }

        [JsonProperty("artist_desc")]
        public string ArtistDesc { get; set; }

        [JsonIgnore]
        public string Etc { get; set; }

        [JsonIgnore]
        public string EtcEn { get; set; }


        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonIgnore]
        public int TotalCount { get; set; }
    }
}
