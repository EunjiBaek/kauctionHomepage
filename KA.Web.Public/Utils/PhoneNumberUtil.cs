namespace KA.Web.Public.Utils
{
    /// <summary>
    /// 전화번호 관련 유틸 모음 클래스입니다.
    /// </summary>
    public static class PhoneNumberUtils
    {
        /// <summary>
        /// 로컬(한국) 휴대전화 번호에 Hyphen(-) 을 설정합니다.
        /// </summary>
        /// <param name="phoneNumber">전화 번호 원본</param>
        /// <returns>하이픈 추가된 전화번호</returns>
        public static string SetHyphenToLocalNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Insert(3, "-");
            phoneNumber = phoneNumber.Insert(phoneNumber.Length is 11 ? 8 : 7, "-");

            return phoneNumber;
        }
    }
}