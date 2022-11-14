using KA.Entities.Models.Common;
using KA.Entities.Models.Member;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.MyPage
{
    public class MemberViewModel
    {
        public Entities.Models.Member.Member Member { get; set; }

        public IEnumerable<CommonCode> MemCountryCodeList { get; set; }

        public IEnumerable<CommonCode> MemJobCodeList { get; set; }

        public IEnumerable<MemberAddress> MemberAddresses { get; set; }

        public bool MemberAddressFlag { get; set; }

        public bool MemberAuthFlag { get; set; }

        public IEnumerable<MemberClause> MemberClauses { get; set; }
    }
}
