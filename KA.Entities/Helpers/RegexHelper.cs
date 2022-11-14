using System.Text.RegularExpressions;

namespace KA.Entities.Helpers
{
    public static class RegexHelper
    {
        /// <summary>
        /// 이메일 주소 유효성 체크
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmailValid(string value) => Regex.IsMatch(value, @"^\w+@[a-zA-Z_\-]+?\.[a-zA-Z]{2,3}$");
    }
}
