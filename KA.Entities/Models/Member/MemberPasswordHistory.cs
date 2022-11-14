using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.05.30 :: PwdNotiTarget, PwdNotiValue 변수 추가 - 관리자 비밀번호 변경 시 알림을 위한 별도 입력값 처리
    /// </summary>
    public class MemberPasswordHistory
    {
        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("type_name")]
        public string TypeName { get; set; }

        [JsonProperty("pwd")]
        public string Pwd { get; set; }

        [JsonProperty("pwd_new")]
        public string PwdNew { get; set; }

        /// <summary>
        /// 비밀번호 변경 시 알람대상 유형 (U:관리자 입력값, A:인증정보)
        /// </summary>
        [JsonProperty("pwd_noti_target")]
        public string PwdNotiTarget { get; set; }

        /// <summary>
        /// 비빌번호 변경 시 알람대상 값 (문자인 경우 관리자 입력 문자번호, 이메일인 경우 관리자 입력 메일주소)
        /// </summary>
        [JsonProperty("pwd_noti_value")]
        public string PwdNotiValue { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }
    }
}
