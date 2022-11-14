using KA.Entities.Models.Member;
using System.Collections.Generic;

namespace KA.Web.Admin.ViewModels.Consign
{
    public class ConsignViewModel
    {
        public MemberConsign MemberConsign { get; set; }

        public IEnumerable<MemberConsignImg> MemberConsignImg { get; set; }

        public string Query { get; set; }
    }
}
