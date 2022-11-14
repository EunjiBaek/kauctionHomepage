using KA.Entities.Models.Auction;
using KA.Entities.Models.Content;
using KA.Entities.Models.Common;
using KA.Entities.Models.Main;
using KA.Repositories;
using KA.Web.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Text;
using KA.Entities.Models.Member;

namespace KA.Web.Public.Services
{
    public class CommonService
    {
        #region # Constructor #

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonRepository _commonRepository;
        private readonly IMainRepository _mainRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IConfiguration _configuration;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IMemberRepository _memberRepository;

        public CommonService(IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ICommonRepository commonRepository,
            IMainRepository mainRepository,
            IContentRepository contentRepository,
            IAuctionRepository auctionRepository,
            IMemberRepository memberRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _commonRepository = commonRepository;
            _mainRepository = mainRepository;
            _contentRepository = contentRepository;
            _configuration = configuration;
            _auctionRepository = auctionRepository;
            _memberRepository = memberRepository;
        }

        #endregion

        #region # 공통 #

        /// <summary>
        /// [공통] 현재 요청 페이지 호스트 정보 리턴
        /// </summary>
        public string GetHost => _httpContextAccessor.HttpContext.Request.Host.ToString();

        /// <summary>
        /// [공통] 현재 요청 페이지 정보 리턴
        /// </summary>
        /// <returns></returns>
        public string GetPage => _httpContextAccessor.HttpContext.Request.Path.Value.ToString();

        /// <summary>
        /// [공통] 파라미터의 값을 HTML 디코딩 처리하는 함수
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string HTMLDecode(string value) => HttpUtility.HtmlDecode(value);

        /// <summary>
        /// [공통] 파라미터의 값을 URL 디코딩 처리하는 함수
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string UrlDecode(string value) => HttpUtility.UrlDecode(value);

        /// <summary>
        /// [공통] 공통 코드 목록을 가져오는 함수
        /// </summary>
        /// <param name="mainCode"></param>
        /// <returns></returns>
        public IEnumerable<CommonCode> GetCodeList(string mainCode, bool isKor = true)
        {
            var codeList = _commonRepository.GetCodeList(mainCode, "", isKor ? "K" : "E");
            foreach (var item in codeList)
            {
                item.DisplayCodeName = isKor ? item.CodeName : item.CodeName2;
            }
            return codeList;
        }

        public IEnumerable<CommonCode> GetMainCodeList(string mainCodeList)
        {
            var codeList = _commonRepository.GetMainCodeList(mainCodeList);
            foreach (var item in codeList)
            {
                item.DisplayCodeName = IsKor() ? item.CodeName : item.CodeName2;
            }
            return codeList;
        }

        /// <summary>
        /// [공통] 메뉴 정보 가져오는 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Menu> GetMenus(string type = "")
        {
            return _commonRepository.GetMenus(type, IsKor() ? "K" : "E");
        }

        /// <summary>
        /// [공통] 언어설정이 한국어 인지 체크하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsKor()
        {            
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            var currentLang = requestCulture == null ? "ko-KR" : requestCulture.RequestCulture.UICulture.Name;
            return currentLang.Contains("ko-KR");
        }

        /// <summary>
        /// [공통] 인증 여부체크 함수
        /// </summary>
        public bool IsLogin => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

