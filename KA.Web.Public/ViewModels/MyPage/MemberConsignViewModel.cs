using KA.Entities.Models.Member;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.MyPage
{
    public class MemberConsignViewModel
    {
        public MemberConsign MemberConsign { get; set; }

        public IEnumerable<MemberConsignImg> MemberConsignImg { get; set; }
    }
}
