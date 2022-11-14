using KA.Entities.Models.Auction;

namespace KA.Entities.Models.Member
{
    public class MemberNotiToken : AuctionWork
    {
        public string TokenVal { get; set; }

        public int KoffMemUid { get; set; }

        public string AuthVal { get; set; }

        public string UserVal { get; set; }
    }
}
