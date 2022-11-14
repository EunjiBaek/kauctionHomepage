using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.05.16 :: AuthType 변수 추가 - 인증 타입 (M:모바일, E:이메일)
    /// 2022.07.27 :: [#667/계획] ISMS 국내 회원가입 본인인증 선택으로 변경+응찰필수조건처리 - RedirectUrl 변수 추가
    /// </summary>
    public class MemberMobileAuth
    {
        [JsonIgnore]
        public int Uid { get; set; }

        [JsonIgnore]
        public string Seq { get; set; }

        [JsonProperty("auth")]
        public string Auth { get; set; }

        [JsonProperty("auth_name")]
        public string AuthName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("type2")]
        public string Type2 { get; set; }

        [JsonProperty("type_detail")]
        public string TypeDetail { get; set; }

        [JsonProperty("type_name")]
        public string TypeName { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("state_name")]
        public string StateName { get; set; }

        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("device_name")]
        public string DeviceName { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public int MemUid { get; set; }

        [JsonIgnore]
        public string MemId { get; set; }

        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        [JsonProperty("mobile_code")]
        public string MobileCode { get; set; }

        [JsonProperty("mobile_co")]
        public string MobileCo { get; set; }

        [JsonProperty("mobile_co_name")]
        public string MobileCoName { get; set; }

        [JsonProperty("mobile_no")]
        public string MobileNo { get; set; }

        [JsonProperty("crd_cd")]
        public string CrdCD { get; set; }

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("di")]
        public string DI { get; set; }

        [JsonProperty("ci")]
        public string CI { get; set; }

        [JsonIgnore]
        public string CipherTime { get; set; }

        [JsonProperty("reg_date")]
        public DateTime RegDate { get; set; }

        [JsonProperty("mod_date")]
        public DateTime ModDate { get; set; }

        [JsonIgnore]
        public string CodeName { get; set; }

        [JsonIgnore]
        public string CodeNameEn { get; set; }

        [JsonIgnore]
        public string AuthType { get; set; }

        [JsonIgnore]
        public string AuthNo { get; set; }

        /// <summary>
        /// 인증 후 이동할 페이지 정보 변수
        /// </summary>
        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }
    }
}
