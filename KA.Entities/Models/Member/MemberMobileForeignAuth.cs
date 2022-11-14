using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.05.16 :: Auth 변수 추가 - 인증 타입 (M:모바일, E:이메일)
    /// </summary>
    public class MemberMobileForeignAuth
    {
        public int Uid { get; set; }

        public string Seq { get; set; }

        public string Type { get; set; }

        public string TypeDetail { get; set; }

        public string State { get; set; }

        public string Device { get; set; }

        public string UserAgent { get; set; }

        public string Ip { get; set; }

        public string Result { get; set; }

        public int MemUid { get; set; }

        public string Auth { get; set; }

        public string MobileNo { get; set; }

        public string AuthNo { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime ModDate { get; set; }

        public string Lang { get; set; }
    }
}
