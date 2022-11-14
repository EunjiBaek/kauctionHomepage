using System;

namespace KA.Entities.Models.Member
{
    public class MemberDocument
    {
        public int Uid { get; set; }

        public string Token { get; set; }

        public string DocumentType { get; set; }

        public string DocumentName { get; set; }

        public string DocumentHtml { get; set; }

        public string Params { get; set; }

        public string SendType { get; set; }

        public string SendInfo { get; set; }

        public string SecurityYn { get; set; }

        public string SecurityCode { get; set; }

        public int MemUid { get; set; }

        public int MngUid { get; set; }

        public DateTime SendDate { get; set; }

        public DateTime ReadDate { get; set; }

        public int ReadCnt { get; set; }
    }
}
