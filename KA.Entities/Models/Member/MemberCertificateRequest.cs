using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberCertificateRequest
    {
        /// <summary>
        /// 고유번호
        /// </summary>
        [JsonProperty("uid")]
        public int Uid { get; set; }

        /// <summary>
        /// 회원 고유번호
        /// </summary>
        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        /// <summary>
        /// 회원 이름
        /// </summary>
        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        /// <summary>
        /// 작품 고유번호
        /// </summary>
        [JsonProperty("work_uid")]
        public int WorkUid { get; set; }

        /// <summary>
        /// 작품 명
        /// </summary>
        [JsonProperty("work_title")]
        public string WorkTitle { get; set; }

        /// <summary>
        /// 작품 Lot
        /// </summary>
        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        /// <summary>
        /// 경매 명
        /// </summary>
        [JsonProperty("auc_title")]
        public string AucTitle { get; set; }

        /// <summary>
        /// 인쇄 이력 고유번호 (케이오피스)
        /// </summary>
        [JsonProperty("print_hst")]
        public int PrintHst { get; set; }

        /// <summary>
        /// 주소 고유번호
        /// </summary>
        [JsonProperty("address_uid")]
        public int AddressUid { get; set; }

        /// <summary>
        /// 배송지 우편번호
        /// </summary>
        [JsonProperty("receiver_zipcode")]
        public string ReceiverZipcode { get; set; }

        /// <summary>
        /// 배송지 주소
        /// </summary>
        [JsonProperty("receiver_address")]
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 배송지 상세주소
        /// </summary>
        [JsonProperty("receiver_address2")]
        public string ReceiverAddress2 { get; set; }

        /// <summary>
        /// 수신자 이름
        /// </summary>
        [JsonProperty("receiver_name")]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 수신자 연락처
        /// </summary>
        [JsonProperty("receiver_mobile")]
        public string ReceiverMobile { get; set; }

        /// <summary>
        /// 수신자 이메일
        /// </summary>
        [JsonProperty("receiver_email")]
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// 이메일 주소 변경 여부(Y/N)
        /// </summary>
        [JsonProperty("email_flag")]
        public string EmailFlag { get; set; }
        
        /// <summary>
        /// 상태 코드
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// 상태 명
        /// </summary>
        [JsonProperty("state_name")]
        public string StateName { get; set; }

        /// <summary>
        /// 등록일자
        /// </summary>
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        [JsonProperty("reg_date")]
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 처리일자
        /// </summary>
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        [JsonProperty("proc_date")]
        public DateTime ProcDate { get; set; }

        public int TotalCount { get; set; }
    }
}
