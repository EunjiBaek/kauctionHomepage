using KA.Entities.Models.Auction;
using System.Collections.Generic;

namespace KA.Web.Admin.ViewModels.Auction
{
    public class AuctionScheduleViewModel
    {
        public AuctionSchedule AuctionSchedule { get; set; }

        public IEnumerable<AuctionWorkType> AuctionWorkTypes { get; set; }

        public IEnumerable<AuctionWork> AuctionWorks { get; set; }
    }
}
