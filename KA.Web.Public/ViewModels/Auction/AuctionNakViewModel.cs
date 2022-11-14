using KA.Entities.Models.Auction;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Auction
{
    public class AuctionNakViewModel
    {
        [JsonProperty("auc_title")]
        public string AucTitle { get; set; }

        [JsonProperty("expire_date")]
        public string ExpireDate { get; set; }

        [JsonProperty("total_price_successful_bid")]
        public string TotalPriceSuccessfulBid { get; set; }

        [JsonProperty("total_buy_comm_sum")]
        public string TotalBuyCommSum { get; set; }

        [JsonProperty("total_delivery_fee")]
        public string TotalDeliveryFee { get; set; }

        [JsonProperty("total_price")]
        public string TotalPrice { get; set; }

        [JsonProperty("total_price_fee")]
        public string TotalPriceFee { get; set; }

        [JsonProperty("mail_send_yn")]
        public string MailSendYN { get; set; }

        public IEnumerable<AuctionNak> AuctionNak { get; set; }
    }
}
