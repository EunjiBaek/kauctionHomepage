using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionWorkCondition
    {
        public int seq { get; set; }

        public string ConditionText { get; set; }        
    }
}
