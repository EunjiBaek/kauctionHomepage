using System;

namespace KA.Entities.Models.Member
{
    public class MemberPaymentInfo : MemberAddress
    {
        public new string Uid { get; set; }

        public int AddressUid { get; set; }

        public string DesiredDate1 { get; set; }

        public string DesiredDate2 { get; set; }

        public string ElevatorType { get; set; }

        public string VehicleEntry { get; set; }

        public string LadderTruck { get; set; }

        public string Memo { get; set; }

        public string Installation { get; set; }

        public DateTime CompDate { get; set; }
    }
}
