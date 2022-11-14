using System;
using System.Text.RegularExpressions;

namespace KA.Entities.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// 통화 포맷으로 (콤마) 처리하여 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <param name="empty"></param>
        /// <returns></returns>
        public static string GetMoneyFormat(object value, bool empty = false)
        {
            if (decimal.TryParse(value.ToString(), out decimal outValue))
            {
                string result = string.Format("{0:#,###}", outValue);

                return string.IsNullOrWhiteSpace(result) ? (empty ? "" : "-") : result;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 개행 처리 태그로 변환 처리하여 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReplaceNewLineTag(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace("\n", "<br />");
        }

        /// <summary>
        /// 파라미터 값에서 태그를 제거하여 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveTag(string value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value)) return string.Empty;

            value = value.Replace("&lt;", "<").Replace("&gt;", ">");
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(value, pattern, string.Empty);
        }

        /// <summary>
        /// 임의의 문자열을 생성하여 리턴
        /// </summary>
        /// <param name="div">구분 1:숫자(num), 2:영문(alpha), 3:숫자, 영문 조합(alphanum)</param>
        /// <param name="length">글자수</param>
        /// <returns>임의의 문자열</returns>
        public static string GetRandomString(string div, int length = 8)
        {
            string randomString = string.Empty;
            string character;

            if (div.Equals("num") || div.Equals("1"))
            {
                character = "1234567890";
            }
            else if (div.Equals("alpha") || div.Equals("2"))
            {
                character = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            else if (div.Equals("alphanum") || div.Equals("3"))
            {
                character = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            else
            {
                character = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            
            Random rnd = new Random();
            for (int i = 1; i <= length; i++)
            {
                randomString += character.Substring(rnd.Next(0, character.Length), 1);
            }
            return randomString;
        }

        /// <summary>
        /// 숫자만 입력된 모바일 번호에 - 처리
        /// </summary>
        /// <param name="value">모바일</param>
        /// <returns></returns>
        public static string GetMobileHyphen(string value)
        {
            if (value.Length == 11)
            {
                value = value.Insert(3, "-");
                value = value.Insert(8, "-");
                return value;
            }
            else if (value.Length == 10)
            {
                value = value.Insert(2, "-");
                value = value.Insert(7, "-");
                return value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 해당 날짜의 한글 요일명을 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDay(DateTime value)
        {
            if (value == null) return string.Empty;

            return value.DayOfWeek switch
            {
                DayOfWeek.Monday => "월",
                DayOfWeek.Tuesday => "화",
                DayOfWeek.Wednesday => "수",
                DayOfWeek.Thursday => "목",
                DayOfWeek.Friday => "금",
                DayOfWeek.Saturday => "토",
                DayOfWeek.Sunday => "일",
                _ => "",
            };
        }

        /// <summary>
        /// 유형에 맞게 value 값을 마스킹하여 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMaskingData(string value, string type)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            if (type.Equals("E")) // 이메일 @ 이전 문장의, 맨 앞 2자리, 맨 끝 1자리 - 예> z**g080*@naver.com​
            {
                //string pattern = @"([\w-\._\+%]{1})(?<=[\w]{2})[\w-\._\+%]*(?=[\w]{1}@)";
                //return Regex.Replace(value, pattern, m => new string('*', m.Length));
                var temp = value.ToCharArray();
                var result = string.Empty;
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i].ToString() == "@") { result = result[0..^1] + "*@"; }
                    else if(i == 1 || i == 2) { result += "*"; }
                    else { result += temp[i].ToString(); }
                }
                return result;
            }
            else if (type.Equals("M")) // 앞번호 뒷3자리,​ 뒷번호 앞1자리 마스킹​ - 예> 010-9***-*930​
            {
                var temp = value.ToCharArray();
                var result = string.Empty;
                if (value.Length >= 13)
                {
                    for (int i = 0; i < temp.Length; i++) { result += (i == 5 || i == 6 || i == 7 || i == 9) ? "*" : temp[i].ToString(); }   
                }
                else if (value.Length < 13)
                {
                    for (int i = 0; i < temp.Length; i++) { result += (i == 5 || i == 6 || i == 8) ? "*" : temp[i].ToString(); }
                }
                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// 마스킹 처리하여 리턴
        /// [회원명]
        ///   -> 한글설명: 첫글자와 마지막 글자 표시
        ///   -> 2글자인 경우(가운데 공백 포함): 첫글자만 표시
        ///   -> 영문: 앞3글자 표시
        ///   -> 법인명은 노출 가능
        /// [회원별칭(아이디)]
        ///   -> 첫글자 외 * 별표시
        ///   
        /// </summary>
        /// <param name="value">마스킹할 문자열</param>
        /// <param name="type">이름(N), 이메일(E), 주소(A), 연락처(M), 아이디(I)</param>
        /// <returns></returns>
        public static string GetPrivateInfoMask(string value, string type = "N")
        {
            if (value == null || string.IsNullOrWhiteSpace(value)) return string.Empty;

            if (type.Equals("N")) // 이름
            {
                if ((new Regex(@"[a-zA-Z]")).IsMatch(value))
                {
                    return SetMask(value, 3);
                }
                else
                {
                    return value.Length < 3 ? SetMask(value, 1) : string.Concat(value.Substring(0, 1), "".PadRight(value.Length - 2, '*'), value.Substring(value.Length - 1, 1));
                }
            }
            else if (type.Equals("E")) // 이메일
            {
                if (value.IndexOf("@") > -1)
                {
                    var arr = value.Split('@');
                    return string.Concat(SetMask(arr[0], 2), "@", arr[1]);
                }
                else
                    return SetMask(value, value.Length > 2 ? 2 : 1);
            }
            else if (type.Equals("A")) // 주소
            {
                string result = string.Empty;
                try
                {
                    value = value.Trim();
                    var regex = new Regex(@"(((\[[0-9]{3}-[0-9]{3}\])|(\[[0-9]+\]))( |))?(\[수정\]|[가-힣]+(시|도)? [가-힣]+(시|군|구)( |)?([가-힣]+(시|군|구)(?!([가-힣]+)))?)");
                    if (regex.IsMatch(value))
                    {
                        foreach (Match match in regex.Matches(value))
                        {
                            result += match.Value;
                        }
                    }
                    else
                    {
                        regex = new Regex(@"(((\[[0-9]{3}-[0-9]{3}\])|(\[[0-9]+\]))( |))?([가-힣]{10})");
                        foreach (Match match in regex.Matches(value))
                        {
                            result += match.Value;
                        }
                    }
                    return result;
                }
                catch (Exception) { return string.Empty; }
            }
            else if (type.Equals("M")) // 연락처
            {
                return value.Length >= 4 ? Right(value, 4) : value;
            }
            else if (type.Equals("I")) // 아이디
            {
                return SetMask(value, 1);
            }
            return string.Empty;
        }

        /// <summary>
        /// 특정길이 이후 마스킹 처리
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static string SetMask(string str, int len)
        {
            if (string.Equals(str, "unknown(CSAM 1.0)"))
                return str;
            else if (string.IsNullOrWhiteSpace(str) || str.Length <= len)
                return str;
            else
            {
                var result = str.Substring(0, len);
                for (int i = 0; i < str.Length - len; i++)
                {
                    result = string.Concat(result, "*");
                }
                return result;
            }
        }

        /// <summary>
        /// 문자열을 받아 오른쪽부터 길이만큼 자른값을 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(string value, int length)
        {
            return string.IsNullOrWhiteSpace(value) || length < 1 || value.Length < length ? string.Empty : value.Substring(value.Length - length, length);
        }
    }
}
