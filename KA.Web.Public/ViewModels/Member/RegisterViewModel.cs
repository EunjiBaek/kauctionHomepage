using KA.Entities.Models.Common;
using KA.Entities.Models.Member;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Member
{
    public class RegisterViewModel : Entities.Models.Member.Member
    {
        // 회원 유형 (001: 국내개인고객)
        public string JoinType { get; set; }

        public string Mode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// KCB 인증 후 리턴 키
        /// </summary>
        public string Key { get; set; }        

        public IEnumerable<MemberClause> memberClauses;

        public IEnumerable<CommonCode> memberType;

        public string JoinToken { get; set; }
    }
}
