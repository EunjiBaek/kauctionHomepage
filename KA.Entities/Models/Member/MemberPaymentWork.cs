using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    public class MemberPaymentWork
    {
        [JsonProperty("ow_uid")]
        public int OwUid { get; set; }

        [JsonIgnore]
        public string AucTitle { get; set; }

        [JsonIgnore]
        public string AucTitleEn { get; set; }

        [JsonProperty("auc_title")]
        public string DisplayAucTitle => !IsKor && !string.IsNullOrWhiteSpace(AucTitleEn) ? AucTitleEn : AucTitle;

        [JsonProperty("auc_date")]
        public DateTime AucDate { get; set; }

        public string AucKind { get; set; }

        public int AucNum { get; set; }

        public int LotNum { get; set; }

        [JsonIgnore]
        public string WName { get; set; }

        [JsonIgnore]
        public string WNameEn { get; set; }

        [JsonProperty("w_name")]
        public string DisplayWName => !IsKor && !string.IsNullOrWhiteSpace(WNameEn) ? WNameEn : WName;

        [JsonIgnore]
        public string ThumFileName { get; set; }

        [JsonProperty("w_image")]
        public string WImage { get; set; }

        [JsonIgnore]
        public string AName { get; set; }

        [JsonIgnore]
        public string ANameEn { get; set; }

        [JsonProperty("a_name")]
        public string DisplayAName => !IsKor && !string.IsNullOrWhiteSpace(ANameEn) ? ANameEn : AName;

        [JsonProperty("price_successful_bid")]
        public decimal PriceSuccessfulBid { get; set; }

        [JsonProperty("buy_comm_sum")]
        public decimal BuyCommSum { get; set; }

        [JsonProperty("etc_fee")]
        public decimal EtcFee { get; set; }

        [JsonProperty("successful_bid_price")]
        public decimal SuccessfulBidPrice { get; set; }

        [JsonProperty("comm_price")]
        public decimal CommPrice { get; set; }

        [JsonProperty("delivery_fee_price")]
        public decimal DeliveryFeePrice { get; set; }

        [JsonProperty("receipt")]
        public string Receipt { get; set; }

        public int SuccessfulPersonUid { get; set; }

        public int TotalCount { get; set; }

        public bool IsKor { get; set; }
    }
}
