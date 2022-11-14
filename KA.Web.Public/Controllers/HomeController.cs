using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Repositories;
using KA.Web.Public.Models;
using KA.Web.Public.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KA.Web.Public.Controllers
{
    public class HomeController : BaseController
    {
        #region # Constructor #

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuctionRepository auctionRepository;
        private readonly IMainRepository mainRepository;
        private readonly IMemberRepository memberRepository;
        private readonly ICommonRepository commonRepository;
        private readonly CommonService commonService;

        public HomeController(IHttpContextAccessor httpContextAccessor,
            IAuctionRepository auctionRepository,
            IMainRepository mainRepository,
            IMemberRepository memberRepository,
            ICommonRepository commonRepository,
            CommonService commonService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.auctionRepository = auctionRepository;
            this.mainRepository = mainRepository;
            this.memberRepository = memberRepository;
            this.commonRepository = commonRepository;
            this.commonService = commonService;
        }

        #endregion

        #region # WebApi #

        /// <summary>
        /// 언어 변경 처리
        /// </summary>
        /// <param name="culture">us(엉어), kr(한글)</param>
        /// <param name="returnUrl">리다이렉트 처리할 Url</param>
        /// <returns></returns>
        public IActionResult SetLanguage(string culture, string returnUrl = "")
        {
            if (!string.IsNullOrWhiteSpace(culture))
            {
                culture = culture switch
                {
                    "us" or "ENG" => "en-US",
                    _ => "ko-KR",
                };
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }

            var referer = "/";
            if (HttpContext.Request.Headers["Referer"].Count > 0)
            {
                referer = HttpContext.Request.Headers["Referer"].ToString();
                if (!(referer.ToLower().Contains("/about/recruit") || referer.ToLower().Contains("/about/press") || referer.ToLower().Contains("/about/notice")))
                {
                    referer = referer.Replace(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host, "");
                }
            }
            return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? referer : returnUrl);
        }

        /// <summary>
        /// 메인 컨테츠 조회 기록 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Home/SetContentHistory")]
        public JObject SetMainContentHistory([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResultLang("90", IsKor());

            json["mem_uid"] = LoginInfo.Uid;
            json["token"] = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"].ToString() : "";
            json["ip"] = HttpContext.Connection.RemoteIpAddress.ToString();
            json["user_agent"] = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = mainRepository.SetContentReadHst(json);
            commonService.CheckErrorLog(result);
            return JsonHelper.GetApiResultLang(result.Result, IsKor());
        }

        [HttpPost]
        [Route("/api/Home/GetMainContent")]
        public JObject GetMainContent([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResultLang("90", IsKor());

            json["mode"] = "main_content";
            json["mem_uid"] = LoginInfo.Uid;
            var list = mainRepository.GetCrawlingDatas(json);
            foreach (var item in list)
            {
                item.ImagePath = Config.ImageDomain + "/www" + item.ImagePath;
                item.Etc2 = commonService.TimeCodeFormat(item.Etc2);
            }
            return JsonHelper.GetApiResult("00", list);
        }

        #endregion

        #region # Index #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Home/GetContentData")]
        public JObject GetContentData()
        {
            return JsonHelper.GetApiResult("00", commonService.GetCrawlingDatas());
        }

        #endregion

        #region [View]

        public IActionResult Index()
        {
            // 주소 복사후 lang 이 en 인 경우 영문 처리
            var query = HttpContext.Request.Query;
            if (query["lang"].Any() && query["lang"].ToString().ToLower().Equals("en"))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US")),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
                return RedirectToAction("Index");
            }

            // 자동로그인 처리 시 K-Auction.Keepuprefresh 세션 생성하며, Controller 에서 값이 있는 경우 Refresh 처리 (인증 처리 후 새로고침)
            if (httpContextAccessor.HttpContext.Session.GetString("K-Auction.KeepupRefresh") != null)
            {
                httpContextAccessor.HttpContext.Session.Remove("K-Auction.KeepupRefresh");
                return RedirectMain();
            }

            // 응찰가능 회원이지만 생년월일 정보가 없는 경우 알림 처리 (consignbid 계정은 예외 처리)
            var member = LoginInfo.Uid > 0 ? memberRepository.GetMember(LoginInfo.Uid) : new Entities.Models.Member.Member();
            ViewBag.BCheck = member != null && member.Uid > 0
                && (member.BidAllowYN != null && member.BidAllowYN.Equals("Y"))
                && (member.BirthDate != null && string.IsNullOrWhiteSpace(member.BirthDate))
                && (member.ID != null && !member.ID.ToLower().StartsWith("consignbid"))
                ? "Y" : "N";

            return View();
        }

        #endregion

        #endregion

        #region # Error #

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string code = "", string uid = "")
        {
            if (code.Equals("auction_work") && int.TryParse(uid, out int result))
            {
                var auctionWork = auctionRepository.GetAuctionWork(result);
                if (auctionWork != null && DateTime.Now < auctionWork.AucStartDate)
                {
                    auctionWork.IsKor = IsKor();
                    auctionWork.AucKindShortName = MessageHelper.GetShortTitleFromAucKind(auctionWork.AucKind, !IsKor() ? "ENG" : "KOR");
                    return View(new ErrorViewModel { Code = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, AuctionWork = auctionWork });
                }
                else
                {
                    code = "wrong";
                }
            }
            if (code.Equals("auction_list") && int.TryParse(uid, out int aucUid))
            {
                var auctionSchedule = auctionRepository.GetAuctionSchedule(aucUid);
                if (auctionSchedule != null && DateTime.Now < auctionSchedule.AucStartDate)
                {
                    auctionSchedule.IsKor = IsKor();
                    auctionSchedule.AucKindShortName = MessageHelper.GetShortTitleFromAucKind(auctionSchedule.AucKind, !IsKor() ? "ENG" : "KOR");
                    return View(new ErrorViewModel { Code = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, AuctionSchedule = auctionSchedule });
                }
                else
                {
                    code = "wrong";
                }
            }
            ViewData["Title"] = GetPageTitle("common", "error");
            return View(new ErrorViewModel { Code = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, AuctionWork = new AuctionWork() }); ;
        }

        #endregion

        #region # Search #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Home/GetSearchHistories")]
        public JObject GetSearchHistories()
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"] != null ? httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"].ToString() : "";
            if (!string.IsNullOrWhiteSpace(token))
            {
                // 쿠키값 체크하여 처리
                if (httpContextAccessor.HttpContext.Request.Cookies["K-Auction.UserSearch"] == null || string.IsNullOrWhiteSpace(httpContextAccessor.HttpContext.Request.Cookies["K-Auction.UserSearch"].ToString()))
                {
                    var list = auctionRepository.GetAuctionWorkSearchHistories(new JObject() { ["mode"] = "main", ["token"] = token, ["mem_uid"] = LoginInfo.Uid });

                    // 사용자 검색 이력 쿠키 처리
                    httpContextAccessor.HttpContext.Response.Cookies.Append("K-Auction.UserSearch", JsonConvert.SerializeObject(list));

                    return JsonHelper.GetApiResult("00", list);
                }
                else
                {
                    var list = httpContextAccessor.HttpContext.Request.Cookies["K-Auction.UserSearch"].ToString();
                    return JsonHelper.GetApiResult("00", JsonConvert.DeserializeObject<IEnumerable<AuctionWorkSearchHistory>>(list));
                }
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        [HttpPost]
        [Route("/api/Home/DelSearchHistory")]
        public JObject DelSearchHistory([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json["mode"] = "DELETE";
            json["mem_uid"] = LoginInfo.Uid;
            json["token"] = httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"] != null ? httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"].ToString() : "";
            var result = auctionRepository.SetAuctionWorkSearchHst(json);

            if (result.Result.Equals("00"))
            {
                httpContextAccessor.HttpContext.Response.Cookies.Delete("K-Auction.UserSearch");
            }

            return JsonHelper.GetApiResult(result.Result);
        }

        [HttpPost]
        [Route("/api/Home/DelAllSearchHistory")]
        public JObject DelAllSearchHistory()
        {
            JObject json = new()
            {
                ["mode"] = "DELETE_ALL",
                ["mem_uid"] = LoginInfo.Uid,
                ["token"] = httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"] != null ? httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"].ToString() : ""
            };
            var result = auctionRepository.SetAuctionWorkSearchHst(json);

            if (result.Result.Equals("00"))
            {
                httpContextAccessor.HttpContext.Response.Cookies.Delete("K-Auction.UserSearch");
            }

            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        [HttpGet]
        [Route("/Home/Search2")]
        public IActionResult Search(string key = "")
        {
            ViewBag.Key = commonService.UrlDecode(key);
            ViewData["Title"] = GetPageTitle("common", "search", key);
            return View();
        }

        /// <summary>
        /// 통합검색 퍼블 작업용 임시 페이지
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Home/Search")]
        public IActionResult Search2(string key = "")
        {
            // 검색 이력 초기화
            httpContextAccessor.HttpContext.Response.Cookies.Delete("K-Auction.UserSearch");

            ViewBag.Key = commonService.UrlDecode(key);
            ViewData["Title"] = GetPageTitle("common", "search", key);
            return View();
        }

        #endregion

        #endregion

        #region # Info #

        public IActionResult Info()
        {
            return View();
        }

        #endregion

        #region # Test #

        public IActionResult Test()
        {
            ViewBag.Keepup = httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Keepup"] == null ? "N/A" : httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Keepup"].ToString();
            ViewBag.Token = httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"] == null ? "N/A" : httpContextAccessor.HttpContext.Request.Cookies["K-Auction.Token"].ToString();
            return View();
        }

        #endregion

        #region # Consign Read Check From Consign Mail Link #

        [HttpGet]
        [Route("/Consign/{code}")]
        public string ConsignReadCheck(string code = "")
        {
            var value = DESCryptoHelper.DESDecrypt(code);
            if (string.IsNullOrWhiteSpace(value))
            {
                return "수신확인 처리를 하지 못하였습니다.";
            }
            else
            {
                memberRepository.SetMemberConsign(new JObject()
                {
                    ["mode"] = "update_state",
                    ["state"] = "002",
                    ["code"] = value,
                    ["admin_uid"] = "999"
                });
                return "수신확인 처리하였습니다.";
            }
        }

        #endregion

        #region # live direct url #

        [HttpGet]
        [Route("/rl/{uid}")]
        public IActionResult RedirectLiveAuction(string uid)
        {
            var codeList = commonRepository.GetCodeList("REDIRECT_LIVE");
            if (codeList.Any() && codeList.Where(x => x.SubCode.Equals("001")).Any())
            {
                var code = codeList.Where(x => x.SubCode.Equals("001")).First();
                var codeName = code.CodeName;
                var targetID = codeName.Split('|');

                if (targetID.Contains(uid.ToLower()) && int.TryParse(uid, out int result))
                {
                    var member = memberRepository.GetMember(result);
                    if (member.Uid > 0)
                    {
                        SignIn(member, member.IsSaved ?? "F");

                        var currentAuction = new CurrentAuctionSchedule(commonService.GetCurrentAuctionSchedule(), "KOR");
                        return currentAuction.IsActive[0] ? Redirect($"/live/major/{currentAuction.AucNum[0]}") : RedirectError();
                    }
                }
            }
            return RedirectError();
        }

        #endregion
    }
}
