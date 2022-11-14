using Newtonsoft.Json;

namespace KA.Entities.Models.Member
{
    public class MemberDailyAccessStatus
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("today_yn")]
        public string TodayYn { get; set; }

        [JsonProperty("login_count")]
        public int LoginCount { get; set; }

        [JsonProperty("login_member_count")]
        public int LoginMemberCount { get; set; }
    }
}
