using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.06.14
    /// </summary>
    public class MemberApplyBook
    {
        [JsonProperty("no")]
        public int No { get; set; }

        /// <summary>
        /// 고유번호
        /// </summary>
        [JsonProperty("uid")]
        public int Uid { get; set; }

        /// <summary>
        /// 신청인 정보 - 홈페이지 고유번호
        /// </summary>
        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        /// <summary>
        /// 신청인 정보 - 케이오피스 고유번호
        /// </summary>
        [JsonProperty("koffice_uid")]
        public int KofficeUid { get; set; }

        [JsonProperty("mem_id")]
        public string MemId { get; set; }

        /// <summary>
        /// 신청인 정보 - 이름
        /// </summary>
        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        /// <summary>
        /// 신청인 정보 - 이메일
        /// </summary>
        [JsonProperty("mem_email")]
        public string MemEmail { get; set; }

        /// <summary>
        /// 신청인 정보 - 모바일
        /// </summary>
        [JsonProperty("mem_mobile")]
        public string MemMobile { get; set; }

        /// <summary>
        /// 신청인 정보 - 회원 유형 코드
        /// </summary>
        [JsonProperty("mem_type")]
        public string MemType { get; set; }

        /// <summary>
        /// 신청인 정보 - 회원 유형
        /// </summary>
        [JsonProperty("mem_type_name")]
        public string MemTypeName { get; set; }

        /// <summary>
        /// 신청일자
        /// </summary>
        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 상태 (010:신청, 011:신청완료)
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// 케이오피스 도록회원 시작일
        /// </summary>
        [JsonProperty("mem_pay_s_date")]
        public string MemPaySDate { get; set; }

        /// <summary>
        /// 케이오피스 도록회원 여부 (Y/N)
        /// </summary>
        [JsonProperty("mem_catalogue_yn")]
        public string MemCatalogueYn { get; set; }

        /// <summary>
        /// 도록 수신주소 (우편번호)
        /// </summary>
        [JsonProperty("mem_zipcode")]
        public string MemZipCode { get; set; }

        /// <summary>
        /// 도록 수신주소 (주소)
        /// </summary>
        [JsonProperty("mem_address")]
        public string MemAddress { get; set; }

        /// <summary>
        /// 도록 수신주소 (상세주소)
        /// </summary>
        [JsonProperty("mem_address2")]
        public string MemAddress2 { get; set; }

        [JsonProperty("mem_receiver")]
        public string MemReceiver { get; set; }

        /// <summary>
        /// 승인일자
        /// </summary>
        [JsonProperty("apply_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime ApplyDate { get; set; }

        /// <summary>
        /// 도록 수신주소 (수신자)
        /// </summary>
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

    }
}
