using Newtonsoft.Json;

namespace KA.Entities.Models.Member
{
    public class TermAndConditionDetail
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("vresion")]
        public int Version { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("code_name")]
        public string CodeName { get; set; }

        [JsonProperty("code_name_en")]
        public string CodeNameEn { get; set; }

        [JsonIgnore]
        public string DisplayCodeName => !IsKor && !string.IsNullOrWhiteSpace(CodeNameEn) ? CodeNameEn : CodeName;

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("content_en")]
        public string ContentEn { get; set; }

        [JsonIgnore]
        public string DisplayContent => !IsKor && !string.IsNullOrWhiteSpace(ContentEn) ? ContentEn : Content;

        [JsonProperty("login_yn")]
        public string LoginYn { get; set; }

        [JsonProperty("use_yn")]
        public string UseYn { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonIgnore]
        public string SubCode { get; set; }
    }
}
