using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberInquiry
    {
        #region # table - tbl_member_inquiry #

        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("org_uid")]
        public int OrgUid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("work_uid")]
        public int WorkUid { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
        
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("contents")]
        public string Contents { get; set; }

        [JsonProperty("manager_uid")]
        public int ManagerUid { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonProperty("reg_type")]
        public string RegType { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("mail_yn")]
        public string MailYn { get; set; }

        [JsonProperty("mail_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime MailDate { get; set; }

        #endregion

        #region # UI #

        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonIgnore]
        public string TypeName { get; set; }

        [JsonIgnore]
        public string TypeNameEn { get; set; }

        [JsonProperty("type_name")]
        public string DisplayTypeName => !IsKor && !string.IsNullOrWhiteSpace(TypeNameEn) ? TypeNameEn : TypeName;

        [JsonIgnore]
        public string MemTypeName { get; set; }

        [JsonIgnore]
        public string MemTypeNameEn { get; set; }

        [JsonProperty("mem_type_name")]
        public string DisplayMemTypeName => !IsKor && !string.IsNullOrWhiteSpace(MemTypeNameEn) ? MemTypeNameEn : MemTypeName;

        [JsonIgnore]
        public string StateName { get; set; }

        [JsonIgnore]
        public string StateNameEn { get; set; }

        [JsonProperty("state_name")]
        public string DisplayStateName => !IsKor && !string.IsNullOrWhiteSpace(StateNameEn) ? StateNameEn : StateName;
        
        [JsonIgnore]
        private string CategoryName { get; set; }

        [JsonIgnore]
        private string CategoryNameEn { get; set; }

        [JsonProperty("category_name")]
        public string DisplayCategoryName => !IsKor && !string.IsNullOrWhiteSpace(CategoryName) ? CategoryNameEn : CategoryName;

        [JsonIgnore]
        public string Title { get; set; }

        [JsonIgnore]
        public string TitleEn { get; set; }

        [JsonProperty("title")]
        public string DisplayTitle => !IsKor && !string.IsNullOrWhiteSpace(TitleEn) ? TitleEn : Title;

        [JsonIgnore]
        public string ArtistName { get; set; }

        [JsonIgnore]
        public string ArtistNameEn { get; set; }

        [JsonProperty("artist_name")]
        public string DisplayArtistName => !IsKor && !string.IsNullOrWhiteSpace(ArtistNameEn) ? ArtistNameEn : ArtistName;

        [JsonIgnore]
        public string AucTitle { get; set; }

        [JsonIgnore]
        public string AucTitleEn { get; set; }

        [JsonProperty("auc_title")]
        public string DisplayAucTitle => !IsKor && !string.IsNullOrWhiteSpace(AucTitleEn) ? AucTitleEn : AucTitle;

        [JsonProperty("auc_kind")]
        public string AucKind { get; set; }

        [JsonProperty("auc_num")]
        public int AucNum { get; set; }

        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        [JsonProperty("img_file_name")]
        public string ImgFileName { get; set; }

        [JsonProperty("img_file_url")]
        public string ImgFileUrl { get; set; }

        [JsonProperty("mem_uid")]
        public int MemUid { get; set; }

        [JsonProperty("mem_name")]
        public string MemName { get; set; }

        [JsonProperty("koffice_uid")]
        public int KofficeUid { get; set; }

        [JsonProperty("mng_name")]
        public string MngName { get; set; }

        [JsonProperty("grp_name")]
        public string GrpName { get; set; }

        [JsonProperty("mem_email")]
        public string MemEmail { get; set; }

        [JsonProperty("mem_mobile")]
        public string MemMobile { get; set; }

        [JsonProperty("no")]
        public int No { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("reply_count")]
        public int ReplyCount { get; set; }

        #endregion
    }
}
