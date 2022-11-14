using Newtonsoft.Json;

namespace KA.Entities.Models.Member
{
    public class MemberStatisticJoin
    {
        [JsonProperty("reg_year")]
        public int RegYear { get; set; }

        [JsonProperty("reg_month")]
        public int RegMonth { get; set; }

        [JsonProperty("reg_day")]
        public int RegDay { get; set; }

        [JsonProperty("display_date")]
        public string DisplayDate { get; set; }

        [JsonProperty("mem_type")]
        public string Type { get; set; }

        [JsonProperty("join_count")]
        public int JoinCount { get; set; }
    }
}
