using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.07.05 :: [#698/현업요청] 위탁문의 관련 권한 및 검토프로세스 반영 :: ReceiptYn, RecommendedPrice, Memo, MemoSalesTeam 변수 추가
    /// 2022.07.06 :: [#698/현업요청] 위탁문의 관련 권한 및 검토프로세스 반영 :: MngEmail 변수 추가 (메일발송처리)
    /// </summary>
    public class MemberConsign
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 위탁인 정보 - 고유번호
        /// </summary>
        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        /// <summary>
        /// 위탁인 정보 - 고유번호(케이오피스)
        /// </summary>
        [JsonProperty("koffice_mem_uid")]
        public int KofficeMemUid { get; set; }

        /// <summary>
        /// 위탁인 정보 - 이름
        /// </summary>
        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        /// <summary>
        /// 위탁인 정보 - 휴대전화
        /// </summary>
        [JsonProperty("mem_mobile")]
        public string MemMobile { get; set; }

        /// <summary>
        /// 위탁인 정보 - 이메일
        /// </summary>
        [JsonProperty("mem_email")]
        public string MemEmail { get; set; }

        /// <summary>
        /// 위탁인 정보 - 주소(우편번호)
        /// </summary>
        [JsonProperty("mem_zipcode")]
        public string MemZipCode { get; set; }

        /// <summary>
        /// 위탁인 정보 - 주소(기본주소)
        /// </summary>
        [JsonProperty("mem_address")]
        public string MemAddress { get; set; }

        /// <summary>
        /// 위탁인 정보 - 주소(상세주소)
        /// </summary>
        [JsonProperty("mem_address2")]
        public string MemAddress2 { get; set; }

        [JsonProperty("type_cd")]
        public string TypeCD { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작가명
        /// </summary>
        [JsonProperty("artist")]
        public string Artist { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품명
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 재료(기타)
        /// </summary>
        [JsonProperty("material")]
        public string Material { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 재료 코드
        /// </summary>
        [JsonProperty("material_code")]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 재료명
        /// </summary>
        [JsonProperty("material_name")]
        public string MaterialName { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 크기(가로)
        /// </summary>
        [JsonProperty("work_x")]
        public decimal WorkX { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 크기(세로)
        /// </summary>
        [JsonProperty("work_y")]
        public decimal WorkY { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 크기(폭)
        /// </summary>
        [JsonProperty("work_z")]
        public decimal WorkZ { get; set; }

        /// <summary>
        /// 위탁작품 정보 - 작품의 크기(호)
        /// </summary>
        [JsonProperty("ho")]
        public int Ho { get; set; }

        [JsonProperty("edition")]
        public string Edition { get; set; }

        [JsonProperty("make_date")]
        public string MakeDate { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("etc")]
        public string Etc { get; set; }

        [JsonProperty("price_purchase")]
        public decimal PricePurchase { get; set; }

        [JsonProperty("price_desired")]
        public decimal PriceDesired { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonIgnore]
        public string StateName { get; set; }

        [JsonIgnore]
        public string StateNameEn { get; set; }

        [JsonProperty("state_name")]
        public string DisplayStateName => !IsKor && !string.IsNullOrWhiteSpace(StateNameEn) ? StateNameEn : StateName;

        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonIgnore]
        public int AdminUid { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("no")]
        public int No { get; set; }

        [JsonProperty("images")]
        public string Images { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("consign_images")]
        public string ConsignImages { get; set; }

        /// <summary>
        /// 담당자 이름
        /// </summary>
        [JsonProperty("mng_name")]
        public string MngName { get; set; }

        /// <summary>
        /// 담당자 이메일
        /// </summary>
        [JsonProperty("mng_email")]
        public string MngEmail { get; set; }

        /// <summary>
        /// 입고여부 (Y:적합,N:비적합)
        /// </summary>
        [JsonProperty("receipt_yn")]
        public string ReceiptYn { get; set; }

        /// <summary>
        /// 권고가
        /// </summary>
        [JsonProperty("recommended_price")]
        public Decimal RecommendedPrice { get; set; }

        /// <summary>
        /// 메모(내부용)
        /// </summary>
        [JsonProperty("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 접수 메모 (내부용)
        /// </summary>
        [JsonProperty("memo_sales_team")]
        public string MemoSalesTeam { get; set; }

        [JsonProperty("review_uid")]
        public int ReviewUid { get; set; }

        [JsonProperty("review_name")]
        public string ReviewName { get; set; }

        [JsonProperty("review_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ReviewDate { get; set; }

        [JsonProperty("review_sales_team_uid")]
        public int ReviewSalesTeamUid { get; set; }

        [JsonProperty("review_sales_team_name")]
        public string ReviewSalesTeamName { get; set; }

        [JsonProperty("review_sales_team_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ReviewSalesTeamDate { get; set; }

        [JsonProperty("reg_date_full")] 
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDateFull => RegDate;
    }
}
