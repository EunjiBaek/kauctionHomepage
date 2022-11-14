using System;

namespace KA.Entities.Models.Member
{
    public class MemberConsignImg
    {
        public int Uid { get; set; }

        public string Code { get; set; }

        public string ImgFileName { get; set; }

        public string Target { get; set; }

        public int ImgW { get; set; }

        public int ImgH { get; set; }

        public DateTime RegDate { get; set; }
    }
}
