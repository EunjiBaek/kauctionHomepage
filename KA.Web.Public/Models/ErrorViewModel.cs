using KA.Entities.Models.Auction;

namespace KA.Web.Public.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Code { get; set; }

        public AuctionWork AuctionWork { get; set; }

        public AuctionSchedule AuctionSchedule { get; set; }
    }
}
