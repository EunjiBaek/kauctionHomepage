using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KA.Entities.Models.Auction
{
    public class Artist
    {
        [JsonIgnore]
        /// <summary>
        /// 작가 고유번호
        /// </summary>
        public int Uid { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 작가명 (국문)
        /// </summary>
        public string ArtistName { get; set; }
        
        [JsonIgnore]
        /// <summary>
        /// 작가명 (영문)
        /// </summary>
        public string ArtistNameEn { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 작가명 (중문)
        /// </summary>
        public string ArtistNameCn { get; set; }

        /// <summary>
        /// 국가
        /// </summary>
        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("birth_date")]
        /// <summary>
        /// 생년
        /// </summary>
        public string BirthDate { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 생년 표기 여부 (N이면 미표기)
        /// </summary>
        public string BirthDisplay { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 몰년
        /// </summary>
        public string DeadDate { get; set; }

        [JsonProperty("direct_date")]
        /// <summary>
        /// 생몰년
        /// </summary>
        public string DirectDate { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 생몰년 표기 변수
        /// </summary>
        public string DisplayBirth
        {
            get
            {
                if ((BirthDisplay != null && BirthDisplay.Equals("N")) || (string.IsNullOrWhiteSpace(DirectDate) && string.IsNullOrWhiteSpace(BirthDate) && string.IsNullOrWhiteSpace(DeadDate)))
                {
                    return string.Empty;
                }
                else
                {
                    return string.IsNullOrWhiteSpace(DirectDate) ? $"{BirthDate} ~ {DeadDate}" : DirectDate;
                }
            }
        }

        [JsonIgnore]
        /// <summary>
        /// 외부 링크
        /// </summary>
        public string ExternalLink { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 참고 정보
        /// </summary>
        public string ReferenceInfo { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 참고 정보(영문)
        /// </summary>
        public string ReferenceInfoEn { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 학력
        /// </summary>
        public string SchoolCareer { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 학력(영문)
        /// </summary>
        public string SchoolCareerEn { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 전시
        /// </summary>
        public string Exhibition { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 전시(영문)
        /// </summary>
        public string ExhibitionEn { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 기타 정보
        /// </summary>
        public string Etc { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 기타 정보(영문)
        /// </summary>
        public string EtcEn { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 표기 정보 코드 (EC:Etc, SC:SchoolCareer, EH:Exhibition, RI:ReferenceInfo, EL:ExternalLink)
        /// </summary>
        public string Display { get; set; }

        [JsonIgnore]
        public List<string> DisplayOption => Display != null ? Display.Split(',').ToList() : new List<string>();

        [JsonProperty("copyright")]
        /// <summary>
        /// 카파라이트
        /// </summary>
        public string Copyright { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 카피라이트 표기 여부
        /// </summary>
        public string CopyrightDisplayYN { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 등록일자
        /// </summary>
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 경매 목록 검색 패널에서 해당 경매 아티스트 작품 수 표기
        /// </summary>
        [JsonProperty("artist_count")]
        public int ArtistCount { get; set; }
    }
}
