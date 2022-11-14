using KA.Entities.Models.Member;

namespace KA.Web.Public.ViewModels.Member
{
    public class MyPageViewModel : MemberRetire
    {
        #region # Common #

        public string TabIndex { get; set; }

        public string ProcessMode { get; set; }

        public string ProcessResult { get; set; }

        #endregion

        #region # 1. 비밀번호 변경 #

        public string PasswordOld { get; set; }

        public string PasswordNew { get; set; }

        public string PasswordNewConfirm { get; set; }

        #endregion
    }
}