        /// <summary>
        /// [공통] 접속 기기가 모바일인지 체크하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsMobile()
        {
            try
            {
                // [#291] 케이옥션 앱 아이패드 접속 시 개선
                string u = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
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
            catch (Exception) { return false; }
        }

        /// <summary>
        /// [공통] 접속 기기가 IOS 기기인지 체크하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsIOS()
        {
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            return userAgent.Contains("Mac OS");
        }

        /// <summary>
        /// [공통] 접속 기기가 Macintosh 기기인지 체크하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsMacintosh()
        {
            var userAgent = _httpContextAccessor?.HttpContext?.Request.Headers["User-Agent"].ToString();
            return userAgent.Contains("Mac OS") && userAgent.Contains("Macintosh");
        }

        /// <summary>
        /// [공통] 앱으로 접속했는지 체크하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsApp()
        {
            try
            {
                string u = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
                return u.ToLower().Contains("kauction");
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// [공통] 접속 브라우저가 IE인지 체크
        /// </summary>
        /// <returns></returns>
        public bool IsIE()
        {
            try
            {
                return _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString().Contains("Trident/");
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// DB 에러 로그 처리
        /// </summary>
        /// <param name="dbResult"></param>
        public void CheckErrorLog(DbResult dbResult)
        {
            if (!string.IsNullOrWhiteSpace(dbResult.Result) && !dbResult.Result.Equals("00"))
            {
                _commonRepository.SetErrorLog(dbResult);
            }
        }

        /// <summary>
        /// 초를 입력 받아 시:분:초 포맷으로 리턴
        /// </summary>
        /// <param name="timeCodeValue"></param>
        /// <returns></returns>
        public string TimeCodeFormat(object value)
        {

            //var result = Convert.ToDouble(value);
            //if (result > 0)
            //{
            //    var hour = Convert.ToInt32(result / 3600);
            //    var minute = Convert.ToInt32(result % 3600 / 60);
            //    var second = Convert.ToInt32(result % 3600 % 60);
            //    return $"{(hour > 0 ? hour.ToString().PadLeft(2, '0') + ":" : "")}{(minute > 0 ? minute.ToString().PadLeft(2, '0') + ":" : "")}{second.ToString().PadLeft(2, '0')}";
            //}
            //else
            //{
            //    return string.Empty;
            //}
            if (value == null) return string.Empty;

            if (double.TryParse(value.ToString(), out double result))
            {
                var hour = Convert.ToInt32(result / 3600);
                var minute = Convert.ToInt32(result % 3600 / 60);
                var second = Convert.ToInt32(result % 3600 % 60);
                return $"{(hour > 0 ? hour.ToString().PadLeft(2, '0') + ":" : "")}{(minute > 0 ? minute.ToString().PadLeft(2, '0') + ":" : "")}{second.ToString().PadLeft(2, '0')}";
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region # 메인 #

        /// <summary>
        /// [메인] 배너 정보 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Banner> GetMainBannerList()
        {
            var list = _mainRepository.GetMainBannerList("", 1, IsKor() ? "K" : "E");
            foreach (var item in list)
            {
                item.IsMobile = IsMobile();
            }
            return list;
        }

        /// <summary>
        /// (Spec-Out) [메인] 공지사항 정보 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BoardDoc> GetMainBoardList()
        {
            return _contentRepository.GetBoardDocs("main", "Notice", "1", "Y");
        }

        /// <summary>
        /// [메인] 상단 메뉴 현재 경매 정보 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuctionSchedule> GetCurrentAuctionSchedule()
        {
            var list = _mainRepository.GetCurrentAuctions();
            foreach (var item in list)
            {
                item.IsKor = IsKor();
            }
            return list;
        }

        /// <summary>
        /// [메인] 작품 하이라이트 정보 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuctionWork> GetWorkHighlight()
        {
            var list = _mainRepository.GetWorkHighlight("main", LoginInfo.Uid);
            foreach (var item in list)
            {
                item.IsKor = IsKor();
                //item.AucKindName = GetAucKindName(item.AucKind);
                item.ThumFileName = GetImagePath(item.AucKind, item.AucNum, item.ThumFileName);
            }
            return list;
        }

        /// <summary>
        /// [메인] 크롤링 데이터 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CrawlingData> GetCrawlingDatas()
        {
            var list = _mainRepository.GetCrawlingDatas(new JObject() { ["mode"] = "main", ["mem_uid"] = LoginInfo.Uid });
            foreach (var item in list)
            {
                item.ImagePath = Config.ImageDomain + "/www" + item.ImagePath;
            }
            return list;
        }

        /// <summary>
        /// [메인] 상단 공지사항 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notice> GetNotices(string page, string mode = "main")
        {
            return _mainRepository.GetNotices(mode, IsKor() ? "K" : "E", "", "", page);
        }

        /// <summary>
        /// [메인] 경매 일정 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Schedule> GetAuctionSchedules()
        {
            var list = _mainRepository.GetAuctionSchedules("main");
            foreach (var item in list)
            {
                item.IsKor = IsKor();
            }
            return list;
        }

        /// <summary>
        /// [메인] 추천 검색어 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SearchTerm> GetSearchTerms()
        {
            return _mainRepository.GetSearchTerms(new JObject() { ["mode"] = "main" });
        }

        /// <summary>
        /// [메인] 최근 검색어 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuctionWorkSearchHistory> GetAuctionWorkSearchHistories()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"] != null ? _httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"].ToString() : "";
            return _auctionRepository.GetAuctionWorkSearchHistories(new JObject() { ["mode"] = "main", ["token"] = token, ["mem_uid"] = LoginInfo.Uid });
        }

        #endregion

        #region # 경매 #

        /// <summary>
        /// [경매] 경매 타이틀 정보 처리 함수
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetAucKind(string value) => value switch
        {
            "1" => "메이저 경매",
            "2" => "프리미엄 온라인",
            "4" => "위클리 온라인",
            _ => string.Empty,
        };

        /// <summary>
        /// [경매] 경매 별 작품 이미지 경로 처리 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetImagePath(string aucKind, int aucNum, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;

            var value = _configuration.GetSection("Configs").GetSection("ImageDomain").Value;
            if (aucKind.Equals("2"))
            {
                value += "/www/KMall/Work/" + aucNum.ToString().PadLeft(4, '0') + "/" + fileName.Replace(".jpg", "_L.jpg");
            }
            else if (aucKind.Equals("4"))
            {
                value += "/www/Konline/Work/" + aucNum.ToString().PadLeft(4, '0') + "/" + fileName.Replace(".jpg", "_L.jpg");
            }
            else
            {
                value += "/www/Work/" + aucNum.ToString().PadLeft(4, '0') + "/T/" + fileName.Replace(".jpg", "_L.jpg");
            }
            return value;
        }

        /// <summary>
        /// [경매] 경매 코드 별 경매 영문명 처리 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <returns></returns>
        public string GetAucKindName(string aucKind)
        {
            if (aucKind.Equals("2")) return "Premium";
            else if (aucKind.Equals("4")) return "Weekly";
            else return "Major";
        }

        /// <summary>
        /// [경매] 응찰 가격 목록 처리 함수
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IEnumerable<AuctionPriceBid> GetAuctionBidPre(int uid, string mode = "")
        {
            return _auctionRepository.GetAuctionBidPre(uid, mode);
        }

        /// <summary>
        /// [경매] 가격을 국문 단위로 표기 (억/만)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetMoneyUnitFormat(decimal value)
        {
            string[] unitSymbol = new string[] { "", "만", "억" };

            if (value == 0) { return "0"; }

            int unitID = 0;

            string number = string.Format("{0:# #### #### #### #### ####}", value).TrimStart();
            string[] splits = number.Split(' ');

            StringBuilder sb = new();

            for (int i = splits.Length; i > 0; i--)
            {
                if (int.TryParse(splits[i - 1], out int digits))
                {
                    if (digits != 0)
                    {
                        sb.Insert(0, $"{ digits}{ unitSymbol[unitID] }");
                    }
                }
                else
                {
                    sb.Insert(0, $"{ splits[i - 1] }");
                }
                unitID++;
            }
            return sb.ToString();
        }

        #endregion

        #region # 회원 #

        /// <summary>
        /// 현재 설정된 약관 버전 정보 리턴
        /// </summary>
        /// <returns></returns>
        public int GetMemClauseVersion()
        {
            var codeVersion = 4;
            var codeList = _commonRepository.GetCodeList("MEM_CLAUSE_VERSION", "", IsKor() ? "K" : "E");
            if (codeList != null && codeList.Any())
            {
                codeVersion = int.TryParse(codeList.ToList()[0].CodeName, out int result) ? result : 4;
            }
            return codeVersion;
        }

        /// <summary>
        /// 대표 주소 정보 리턴
        /// </summary>
        /// <returns></returns>
        public MemberAddress GetMemPrimaryAddress()
        {
            var addressList = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid, 0, "Y");
            return addressList.Any() ? addressList.First() : new MemberAddress();
        }

        #endregion
    }
}
