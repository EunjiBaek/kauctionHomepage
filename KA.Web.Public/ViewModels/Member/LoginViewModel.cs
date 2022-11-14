namespace KA.Web.Public.ViewModels.Member
{
    public class LoginViewModel : Entities.Models.Member.Member
    {
        ///// <summary>
        ///// 로그인 상태 유지
        ///// </summary>
        //public bool IsSaved { get; set; }

        /// <summary>
        /// 유효성/결과 처리 결과 메세지
        /// </summary>
        public string Message { get; set; }
    }
}
