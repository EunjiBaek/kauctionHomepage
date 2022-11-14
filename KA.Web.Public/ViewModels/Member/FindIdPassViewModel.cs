namespace KA.Web.Public.ViewModels.Member
{
    public class FindIdPassViewModel : Entities.Models.Member.Member
    {
        /// <summary>
        /// 아이디 찾기/비밀번호 찾기 구분 값 (id/pass)
        /// </summary>
        public string FindMode { get; set; }

        /// <summary>
        /// 모달 처리 메세지
        /// </summary>
        public string FindMessage { get; set; }

        /// <summary>
        /// 아이디 찾기의 이름
        /// </summary>
        public string NameByID { get; set; }

        /// <summary>
        /// 비밀번호 찾기의 이름
        /// </summary>
        public string NameByPass { get; set; }

        /// <summary>
        /// 아이디 찾기 유효성 처리 결과 메세지
        /// </summary>
        public string MsgByID { get; set; }

        /// <summary>
        /// 비밀번호 찾기 유효성 처리 결과 메세지
        /// </summary>
        public string MsgByPass { get; set; }
    }
}
