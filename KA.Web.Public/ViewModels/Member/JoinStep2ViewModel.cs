using KA.Entities.Models.Common;
using KA.Entities.Models.Member;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Member
{
    public class JoinStep2ViewModel : Entities.Models.Member.Member
    {
        public string Key { get; set; }

        public string JoinType { get; set; }

        public string JoinTypeName { get; set; }

        public string Message { get; set; }

        public IEnumerable<CommonCode> MemCountryCodeList { get; set; }

        public IEnumerable<CommonCode> MemInterestCodeList { get; set; }

        public IEnumerable<CommonCode> MemJobCodeList { get; set; }

        public MemberMobileAuth MemberMobileAuth { get; set; }
    }
}
