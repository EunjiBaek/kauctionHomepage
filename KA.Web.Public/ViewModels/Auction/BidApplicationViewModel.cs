
using KA.Entities.Models.Auction;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Auction
{
    /// <summary>
    /// 2022.05.13 :: AddressFlag 변수 추가 - 주소 정보 등록여부
    /// </summary>
    public class BidApplicationViewModel
    {
        public bool ExistBidPreType0;

        public bool ExistBidPreType2;

        public decimal PriceBidPreType0;

        public bool MemberBidAllowYn { get; set; }

        public AuctionSchedule AuctionSchedule { get; set; }

        public AuctionWork AuctionWork { get; set; }

        public IEnumerable<AuctionWorkImage> AuctionWorkImages { get; set; }

        // 2022.05.13 :: 주소 등록 여부 True/False
        public bool AddressFlag { get; set; }
    }
}
