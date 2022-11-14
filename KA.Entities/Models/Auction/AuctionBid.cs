using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionBid
    {
        [JsonProperty("bid_uid")]
        public int Uid { get; set; }

        [JsonProperty("auc_kind")]
        public int AucKind { get; set; }

        [JsonIgnore]
        public int WorkUid { get; set; }

        [JsonIgnore]
        public string WorkCode { get; set; }

        [JsonIgnore]
        public int MemUid { get; set; }

        [JsonProperty("price_bid")]
        public decimal PriceBid { get; set; }

        [JsonProperty("price_bid_format")]
        public string PriceBidFormat
        {
            get
            {
                return  PriceBid.ToString("#,##0");
            }
        }

        [JsonIgnore]
        public decimal PriceBidPre { get; set; }

        [JsonProperty("price_bid_pre_format")]
        public string PriceBidPreFormat
        {
            get
            {
                return BidType.Equals("002") ?  PriceBidPre.ToString("#,##0") : "";
            }
        }

        [JsonProperty("bid_type_format")]
        public string BidTypeFormat 
        {
            get { 
                return BidType.Equals("002") ? "자동응찰" : "일반응찰";
            }
        }        

        [JsonProperty("nak_yn")]
        public string NakYN { get; set; }

        [JsonProperty("bid_type")]
        public string BidType { get; set; }

        [JsonIgnore]
        public DateTime RegDate { get; set; }

        [JsonIgnore]
        public string MemID { get; set; }

        [JsonProperty("mem_id")]
        public string DisplayMemID { get; set; } // => string.IsNullOrWhiteSpace(MemID) ? "********" : "******" + MemID.Substring(MemID.Length - 2);

        [JsonIgnore]
        public string MemName { get; set; }

        [JsonProperty("mem_name")]
        public string DisplayMemName { get; set; }

        [JsonIgnore]
        public string MemMobile { get; set; }

        [JsonProperty("mem_mobile")]
        public string DisplayMemMobile { get; set; }

        [JsonProperty("mng_name")]
        public string MngName { get; set; }

        [JsonProperty("reg_ymd")]
        public string DisplayYMD => RegDate.ToString("yyyy-MM-dd");

        [JsonProperty("reg_hms")]
        public string DisplayHMS => RegDate.ToString("HH:mm:ss");

        [JsonProperty("my_bid")]
        public string MyBid { get; set; }

        [JsonProperty("use_yn")]
        public string UseYN { get; set; }

        [JsonProperty("last_bid_yn")]
        public string LastBidYN { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("my_count")]
        public int MyCount { get; set; }

        [JsonProperty("highest_uid")]
        public int HighestUid { get; set; }

        [JsonProperty("bid_device_type")]
        public string BidDeviceType { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }
    }
}
