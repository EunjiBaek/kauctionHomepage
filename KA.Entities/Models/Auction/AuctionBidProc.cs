using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionBidProc
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("work_uid")]
        public int WorkUid { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("koffice_ow_uid")]
        public int KofficeOwUid { get; set; }

        [JsonProperty("koffice_mem_uid")]
        public int KofficeMemUid { get; set; }

        [JsonProperty("koffice_mng_uid")]
        public int KofficeMngUid { get; set; }

        [JsonProperty("proc_yn")]
        public string ProcYn { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        [JsonProperty("work_title")]
        public string WorkTitle { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("auc_title")]
        public string AucTitle { get; set; }

        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        [JsonProperty("koffice_mem_name")]
        public string KofficeMemName { get; set; }

        [JsonProperty("koffice_mng_name")]
        public string KofficeMngName { get; set; }

        [JsonProperty("org_mem_name")]
        public string OrgMemName { get; set; }

        [JsonProperty("before_mem_name")]
        public string BeforeMemName { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
