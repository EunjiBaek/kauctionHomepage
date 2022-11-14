using KA.Entities.Helpers;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionReport
    {
        public bool IsKor { get; set; }
        public string MemName { get; set; }

        public string Email { get; set; }

        public string AucTitle { get; set; }

        public string AucTitleEn { get; set; }

        public string DisplayAucTitle => !IsKor && !string.IsNullOrWhiteSpace(AucTitleEn) ? AucTitleEn : AucTitle;

        public DateTime AucDate { get; set; }

        public int LotNum { get; set; }

        public string AName { get; set; }

        public string WName { get; set; }

        public int SuccessfulPersonUid { get; set; }

        public decimal PriceSuccessfulBid { get; set; }

        public string DisplayPriceSuccessfulBid => StringHelper.GetMoneyFormat(PriceSuccessfulBid);

        public decimal BuyCommRateLow { get; set; }

        public decimal BuyCommSum { get; set; }

        public string DisplayBuyCommSum => StringHelper.GetMoneyFormat(BuyCommSum);

        public string DeliveryFeeYn { get; set; }

        public decimal DeliveryFee { get; set; }

        public string DisplayDeliveryFee => StringHelper.GetMoneyFormat(DeliveryFee).Replace("-", "0");

        public string DisplayTotalPrice => StringHelper.GetMoneyFormat(PriceSuccessfulBid + BuyCommSum);

        public DateTime EndDate => AucDate.AddDays(7);
    }
}
