using KA.Entities.Models.Auction;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Auction
{
    public class AuctionListViewModel
    {
        public string BreadcrumbLevel1 { get; set; }

        public string BreadcrumbLevel2 { get; set; }

        public string BreadcrumbLevel3 { get; set; }

        public string BreadcrumbLevel4 { get; set; }

        public string DisplayAucTitle { get; set; }

        public string DisplayAucDate { get; set; }

        public string DisplayAucStartDate { get; set; }

        public string DisplayAucEndDate { get; set; }

        public decimal PriceFrom => 0.0m; // AuctionSchedule.AucKind != null && AuctionSchedule.AucKind == "1" ? AuctionSchedule.PriceEstimatedLow : AuctionSchedule.PriceStart;

        public decimal PriceTo => AuctionSchedule.AucKind != null && AuctionSchedule.AucKind == "1" 
            ? AuctionSchedule.PriceEstimatedHigh > AuctionSchedule.PriceStart ? AuctionSchedule.PriceEstimatedHigh : AuctionSchedule.PriceStart
            : AuctionSchedule.PriceStart;

        public AuctionSchedule AuctionSchedule { get; set; }

        public IEnumerable<AuctionWorkType> AuctionWorkType { get; set; }

        public AuctionLiveRequest AuctionLiveRequest { get; set; }

        public string LiveState { get; set; }

        public IEnumerable<AuctionWork> LotList { get; set; }

        public IEnumerable<AuctionWork> ArtistList { get; set; }
    }
}
