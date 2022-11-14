using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionPriceBid
    {
        public int Index { get; set; }

        public decimal PriceBid { get; set; }

        public decimal PriceBidPre { get; set; }

        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }
    }
}
