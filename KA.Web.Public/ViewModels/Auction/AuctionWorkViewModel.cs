using KA.Entities.Models.Auction;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Auction
{
    /// <summary>
    /// 2022.07.20 [#667/계획] ISMS 국내 회원가입 본인인증 선택으로 변경+응찰필수조건처리 - AuthFlag 변수 추가
    /// </summary>
    public class AuctionWorkViewModel
    {
        public string BreadcrumbLevel1 { get; set; }

        public string BreadcrumbLevel2 { get; set; }

        public bool IsAuthenticated { get; set; }

        public bool IsBid { get; set; }

        public string BidRemainTime { get; set; }

        public string BidStartRemainTime { get; set; }
        
        public AuctionWork AuctionWork { get; set; }

        public IEnumerable<AuctionWorkImage> AuctionWorkImages { get; set; }

        public IEnumerable<AuctionWorkAdditionalInfo> AuctionWorkAdditionalInfos { get; set; }

        public IEnumerable<AuctionWork> ArtistWorks { get; set; }

        /// <summary>
        /// Lot 목록
        /// </summary>
        public IEnumerable<AuctionWork> LotList { get; set; }

        /// <summary>
        /// 작품 컨디션 문구
        /// </summary>
        public IEnumerable<AuctionWorkCondition> AuctionWorkCondition { get; set; }


        /// <summary>
        /// 작품설명 - 참고자료
        /// </summary>
        public IEnumerable<AuctionWorkAdditionalInfo> WorkAddtionalInfo { get; set; }

        /// <summary>
        /// 작품설명 - 영상자료
        /// </summary>
        public IEnumerable<AuctionWorkAdditionalInfo> WorkAdditionalImages { get; set; }

        /// <summary>
        /// 작가설명 - 참고자료
        /// </summary>
        public IEnumerable<AuctionWorkAdditionalInfo> ArtistAdditionalInfo { get; set; }

        /// <summary>
        /// 작가 설명 - 영상자료
        /// </summary>
        public IEnumerable<AuctionWorkAdditionalInfo> ArtistAdditionalImages { get; set; }

        public IEnumerable<Image> ArtistImages { get; set; }

        // public IEnumerable<AuctionPriceBid> AuctionBidPre { get; set; }

        public string ServerTime { get; set; }

        public string DeliveryPrice { get; set; }

        public string IsRequireMobileAuth { get; set; }

        public string MobileAuthType { get; set; }

        /// <summary>
        /// 주소 등록 여부 (회원가입 개편) - 미 등록시 등록 버튼 처리
        /// </summary>
        public bool AddressFlag { get; set; }

        /// <summary>
        /// 가입 후 통신사/신용카드 인증 유무
        /// </summary>
        public bool AuthFlag { get; set; }
        
        /// <summary>
        /// 해외 유저인지 여부
        /// </summary>
        public bool IsOverSeas { get; set; }
    }
}
