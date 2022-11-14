using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionWorkByUser : AuctionSchedule
    {
        [JsonProperty("auc_rank")]
        public int AucRank { get; set; }

        [JsonProperty("work_seq")]
        public int WorkSeq { get; set; }

        [JsonProperty("ow_uid")]
        public int OwUid { get; set; }

        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        [JsonIgnore]
        public string WorkTitle { get; set; }

        [JsonIgnore]
        public string WorkTitleEn { get; set; }

        [JsonProperty("work_title")]
        public string DisplayWorkTitle => !IsKor && !string.IsNullOrWhiteSpace(WorkTitleEn) ? WorkTitleEn : WorkTitle;

        [JsonIgnore]
        public string Title { get; set; }

        [JsonIgnore]
        public string TitleEn { get; set; }

        [JsonProperty("title")]
        public string DisplayTitle => !IsKor && !string.IsNullOrWhiteSpace(TitleEn) ? TitleEn : Title;

        [JsonIgnore]
        public string MakeDate { get; set; }

        [JsonIgnore]
        public string MakeDateEn { get; set; }

        [JsonProperty("make_date")]
        public string DisplayMakeDate => IsKor ? MakeDate : MakeDateEn;

        [JsonIgnore]
        public string Material { get; set; }

        [JsonIgnore]
        public string MaterialEn { get; set; }

        [JsonProperty("material")]
        public string DisplayMaterial => IsKor ? Material : MaterialEn;

        [JsonIgnore]
        public string Size { get; set; }

        [JsonIgnore]
        public string SizeEn { get; set; }

        [JsonProperty("size")]
        public string DisplaySize => IsKor ? Size : SizeEn;

        [JsonProperty("edition")]
        public string Edition { get; set; }

        [JsonProperty("display_edition")]
        public string DisplayEdition => !string.IsNullOrWhiteSpace(Edition) ? string.Format(" ({0})", Edition) : "";

        [JsonProperty("bid_cnt")]
        public int BidCnt { get; set; }

        [JsonProperty("price_hammer")]
        public decimal PriceHammer { get; set; }

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        [JsonIgnore]
        public string ArtistName { get; set; }

        [JsonIgnore]
        public string ArtistNameEn { get; set; }

        [JsonProperty("artist_name")]
        public string DisplayArtistName => IsKor ? ArtistName : ArtistNameEn;

        [JsonProperty("price_bid")]
        public decimal PriceBid { get; set; }

        [JsonProperty("bid_type")]
        public string BidType { get; set; }

        [JsonProperty("bid_reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime BidRegDate { get; set; }

        [JsonProperty("nak_yn")]
        public string NakYN { get; set; }

        [JsonProperty("price_purchase")]
        public decimal PricePurchase => PriceHammer > 10000000 ? 10000000 * 0.198m + (PriceHammer - 10000000) * 0.165m : PriceHammer + (PriceHammer * Fees);

        /// <summary>
        /// 2018.04.02 00:00:00(마감하는 위클리 포함) 이후 19.8%, 이전 16.5% (16.5% < 2018.04.02 < 19.8%)
        /// 프리미엄 경매번호 78 이상, 위클리 경매번호 99 이상: 19.8%
        /// 프리미엄 경매번호 77 이하, 위클리 경매번호 98 이하: 16.5%
        /// </summary>
        [JsonProperty("fees")]
        // public decimal Fees => (AucKind.Equals("2") && AucNum < 78) || (AucKind.Equals("4") && AucNum < 99) ? 0.165m : 0.198m;
        public decimal Fees => PriceBid > 10000000 || (AucKind.Equals("2") && AucNum < 78) || (AucKind.Equals("4") && AucNum < 99) ? 0.165m : 0.198m;

        [JsonProperty("major_bid_type")]
        public string MajorBidType { get; set; }

        [JsonIgnore]
        public string MajorBidTypeName { get; set; }

        [JsonIgnore]
        public string MajorBidTypeNameEn { get; set; }

        [JsonProperty("major_bid_type_name")]
        public string DisplayMajorBidTypeName => !IsKor && !string.IsNullOrWhiteSpace(MajorBidTypeNameEn) ? MajorBidTypeNameEn : MajorBidTypeName;

        [JsonProperty("major_bp_state")]
        public string MajorBpState { get; set; }

        [JsonIgnore]
        public string MajorBpStateName { get; set; }

        [JsonIgnore]
        public string MajorBpStateNameEn { get; set; }

        [JsonProperty("major_bp_state_name")]
        public string DisplayMajorBpStateName => !IsKor && !string.IsNullOrWhiteSpace(MajorBpStateNameEn) ? MajorBpStateNameEn : MajorBpStateName;

        [JsonProperty("is_login")]
        public bool IsLogin { get; set; }

        [JsonProperty("work_link")]
        public string WorkLink
        {
            get
            {
                // 전체 작품 접근 가능하게 처리
                // return !string.IsNullOrWhiteSpace(ActiveYN) && ActiveYN.Equals("Y") ? $"/Auction/{GetAucKindName(AucKind)}/{AucNum}/{Uid}" : string.Empty;
                // return $"/Auction/{GetAucKindName(AucKind)}/{AucNum}/{Uid}";
                return string.Empty;
            }
        }

        [JsonProperty("end_yn")]
        public string EndYN { get; set; }

        [JsonProperty("price_max")]
        public decimal PriceMax { get; set; }

        [JsonIgnore]        
        public DateTime KofficePrintDate { get; set; }

        [JsonProperty("certificate_yn")]
        public string CertificateYN { get; set; }

        [JsonProperty("certificate_print_yn")]
        public string CertificatePrintYN { get; set; }

        [JsonProperty("certificate_print_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime CertificatePrintDate { get; set; }

        [JsonProperty("certificate_delivery_state")]
        public string CertificateDeliveryState { get; set; }

        [JsonProperty("certificate_delivery_state_name")]
        public string DisplayCertificateDeliveryStateName => IsKor ? CertificateDeliveryStateName : CertificateDeliveryStateNameEn;

        [JsonIgnore]
        public string CertificateDeliveryStateName { get; set; }

        [JsonIgnore]
        public string CertificateDeliveryStateNameEn { get; set; }

        [JsonProperty("certificate_delivery_reg_date")]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime CertificateDeliveryRegDate { get; set; }

        #region # Function #

        public string GetAucKindName(string aucKind)
        {
            if (aucKind.Equals("2")) return "Premium";
            else if (aucKind.Equals("4")) return "Weekly";
            else return "Major";
        }

        #endregion
    }
}
