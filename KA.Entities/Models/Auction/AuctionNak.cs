using Newtonsoft.Json;

namespace KA.Entities.Models.Auction
{
    public class AuctionNak
    {
        #region # AuctionWork_낙찰통보서#
        [JsonIgnore]
        public string AucDate { get; set; }

        [JsonIgnore]
        public string AucTitle { get; set; }

        [JsonIgnore]
        public string AucTitleEn { get; set; }

        [JsonProperty("lot_num")]
        public int LotNum { get; set; }

        [JsonIgnore]
        public string AName { get; set; }

        [JsonIgnore]
        public string ANameEn { get; set; }

        [JsonProperty("a_name")]
        public string DisplayAName => IsKor ? AName : string.IsNullOrWhiteSpace(ANameEn) ? AName : ANameEn;

        [JsonIgnore]
        public string WName { get; set; }

        [JsonIgnore]
        public string WNameEn { get; set; }

        [JsonProperty("w_name")]
        public string DisplayWName => IsKor ? WName : string.IsNullOrWhiteSpace(WNameEn) ? WName : WNameEn;

        [JsonIgnore]
        public long PriceSuccessfulBid { get; set; }

        [JsonProperty("price_successful_bid")]
        public string DisplayPriceSuccessfulBid => PriceSuccessfulBid.ToString("#,##0");

        [JsonIgnore]
        public decimal BuyCommSum { get; set; }

        [JsonProperty("buy_comm_sum")]
        public string DisplayBuyCommSum => BuyCommSum.ToString("#,##0");

        [JsonProperty("work_sum")]
        public string WorkSum => ((decimal) PriceSuccessfulBid + BuyCommSum).ToString("#,##0");

        [JsonIgnore]
        public string DeliveryFeeYN { get; set; }

        [JsonIgnore]
        public int DeliveryFee { get; set; }

        [JsonProperty("delivery_fee")]
        public string DisplayDeliveryFee { get; set; }

        [JsonIgnore]
        public string MailSendYN { get; set; }

        [JsonIgnore]
        public bool IsKor { get; set; }
        #endregion
    }
}
