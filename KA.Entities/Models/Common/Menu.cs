using Newtonsoft.Json;

namespace KA.Entities.Models.Common
{
    public class Menu
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("sub_code")]
        public string SubCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_en")]
        public string NameEn { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("root_target")]
        public string RootTarget { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("auth_flag")]
        public string AuthFlag { get; set; }

        [JsonProperty("use_flag")]
        public string UseFlag { get; set; }

        [JsonProperty("login_yn")]
        public string LoginYN { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("option")]
        public string Option { get; set; }

        [JsonProperty("option2")]
        public string Option2 { get; set; }

        [JsonProperty("option3")]
        public string Option3 { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName => !string.IsNullOrWhiteSpace(Lang) && Lang.Equals("K") ? Name : NameEn;
    }
}
