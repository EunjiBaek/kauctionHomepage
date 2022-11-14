using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Entities.Models.Member;
using KA.Web.Public.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KA.Web.Public.Controllers
{
    public class BaseController : Controller
    {
        #region # 변수 정의 #

        /// <summary>
        /// [공통] 사용자 접속 Agent
        /// </summary>
        protected string UserAgent => HttpContext.Request.Headers["User-Agent"].ToString();

        /// <summary>
        /// [공통] 사용자 접속 IP
        /// </summary>
        protected string Ip => HttpContext.Connection.RemoteIpAddress.ToString();

        /// <summary>
        /// [공통] 현재 언어 키
        /// </summary>
        public string Lang => !IsKor() ? "ENG" : "KOR";

        #endregion

        #region # 로그인/로그아웃 처리 및 사용자 클레임 데이터  #

        protected int Uid => int.TryParse(GetUserData(0), out int result) ? result : 0;

        protected string Email => GetUserData(1);

        protected string Pwd => GetUserData(2);

        protected string ID => GetUserData(3);

        protected string GetUserData(int index)
        {
            if (User.Claims.Any())
            {
                var userData = DESCryptoHelper.DESDecrypt(User.Claims.ElementAt(2).Value);
                var arrUserData = userData.Split('^');
                return arrUserData.Length > index ? arrUserData[index] : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        protected void SignIn(Member member, string isSaved = "F")
        {
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, SignHelper.GetPrincipal(member));

            if (isSaved.Equals("T"))
            {
                SavedLogin(member.Uid);
            }
            else
            {
                Response.Cookies.Delete("K-Auction.Keepup");
                HttpContext.Session.Remove("K-Auction.Keepup");
            }
        }

        protected new void SignOut()
        {
            Response.Cookies.Delete("K-Auction.Keepup");
            HttpContext.Session.Remove("K-Auction.Keepup");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        protected void SavedLogin(int uid)
        {
            Response.Cookies.Append("K-Auction.Keepup", uid.ToString(), new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
            HttpContext.Session.SetInt32("K-Auction.Keepup", uid);
        }

        #endregion

        #region # 페이지 리다이렉트 #

        /// <summary>
        /// [공통] 메인 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public IActionResult RedirectMain() => RedirectToAction("Index", "Home");

        /// <summary>
        /// [공통] 에러 페이지로 이동
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IActionResult RedirectError(string code = "", int uid = 0)
            => string.IsNullOrWhiteSpace(code)
            ? RedirectToAction("Error", "Home")
            : RedirectToAction("Error", "Home", code.Equals("auction_work") || code.Equals("auction_list") ? new { code, uid } : new { code });

        #endregion

        #region # 공통 관련 #

        /// <summary>
        /// [공통] 현재 언어가 국문인지 여부 체크 함수
        /// </summary>
        /// <returns></returns>
        public bool IsKor()
        {
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentLang = requestCulture == null ? "ko-KR" : requestCulture.RequestCulture.UICulture.Name;
            return currentLang.Contains("ko-KR");
        }

        /// <summary>
        /// [공통] 현재 로그인 상태 여부 체크 함수
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            return User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// [공통] 접속 기기가 모바일인지 체크하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsMobile()
        {
            // [#291] 케이옥션 앱 아이패드 접속 시 개선
            string u = HttpContext.Request.Headers["User-Agent"].ToString();
            if (u.ToLower().Contains("iosapp kauction"))
            {
                return true;
            }
            else
            {
                Regex b = new(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                return b.IsMatch(u) || v.IsMatch(u.Substring(0, 4));
            }
        }

        /// <summary>
        /// [공통] 절대 경로 리턴 함수
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string AbsoluteUrl(string path)
        {
            return new UriBuilder(Request.GetDisplayUrl())
            {
                Path = path
            }.ToString();
        }

        public static double GetDouble(string str)
        {
            return double.TryParse(str, out double result) ? result : 0;
        }

        public double GetRound(object value, int number)
        {
            if (double.TryParse(value.ToString(), out double outValue))
            {
                return Math.Round(outValue, number);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// [공통] 페이지 타이틀 처리 함수
        /// </summary>
        /// <param name="rootPage"></param>
        /// <param name="subPage"></param>
        /// <returns></returns>
        public string GetPageTitle(string rootPage, string subPage, string code = "")
        {
            switch (rootPage.ToLower())
            {
                #region # 공통 #
                case "common":
                    switch (subPage.ToLower())
                    {
                        case "login":
                            return IsKor() ? "케이옥션-로그인" : "KA-Login";
                        case "error":
                            return IsKor() ? "케이옥션-오류" : "KA-Error";
                        case "search":
                            return IsKor() ? $"케이옥션-검색:{code}" : $"KA-Search:{code}";
                    }
                    break;
                #endregion
                #region # 경매 #
                case "auction":
                    switch (subPage.ToLower())
                    {
                        case "list":
                            return IsKor() ? $"{code} 목록 :: 케이옥션" : $"{code} List :: KA";
                        case "listresult":
                            return IsKor() ? $"{code} 결과 목록 :: 케이옥션" : $"{code} ResultList :: KA";
                        case "detail":
                            return IsKor() ? $"{code} :: 케이옥션" : $"{code} :: KA";
                        case "detailresult":
                            return IsKor() ? $"{code} 결과 :: 케이옥션" : $"{code} Result :: KA";
                        case "result":
                            return IsKor() ? $"케이옥션 {code} 결과 목록" : $"KA {code} ResultList";
                        case "bidapplication":
                            return IsKor() ? "서면/전화 응찰신청 :: 케이옥션" : "Application for PhoneCall or e-Written Bidding :: KA";
                        case "liverequest":
                            return IsKor() ? "라이브응찰 신청 :: 케이옥션" : "Application for Participation in Live Bidding :: KA";
                    }
                    break;
                #endregion
                #region # 서비스 #
                case "service":
                    switch (subPage.ToLower())
                    {
                        case "corporation":
                            return IsKor() ? "케이옥션-서비스:기업컬렉션" : "KA-Services:Corporate Collections";
                        case "kstring":
                            return IsKor() ? "케이옥션-서비스:케이옥션스트링" : "KA-Services:K-Auction & String";
                        case "kartspace":
                            return IsKor() ? "케이옥션-서비스:케이아트스페이스" : "KA-Services:K-ArtSpace";
                        case "privatesale":
                            return IsKor() ? "케이옥션-서비스:프라이빗세일" : "KA-Services:Private Sales";
                    }
                    break;
                #endregion
                #region # 회사정보 #
                case "about":
                    switch (subPage.ToLower())
                    {
                        case "company":
                            return IsKor() ? "케이옥션-정보:회사소개" : "KA-Info:Company";
                        case "recruit":
                            return IsKor() ? "케이옥션-정보:채용공고" : "KA-Info:Recruit";
                        case "press":
                            return IsKor() ? "케이옥션-정보:언론보도" : "KA-Info:Press";
                        case "notice":
                            return IsKor() ? "케이옥션-정보:공지사항" : "KA-Info:Notice";
                        case "map":
                            return IsKor() ? "케이옥션-정보:오시는길" : "KA-Info:Map";
                    }
                    break;
                #endregion
                #region # HOW TO / 약관 및 정책 #
                case "howto":
                    switch (subPage.ToLower())
                    {
                        case "auctionintroduction":
                            return IsKor() ? "케이옥션-안내:경매정보" : "KA-HowTo:Auction Introduction";
                        case "bidguide":
                            return IsKor() ? "케이옥션-안내:응찰방법" : "KA-HowTo:Bidding Guide";
                        case "consignguide":
                            return IsKor() ? "케이옥션-안내:위탁방법" : "KA-HowTo:Consignment Guide";
                        case "major":
                            return IsKor() ? "케이옥션-규정:경매 약관" : "KA-T&P:Terms of Auction";
                        case "online":
                            return IsKor() ? "케이옥션-규정:온라인 경매 약관" : "KA-T&P:Terms of OnlineAuction";
                        case "privacy":
                            return IsKor() ? "케이옥션-규정:개인정보처리방침" : "KA-T&P:privacy policy";
                        case "clause":
                            switch (code.ToLower())
                            {
                                case "join001":
                                    return IsKor() ? "케이옥션-규정:온라인 회원약관" : "KA-T&P:Website Terms of Use";
                                case "infomng":
                                    return IsKor() ? "케이옥션-규정:내부정보관리규정" : "KA-T&P:Internal Information Management Regulations";
                            }
                            break;
                    }
                    break;
                #endregion
                #region # MY PAGE #
                case "mypage":
                    switch (subPage.ToLower())
                    {
                        case "member":
                            return IsKor() ? "내 회원정보 :: 케이옥션" : "My Information :: KA";
                        case "info":
                            return IsKor() ? "내 선호정보 작품 :: 케이옥션" : "My Favorite ArtWorks :: KA";
                        case "artist":
                            return IsKor() ? "내 선호정보 작가 :: 케이옥션" : "My Favorite Artists :: KA";
                        case "account":
                            return IsKor() ? "내 계정설정 :: 케이옥션" : "My Account :: KA";
                        case "consignrequest":
                            return IsKor() ? "위탁문의 신청 :: 케이옥션" : "Application for Consignment Inqurires :: 케이옥션";
                    }
                    break;
                #endregion
            }
            return IsKor() ? "케이옥션" : "K-Auction";
        }

        #endregion

        #region # 경매 관련 #

        /// <summary>
        /// [Online Auction] URL에 포함된 경매 값에 따른 코드 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetAuctionKind(string value) => (value.ToUpper()) switch
        {
            "MAJOR" => "1",
            "PREMIUM" => "2",
            "WEEKLY" => "4",
            _ => "0",
        };

        /// <summary>
        /// [경매] 코드를 통해 AucKind 값 리턴
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetAucKindNameFromCode(string code)
        {
            return code switch
            {
                "1" => "Major",
                "2" => "Premium",
                "4" => "Weekly",
                _ => "",
            };
        }

        /// <summary>
        /// [Online Auction] 목록 작품 타입 List 클래스 정보 리턴
        /// </summary>
        /// <param name="workType"></param>
        /// <returns></returns>
        public List<AuctionWorkType> GetAuctionWorkTypeFromString(string workType)
        {
            var list = new List<AuctionWorkType>();
            if (workType != null)
            {
                foreach (var item in workType.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] arrItem = item.Split(new string[] { "|" }, StringSplitOptions.None);
                    list.Add(new AuctionWorkType
                    {
                        Uid = int.Parse(arrItem[0]),
                        UidOrg = int.Parse(arrItem[1]),
                        Name = arrItem[2],
                        NameEn = arrItem[3],
                        DisplayName = !IsKor() && !string.IsNullOrWhiteSpace(arrItem[3]) ? arrItem[3] : arrItem[2]
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// [Auction 공통] 작품 이미지 Url 정보 리턴
        /// </summary>
        /// <param name="auction"></param>
        /// <param name="aucNum"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetImageUrl(string aucKind, int aucNum, string fileName)
        {
            return aucKind.ToUpper() switch
            {
                "MAJOR" or "1" => $"{Config.ImageDomain}/www/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                "PREMIUM" or "2" => $"{Config.ImageDomain}/www/KMall/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                "WEEKLY" or "4" => $"{Config.ImageDomain}/www/Konline/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                _ => $"{Config.ImageDomain}/www/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
            };
        }

        /// <summary>
        /// [Auction 공통] 작품 리스트 이미지 Url 정보 리턴 (이미지 코드 01인 경우)
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetListImageUrl(string aucKind, int aucNum, string fileName)
        {
            fileName = fileName.Replace(".jpg", "_L.jpg");
            return aucKind.ToUpper() switch
            {
                "MAJOR" or "1" => $"{Config.ImageDomain}/www/Work/{aucNum.ToString().PadLeft(4, '0')}/T/{fileName}",
                "PREMIUM" or "2" => $"{Config.ImageDomain}/www/KMall/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                "WEEKLY" or "4" => $"{Config.ImageDomain}/www/Konline/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                _ => $"{Config.ImageDomain}/www/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
            };
        }

        /// <summary>
        /// [Auction 공통] 작품 썸네일 이미지 Url 정보 리턴 (이미지 코드 01인 경우)
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetThumbImageUrl(string aucKind, int aucNum, string fileName)
        {
            return aucKind.ToUpper() switch
            {
                "MAJOR" or "1" => $"{Config.ImageDomain}/www/Work/{aucNum.ToString().PadLeft(4, '0')}/T/{fileName}",
                "PREMIUM" or "2" => $"{Config.ImageDomain}/www/KMall/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                "WEEKLY" or "4" => $"{Config.ImageDomain}/www/Konline/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
                _ => $"{Config.ImageDomain}/www/Work/{aucNum.ToString().PadLeft(4, '0')}/{fileName}",
            };
        }

        /// <summary>
        /// 보증서 번호 리턴
        /// </summary>
        /// <param name="work"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public string GetWarrantyNo(AuctionWork work, bool after)
        {
            var value = string.Empty;

            var aucYear = after ? DateTime.Now.Year.ToString() : work.AucDate.ToString("yyyy");
            var aucNum = StringHelper.Right("000" + work.AucNum.ToString(), 4);

            if (work.AucKind.Equals("1"))
            {
                value = string.Format("{0}-{1}-{2}", aucYear, aucNum, work.LotNum);
            }
            else if (work.AucKind.Equals("2"))
            {
                value = string.Format("ON{0}-{1}", aucNum, work.LotNum);
            }
            else if (work.AucKind.Equals("4"))
            {
                value = string.Format("KO{0}-{1}", aucNum, work.LotNum);
            }

            return value;
        }

        public string GetWorkSize(AuctionWorkKoffice work, string type)
        {
            if (type.Equals("cm"))
            {
                return GetWorkSize(work.SizePre, work.Length, work.Width, work.Height, work.SizeHd, work.SizeExtra, work.SizeNum, work.SizeSuf, work.Ho.ToString(), work.Edition, "cm");
            }
            else if (type.Equals("inch"))
            {
                return GetWorkSize(work.SizePre, work.Length, work.Width, work.Height, work.SizeHd, work.SizeExtra, work.SizeNum, work.SizeSuf, "0", work.Edition, "inch");
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetWorkSize(string pre, string len, string w, string h, string hd, string extra, string num, string suf, string ho, string ed, string type)
        {
            string tmp_h = h;

            if (type.Equals("inch"))
            {
                if (!string.IsNullOrWhiteSpace(tmp_h) && GetDouble(tmp_h.Replace("(h)", "").Replace("(d)", "")) < 1)
                {
                    return string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(len))
                {
                    if (len.Contains("(h)"))
                    {
                        len = GetRound(GetDouble(len.Replace("(h)", "")) / 2.54, 1).ToString();
                        len += "(h)";
                    }
                    else if (len.Contains("(d)"))
                    {
                        len = GetRound(GetDouble(len.Replace("(d)", "")) / 2.54, 1).ToString();
                        len += "(d)";
                    }
                    else
                    {
                        len = GetRound(GetDouble(len) / 2.54, 1).ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(w))
                {
                    if (w.Contains("(h)"))
                    {
                        w = GetRound(GetDouble(w.Replace("(h)", "")) / 2.54, 1).ToString();
                        w += "(h)";
                    }
                    else if (w.Contains("(d)"))
                    {
                        w = GetRound(GetDouble(w.Replace("(d)", "")) / 2.54, 1).ToString();
                        w += "(d)";
                    }
                    else
                    {
                        w = GetRound(GetDouble(w) / 2.54, 1).ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(h))
                {
                    if (h.Contains("(h)"))
                    {
                        h = GetRound(GetDouble(h.Replace("(h)", "")) / 2.54, 1).ToString();
                        h += "(h)";
                    }
                    else if (h.Contains("(d)"))
                    {
                        h = GetRound(GetDouble(h.Replace("(d)", "")) / 2.54, 1).ToString();
                        h += "(d)";
                    }
                    else
                    {
                        h = GetRound(GetDouble(h) / 2.54, 1).ToString();
                    }
                }
            }

            string result = string.IsNullOrWhiteSpace(pre) ? len : pre + " " + len;

            if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(w))
            {
                result += "×" + w;
            }
            else if (string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(w))
            {
                result = w;
            }

            if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(h))
            {
                result += "×" + h;
            }
            else if (string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(h))
            {
                result = h;
            }

            result = string.IsNullOrWhiteSpace(hd) ? result : result + string.Format("({0})", hd);

            result = string.IsNullOrWhiteSpace(result) ? result : result + type;

            result = !string.IsNullOrWhiteSpace(ho) && !ho.Equals("0") && type.Equals("cm") ? result += string.Format(" ({0}호)", ho) : result;

            result = !string.IsNullOrWhiteSpace(extra) ? result += " " + extra : result;

            result = !string.IsNullOrWhiteSpace(num) && !string.IsNullOrWhiteSpace(suf) ? result += ", " + num + suf : result;

            result = !string.IsNullOrWhiteSpace(ed) ? result += " (" + ed + ")" : result;

            return result;
        }

        #endregion

        #region # 회원 관련 #

        /// <summary>
        /// 회원이 담당팀 메일 주소 
        /// </summary>
        /// <param name="type">N: 팀명, 그외 이메일 주소</param>
        /// <returns></returns>
        public string[] GetMemberTeamEmail(string emailInfo, string type = "")
        {
            if (string.IsNullOrWhiteSpace(emailInfo)) return Array.Empty<string>();

            if (!string.IsNullOrWhiteSpace(emailInfo) && emailInfo.Contains("|"))
            {
                try
                {
                    var list = new List<string>();
                    foreach (var item in emailInfo.Split(';'))
                    {
                        var name = item.Split('|')[0];
                        var email = item.Split('|')[1];
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            list.Add(type.ToUpper().Equals("N") ? name : email);
                        }
                    }
                    return list.ToArray();
                }
                catch (Exception) { return Array.Empty<string>(); }
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public string GetImageSavedFolder(string type)
        {
            if (type.StartsWith("company-reg-doc"))
            {
                return "memBusinessLicenseDoc";
            }
            else if (type.StartsWith("company-business-card"))
            {
                return "memBusinessCard";
            }
            else
            {
                return "identification";
            }
        }

        public void SetMemberAttachFileName(Member member, string type, string fileName)
        {
            switch (GetImageSavedFolder(type))
            {
                case "identification":
                    member.Identification = fileName;
                    break;
                case "memBusinessLicenseDoc":
                    member.CompanyRegDoc = fileName;
                    break;
                case "memBusinessCard":
                    member.CompanyBusinessCard = fileName;
                    break;
            }
        }

        #endregion
    }
}