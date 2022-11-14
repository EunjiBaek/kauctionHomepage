using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberCertificateHistory
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("work_uid")]
        public int WorkUid { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("test_mode")]
        public string TestMode { get; set; }

        [JsonProperty("print_yn")]
        public string PrintYN { get; set; }

        [JsonProperty("del_yn")]
        public string DelYN { get; set; }

        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        [JsonProperty("reg_date")]
        public DateTime RegDate { get; set; }

        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        [JsonProperty("print_date")]
        public DateTime PrintDate { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        [JsonProperty("work_title")]
        public string WorkTitle { get; set; }

        [JsonProperty("auc_title")]
        public string AucTitle { get; set; }

        [JsonProperty("read_cnt")]
        public int ReadCnt { get; set; }

        [JsonProperty("test_print_cnt")]
        public int TestPrintCnt { get; set; }

        [JsonProperty("print_cnt")]
        public int PrintCnt { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
