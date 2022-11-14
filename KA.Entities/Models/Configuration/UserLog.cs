using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Configuration
{
    public class UserLog
    {
        #region # homepage_web_call_log #

        [JsonProperty("user_id")]
        public int UserID { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("referer")]
        public string Referer { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        #endregion

        #region # tbl_page_print_hst #

        [JsonProperty("ow_uid")]
        public int OwUid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("mng_uid")]
        public int MngUid { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("test_yn")]
        public string TestYn { get; set; }

        [JsonProperty("print_yn")]
        public string PrintYn { get; set; }

        [JsonProperty("del_yn")]
        public string DelYn { get; set; }

        [JsonProperty("print_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime PrintDate { get; set; }

        #endregion

        #region # homepage_error_log #

        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("menu")]
        public string Menu { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("stacktrace")]
        public string StackTrace { get; set; }

        #endregion

        #region # tbl_member_mobile_auth #

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("auth")]
        public string Auth { get; set; }

        [JsonProperty("type2")]
        public string Type2 { get; set; }

        [JsonProperty("type_detail")]
        public string TypeDetail { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        [JsonProperty("mobile_code")]
        public string MobileCode { get; set; }

        [JsonProperty("mobile_co")]
        public string MobileCo { get; set; }

        [JsonProperty("mobile_no")]
        public string MobileNo { get; set; }

        [JsonProperty("CrdCd")]
        public string CrdCd { get; set; }

        [JsonProperty("id")]
        public string Di { get; set; }

        [JsonProperty("AuthNo")]
        public string AuthNo { get; set; }

        [JsonProperty("auth_end_time")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime AuthEndTime { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("migration")]
        public string Migration { get; set; }

        #endregion

        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime RegDate { get; set; }

        [JsonProperty("mod_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime ModDate { get; set; }

        public int TotalCount { get; set; }
    }
}
