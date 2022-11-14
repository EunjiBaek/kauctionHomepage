using System;
using System.Collections.Generic;
using System.Globalization;

namespace KA.Entities.Models.Auction
{
    public class CurrentAuctionSchedule
    {
        // 0: 메이저, 1: 프리미엄, 2: 위클리
        public bool[] IsActive = new bool[3];
        public string[] AucTitle = new string[3];
        public string[] AucTitleOrg = new string[3];
        public string[] AucKind = new string[3];
        public int[] AucNum = new int[3];
        public string[] AucDate = new string[3];
        public string[] StartDate = new string[3];
        public string[] EndDate = new string[3];
        public string[] AucPlace = new string[3];
        public string[] AucPeriod = new string[3];
        public string[] AucDateTime = new string[3];
        public DateTime[] dtAucPreview = new DateTime[3];
        public DateTime[] dtAucStartDate = new DateTime[3];
        public DateTime[] dtAucDate = new DateTime[3];

        public CurrentAuctionSchedule(IEnumerable<AuctionSchedule> auctionSchedules = null, string lang = "KOR")
        {
            if (auctionSchedules != null)
            {
                foreach (var item in auctionSchedules)
                {
                    if (item.AucKind.Equals("1"))
                    {
                        SetAuction(0, item, lang);
                    }
                    else if (item.AucKind.Equals("2"))
                    {
                        SetAuction(1, item, lang);
                    }
                    else if (item.AucKind.Equals("4"))
                    {
                        SetAuction(2, item, lang);
                    }
                }
            }
        }

        private void SetAuction(int index, AuctionSchedule auction, string lang)
        {
            IsActive[index] = DateTime.Now <= DateTime.Parse(auction.AucEndDate.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            AucTitle[index] = auction.DisplayAucTitle;
            AucTitleOrg[index] = auction.DisplayAucTitleOrg;
            AucKind[index] = auction.AucKind;
            AucNum[index] = auction.AucNum;
            AucDate[index] = MessageHelper.GetDisplayDate(auction.AucEndDate, lang);
            StartDate[index] = MessageHelper.GetDisplayStartDate(auction.AucStartDate, lang);
            EndDate[index] = MessageHelper.GetDisplayEndDate(auction.AucEndDate, lang);
            AucPlace[index] = auction.DisplayAucPlace;
            AucDateTime[index] = auction.AucEndDate.ToString("yyyy-MM-dd HH:mm:ss");

            dtAucPreview[index] = auction.AucPreviewDate;
            dtAucStartDate[index] = auction.AucStartDate;
            dtAucDate[index] = auction.AucDate;

            CultureInfo cultures = lang.Equals("KOR") ? CultureInfo.CreateSpecificCulture("ko-KR") : CultureInfo.CreateSpecificCulture("en-US");

            AucPeriod[index] = lang.Equals("KOR") ? auction.AucStartDate.ToString(string.Format("yyyy년 MM월 dd일 (ddd) H:mm", cultures)) + " - " + auction.AucEndDate.ToString(string.Format("yyyy년 MM월 dd일 (ddd) H:mm", cultures))
                : auction.AucStartDate.ToString("yyyy/MM/dd - hh:mm tt", new CultureInfo("en-US")) + " - " + auction.AucEndDate.ToString("yyyy/MM/dd - hh:mm tt", new CultureInfo("en-US"));
        }
    }
}
