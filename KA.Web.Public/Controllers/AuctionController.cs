using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Entities.Models.Email;
using KA.Entities.Models.Member;
using KA.Repositories;
using KA.Web.Public.Models;
using KA.Web.Public.ViewModels.Auction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KA.Web.Public.Controllers
{
    public class AuctionController : BaseController
    {
        #region # Constructor #

        private readonly IConfiguration _configuration;
        private readonly IAuctionRepository _auctionRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMainRepository _mainRepository;
        private readonly ILogRepository _logRepository;
        private readonly EmailHelper _emailHelper;

        public AuctionController(IConfiguration configuration, 
            IAuctionRepository auctionRepository, 
            ICommonRepository commonRepository,
            IMemberRepository memberRepository,
            IMainRepository mainRepository,
            ILogRepository logRepository,
            EmailHelper emailHelper)
        {
            _configuration = configuration;
            _auctionRepository = auctionRepository;
            _commonRepository = commonRepository;
            _memberRepository = memberRepository;
            _mainRepository = mainRepository;
            _logRepository = logRepository;
            _emailHelper = emailHelper;
        }

        #endregion

        #region # Auction / Online Auction #

        #region # Common Api #

        /// <summary>
        /// 경매 작품 목록 처리
        /// </summary>
        /// <param name="json"></param>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <returns></returns>
        [Route("/api/Auction/{AucKind}/{AucNum}")]
        public JObject GetList([FromBody] JObject json, string aucKind = "", string aucNum = "", string search = "")
        {
            if (json != null)
            {
                json["token"] = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"].ToString() : "";
            }

            var list = _auctionRepository.GetAuctionWorks(aucKind, int.Parse(aucNum), IsLogin() ? LoginInfo.Uid : 0, json);
            foreach (var item in list)
            {
                item.IsLogin = IsLogin();
                item.IsKor = IsKor();
            }
            return JsonHelper.GetApiResultLang("00", IsKor(), list);
        }

        /// <summary>
        /// 경매 작품 통합 검색 목록 처리
        /// </summary>
        /// <param name="json"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("/api/Search/{key}")]
        public JObject GetSearchList([FromBody] JObject json, string key)
        {
            var list = _auctionRepository.GetAuctionWorksSearch(JsonHelper.GetString(json, "auc_kind"), LoginInfo.Uid, json, "search");
            //var count = _auctionRepository.GetAuctionWorksSearch(JsonHelper.GetString(json, "auc_kind"), LoginInfo.Uid, json, "search_count");
            //var searchResult = count.ToArray().Length > 0 ? count.ToArray()[0].SearchResult : "";
            var totalCount = list.Any() ? list.ToArray()[0].TotalCount : 0;
            var isLogin = IsLogin();
            var isKor = IsKor();
            foreach (var item in list)
            {
                item.IsLogin = isLogin;
                item.IsKor = isKor;
                item.PriceHammer = item.IsLogin ? item.PriceHammer : 0;
            }
            // return JsonHelper.GetApiResultLang("00", IsKor(), new SearchListViewModel() { SearchResult = searchResult, AuctionWorks = list, TotalCount = totalCount });
            return JsonHelper.GetApiResultLang("00", IsKor(), new SearchListViewModel() { AuctionWorks = list, TotalCount = totalCount });
        }

        /// <summary>
        /// 경매 작품 별 응찰 목록 처리
        /// </summary>
        /// <param name="workSeq"></param>
        /// <returns></returns>
        [Route("/api/Auction/BidList/{WorkUid}")]
        public JObject GetBidList(string workUid)
        {
            var uid = int.TryParse(workUid, out int result) ? result : -1;
            var list = _auctionRepository.GetAuctionBids(uid, IsLogin() ? LoginInfo.Uid : -1);
            foreach (var item in list)
            {
                item.DisplayMemID = item.MemUid.Equals(LoginInfo.Uid) ? item.MemID : (string.IsNullOrWhiteSpace(item.MemID) ? "********" : "******" + item.MemID[^2..]);
            }
            return JsonHelper.GetApiResultLang("00", IsKor(), list);
        }

        /// <summary>
        /// 경매 작품 별 응찰 목록 및 자동 응찰가 목록, 사용자 최고가 정보 처리
        /// </summary>
        /// <param name="workSeq"></param>
        /// <returns></returns>
        [Route("/api/Auction/BidTotalList/{WorkUid}")]
        public JObject GetBidTotalList(string workUid)
        {
            var uid = int.TryParse(workUid, out int result) ? result : -1;
            var list = _auctionRepository.GetAuctionBids(uid, IsLogin() ? LoginInfo.Uid : -1);
            foreach (var item in list)
            {
                item.DisplayMemID = item.MemUid.Equals(LoginInfo.Uid) ? item.MemID : (string.IsNullOrWhiteSpace(item.MemID) ? "********" : "******" + item.MemID[^2..]);
            }

            var auctionWork = _auctionRepository.GetAuctionWork(uid, User.Identity.IsAuthenticated ? LoginInfo.Uid : 0);
            var bidPrice = _auctionRepository.GetAuctionBidPre(uid);
            var userHighestPre = _auctionRepository.GetAuctionBidPre(uid, "user_highest_pre", LoginInfo.Uid);

            return JsonHelper.GetApiResult("00", new
            {
                bid_list = list,
                bid_price = bidPrice ?? new List<AuctionPriceBid>(),
                user_highest_pre = userHighestPre.Any() ? userHighestPre.First() : new AuctionPriceBid(),
                bid_remain_time = auctionWork.EndTime.ToString("yyyy/MM/dd HH:mm:ss")
            });
        }

        /// <summary>
        /// 경매 작품 별 사용자 응찰 목록 처리
        /// </summary>
        /// <param name="workSeq"></param>
        /// <returns></returns>
        [Route("/api/Auction/MyBidList/{WorkUid}")]
        public JObject GetMyBidList(string workUid)
        {
            var list = _auctionRepository.GetAuctionMyBids(int.Parse(workUid), IsLogin() ? LoginInfo.Uid : -1);
            foreach (var item in list)
            {
                item.DisplayMemID = item.MemUid.Equals(LoginInfo.Uid) ? item.MemID : (string.IsNullOrWhiteSpace(item.MemID) ? "********" : "******" + item.MemID.Substring(item.MemID.Length - 2));
            }
            return JsonHelper.GetApiResultLang("00", IsKor(), list);
        }

        /// <summary>
        /// 경매 작품 응찰 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Authorize]
        [Route("/api/Auction/Bid/{Uid}")]
        public JObject SetBid([FromBody] JObject json)
        {
            if (User.Identity.IsAuthenticated || LoginInfo.Uid > 0)
            {
                var member = _memberRepository.GetMember(LoginInfo.Uid);
                if (member.BidAllowYN == null || member.BidAllowYN.Equals("N"))
                {
                    return JsonHelper.GetApiResultLang("ka.msg.auction.bid_block", IsKor());
                }
                else
                {
                    json["mode"] = "bid";
                    json["reg_ip"] = HttpContext.Connection.RemoteIpAddress.ToString();
                    json["mem_uid"] = LoginInfo.Uid;
                    json["user_agent"] = HttpContext.Request.Headers["User-Agent"].ToString();
                    var result = _auctionRepository.SetBid(json);
                    return JsonHelper.GetApiResultLang(result.Result, IsKor());
                }
            }
            else
            {
                // return JsonHelper.GetApiResultLang("ACCESSDENY", IsKor());
                return JsonHelper.GetApiResultLang("ka.msg.common.expired", IsKor());
            }
        }

        /// <summary>
        /// 경매 작품 자동응찰 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Authorize]
        [Route("/api/Auction/BidPre/{WorkSeq}")]
        public JObject SetBidPre([FromBody] JObject json)
        {
            if (User.Identity.IsAuthenticated || LoginInfo.Uid > 0)
            {
                var member = _memberRepository.GetMember(LoginInfo.Uid);
                if (member.BidAllowYN is null or "N")
                {
                    return JsonHelper.GetApiResultLang("ka.msg.auction.bid_block", IsKor());
                }
                
                json["mode"] = "bid_pre";
                json["reg_ip"] = HttpContext.Connection.RemoteIpAddress.ToString();
                json["mem_uid"] = LoginInfo.Uid;
                json["user_agent"] = HttpContext.Request.Headers["User-Agent"].ToString();
                var result = _auctionRepository.SetBid(json);
                return JsonHelper.GetApiResultLang(result.Result, IsKor());
            }
            else
            {
                // return JsonHelper.GetApiResult("ACCESSDENY");
                return JsonHelper.GetApiResultLang("ka.msg.common.expired", IsKor());
            }
        }

        /// <summary>
        /// 경매 별 과거 경매 일정 목록 처리
        /// </summary>
        /// <param name="json"></param>
        /// <param name="auction"></param>
        /// <returns></returns>
        [Route("/api/Auction/Schedule/{auction}")]
        public JObject GetScheduleList(string auction, [FromBody] JObject json = null)
        {
            json["page_size"] = 10;
            json["mem_uid"] = LoginInfo.Uid;
            var list = _auctionRepository.GetAuctionSchedules(GetAuctionKind(auction), "result", json);
            foreach (var item in list)
            {
                item.IsKor = IsKor();
            }
            return JsonHelper.GetApiResult("00", list);
        }

        /// <summary>
        /// 경매 작품 검색 이력 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route("/api/Auction/WorkSearchHst")]
        public JObject SetAuctionWorkSearchHst([FromBody] JObject json = null)
        {
            if (json == null) return null;

            json["mode"] = "INSERT";
            json["mem_uid"] = LoginInfo.Uid;
            json["token"] = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"].ToString() : "";
            var result = _auctionRepository.SetAuctionWorkSearchHst(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        /// <summary>
        /// 경매 별 작가 목록 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route("/api/Auction/AuctionWorkArtists")]
        public JObject GetAuctionWorkArtists([FromBody] JObject json)
        {
            var list = _auctionRepository.GetAuctionWorkArtists(json);
            foreach (var item in list)
            {
                item.IsKor = true;
            }
            return JsonHelper.GetApiResult("00", list);
        }

        /// <summary>
        /// 작가 검색 (자동완성 처리)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route("/api/Auction/GetSearchArtists")]
        [HttpGet]
        public JsonResult GetArtists(string keyword = "")
        {
            var list = _auctionRepository.GetArtists(new JObject() { ["mode"] = "join", ["keyword"] = keyword });
            foreach (var item in list)
            {
                item.IsKor = IsKor();
            }
            return Json(list);
        }

        #endregion

        #region # 진행경매 #

        [Route("/Auction2/{auction}/{aucNum}")]
        public IActionResult List(string auction, string aucNum, [FromQuery(Name = "type")] string type = "")
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
                return RedirectToAction("List", new { auction, aucNum, type });
            }

            var aucKind = GetAuctionKind(auction);

            // /경매명/Public 으로 접속 시 최근 경매로 처리
            if (!string.IsNullOrWhiteSpace(aucNum) && aucNum.ToLower().Equals("public"))
            {
                var currentAuction = _mainRepository.GetCurrentAuctions();
                foreach (var item in currentAuction)
                {
                    if (item.AucKind.Equals(aucKind))
                    {
                        aucNum = item.AucNum.ToString();
                    }
                }
            }

            if (!int.TryParse(aucNum, out int _)) return RedirectError();

            var auctionSchedule = _auctionRepository.GetAuctionSchedule(aucKind, int.Parse(aucNum), LoginInfo.Uid);

            // 경매 데이터가 없거나, 직원이 아닌 경우 진행될 경매 페이지를 접근할시 에러 페이지로 이동 처리.
            if (auctionSchedule == null) { return RedirectError("wrong"); }

            // 경매 결과 페이지에서 AuthYN이 Y인 경매만 상세보기 활성화 되며, 바로 접근시에서 해당 권한 체크
            if (!auctionSchedule.AuthYN.Equals("Y")) { return RedirectError("wrong"); }

            if (DateTime.Now < auctionSchedule.AucPreviewDate)
            {
                if (!(IsLogin() & LoginInfo.ManagerYN.Equals("Y")) && DateTime.Now < auctionSchedule.AucStartDate) 
                { 
                    // return RedirectError("wrong");
                    return RedirectError("auction_list", auctionSchedule.Uid);
                }
            }

            auctionSchedule.IsKor = IsKor();

            var auctionLiveRequest = aucKind.Equals("1") ? _auctionRepository.GetAuctionLiveRequestInfo(new JObject()
            {
                ["mode"] = "check",
                ["auc_num"] = aucNum,
                ["mem_uid"] = LoginInfo.Uid
            }) : new AuctionLiveRequest();

            ViewBag.Auction = auction;
            ViewBag.AucKind = aucKind;
            ViewBag.AucNum = aucNum;
            ViewBag.WorkType = type;
            ViewBag.EndYN = DateTime.Now > auctionSchedule.AucEndDate;
            ViewBag.EndDate = auctionSchedule.AucEndDate.ToString("yyyy-MM-dd");

            // LiveState (리스트 상단 표기 구분)
            var liveState = "";
            if (auctionSchedule.AucKind.Equals("1"))
            {
                if (auctionSchedule.OnlineStartYN.Equals("Y") && auctionSchedule.AucDate.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    liveState = "L1";
                }
                else if (!IsLogin() || auctionLiveRequest == null) // 비로그인 또는 신청 이력이 없을때
                {
                    liveState = DateTime.Now <= auctionSchedule.AucOnlineBidEndDate ? "L2" : "L3";
                }
                else if (auctionLiveRequest != null && auctionLiveRequest.State != null) // 신청 결과가 있는 경우
                {
                    liveState = auctionLiveRequest.State.Equals("A") && auctionLiveRequest.PaddleNum > 0 ? "L4" : "L5";
                }

                if ((!string.IsNullOrWhiteSpace(auctionSchedule.AucStatCd) && (auctionSchedule.AucStatCd.Equals("F") || auctionSchedule.AucStatCd.Equals("E")))
                    || DateTime.Now >= auctionSchedule.AucEndDate.AddHours(9)) // 라이브 경매 상태가 마감(F) 또는 종료(E) 이거나, 종료가 된 경우
                {
                    liveState = "";
                }
            }

            ViewData["Title"] = GetPageTitle("auction", (DateTime.Now > auctionSchedule.AucEndDate.AddHours(8) ? "list" : "listresult")
                , IsKor() ? auctionSchedule.AucTitle : auctionSchedule.AucTitleEn);
            return View(new AuctionListViewModel
            {
                LiveState = liveState,
                BreadcrumbLevel1 = aucKind.Equals("1") ? "Live Auction" : "Online Auction",
                BreadcrumbLevel2 = DateTime.Now > auctionSchedule.AucEndDate ? "경매결과" :  MessageHelper.GetTitleFromAucKind(aucKind, Lang),
                BreadcrumbLevel3 = DateTime.Now > auctionSchedule.AucEndDate 
                ? (IsKor() ? (auctionSchedule.AucKind.Equals("1") ? auctionSchedule.AucDate.ToString("yy년MM월dd일") : auctionSchedule.AucEndDate.ToString("yy년MM월dd일")) 
                    : (auctionSchedule.AucKind.Equals("1") ? auctionSchedule.AucDate.ToString("yy.MM.dd") : auctionSchedule.AucEndDate.ToString("yy.MM.dd")))
                : "",
                DisplayAucTitle = auctionSchedule.DisplayAucTitle,
                DisplayAucDate = MessageHelper.GetDisplayStartDate(auctionSchedule.AucDate, Lang),
                DisplayAucStartDate = MessageHelper.GetDisplayStartDate(auctionSchedule.AucStartDate, Lang),
                DisplayAucEndDate = MessageHelper.GetDisplayEndDate(auctionSchedule.AucEndDate, Lang),
                AuctionSchedule = auctionSchedule,
                AuctionWorkType = GetAuctionWorkTypeFromString(auctionSchedule.WorkType),
                AuctionLiveRequest = auctionLiveRequest ?? new AuctionLiveRequest()
            });
        }

        /// <summary>
        /// 퍼블 테스트용
        /// </summary>
        /// <param name="auction"></param>
        /// <param name="aucNum"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("/Auction/{auction}/{aucNum}")]
        public IActionResult List2(string auction, string aucNum, [FromQuery(Name = "type")] string type = "")
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
                return RedirectToAction("List2", new { auction, aucNum, type });
            }

            var aucKind = GetAuctionKind(auction);

            // /경매명/Public 으로 접속 시 최근 경매로 처리
            if (!string.IsNullOrWhiteSpace(aucNum) && aucNum.ToLower().Equals("public"))
            {
                var currentAuction = _mainRepository.GetCurrentAuctions();
                foreach (var item in currentAuction)
                {
                    if (item.AucKind.Equals(aucKind))
                    {
                        aucNum = item.AucNum.ToString();
                    }
                }
            }

            if (!int.TryParse(aucNum, out int _)) return RedirectError();

            var auctionSchedule = _auctionRepository.GetAuctionSchedule(aucKind, int.Parse(aucNum), LoginInfo.Uid);

            // 경매 데이터가 없거나, 직원이 아닌 경우 진행될 경매 페이지를 접근할시 에러 페이지로 이동 처리.
            if (auctionSchedule == null) { return RedirectError("wrong"); }

            // 경매 결과 페이지에서 AuthYN이 Y인 경매만 상세보기 활성화 되며, 바로 접근시에서 해당 권한 체크
            if (!auctionSchedule.AuthYN.Equals("Y")) { return RedirectError("wrong"); }

            if (DateTime.Now < auctionSchedule.AucPreviewDate)
            {
                if (!(IsLogin() & LoginInfo.ManagerYN.Equals("Y")) && DateTime.Now < auctionSchedule.AucStartDate)
                {
                    return RedirectError("auction_list", auctionSchedule.Uid);
                }
            }

            auctionSchedule.IsKor = IsKor();

            var auctionLiveRequest = aucKind.Equals("1") ? _auctionRepository.GetAuctionLiveRequestInfo(new JObject()
            {
                ["mode"] = "check",
                ["auc_num"] = aucNum,
                ["mem_uid"] = LoginInfo.Uid
            }) : new AuctionLiveRequest();

            ViewBag.Auction = auction;
            ViewBag.AucKind = aucKind;
            ViewBag.AucNum = aucNum;
            ViewBag.WorkType = type;
            ViewBag.EndYN = DateTime.Now > auctionSchedule.AucEndDate;
            ViewBag.EndDate = auctionSchedule.AucEndDate.ToString("yyyy-MM-dd");

            // 국내개인/법인 휴대폰/신용카드 인증 유무 체크
            var memberAuth = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "check_mem_auth", ["mem_uid"] = LoginInfo.Uid });
            ViewBag.AuthFlag = memberAuth.Any() && memberAuth.First().Result.Equals("Y");

            // LiveState (리스트 상단 표기 구분)
            var liveState = "";
            if (auctionSchedule.AucKind.Equals("1"))
            {
                if (auctionSchedule.OnlineStartYN.Equals("Y") && auctionSchedule.AucDate.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    liveState = "L1";
                }
                else if (!IsLogin() || auctionLiveRequest == null) // 비로그인 또는 신청 이력이 없을때
                {
                    liveState = DateTime.Now <= auctionSchedule.AucOnlineBidEndDate ? "L2" : "L3";
                }
                else if (auctionLiveRequest != null && auctionLiveRequest.State != null) // 신청 결과가 있는 경우
                {
                    liveState = auctionLiveRequest.State.Equals("A") && auctionLiveRequest.PaddleNum > 0 ? "L4" : "L5";
                }

                if ((!string.IsNullOrWhiteSpace(auctionSchedule.AucStatCd) && (auctionSchedule.AucStatCd.Equals("F") || auctionSchedule.AucStatCd.Equals("E")))
                    || DateTime.Now >= auctionSchedule.AucEndDate.AddHours(9)) // 라이브 경매 상태가 마감(F) 또는 종료(E) 이거나, 종료가 된 경우
                {
                    liveState = "";
                }
            }

            // Lot 목록 정보
            var lotInfo = _auctionRepository.GetAuctionWorks(auctionSchedule.AucKind, auctionSchedule.AucNum, 0, null, "lot_list");
            foreach (var item in lotInfo)
            {
                item.IsKor = IsKor();
                item.ThumFileURL = GetThumbImageUrl(auction, int.Parse(aucNum), item.ThumFileName);
            }

            ViewData["Title"] = GetPageTitle("auction", (DateTime.Now > auctionSchedule.AucEndDate.AddHours(8) ? "listresult" : "list"), IsKor() ? auctionSchedule.AucTitle : auctionSchedule.AucTitleEn);
            return View(new AuctionListViewModel
            {
                // ArtistList = artistList,
                LotList = lotInfo,
                LiveState = liveState,
                BreadcrumbLevel1 = aucKind.Equals("1") ? "Live Auction" : "Online Auction",
                BreadcrumbLevel2 = DateTime.Now > auctionSchedule.AucEndDate ? "경매결과" : MessageHelper.GetTitleFromAucKind(aucKind, Lang),
                BreadcrumbLevel3 = DateTime.Now > auctionSchedule.AucEndDate
                ? (IsKor() ? (auctionSchedule.AucKind.Equals("1") ? auctionSchedule.AucDate.ToString("yy년MM월dd일") : auctionSchedule.AucEndDate.ToString("yy년MM월dd일"))
                    : (auctionSchedule.AucKind.Equals("1") ? auctionSchedule.AucDate.ToString("yy.MM.dd") : auctionSchedule.AucEndDate.ToString("yy.MM.dd")))
                : "",
                DisplayAucTitle = auctionSchedule.DisplayAucTitle,
                DisplayAucDate = MessageHelper.GetDisplayStartDate(auctionSchedule.AucDate, Lang),
                DisplayAucStartDate = MessageHelper.GetDisplayStartDate(auctionSchedule.AucStartDate, Lang),
                DisplayAucEndDate = MessageHelper.GetDisplayEndDate(auctionSchedule.AucEndDate, Lang),
                AuctionSchedule = auctionSchedule,
                AuctionWorkType = GetAuctionWorkTypeFromString(auctionSchedule.WorkType),
                AuctionLiveRequest = auctionLiveRequest ?? new AuctionLiveRequest()
            });
        }

        #endregion

        #region # 경매결과 #

        [Route("/Auction/{auction}/Result")]
        public IActionResult Result(string auction)
        {
            var AucKind = GetAuctionKind(auction);
            ViewBag.Auction = auction;
            ViewBag.AucKind = AucKind;
            ViewData["Title"] = GetPageTitle("auction", "result", (auction.ToUpper().Equals("MAJOR") ? "Live" : auction) + " Auction");
            return View();
        }

        #endregion

        #region # 작품상세 #

        /// <summary>
        /// 2022.07.20 [#667/계획] ISMS 국내 회원가입 본인인증 선택으로 변경+응찰필수조건처리 - 응찰 필수정보 등록 버튼 활성 유무를 위하여 memberAuth 추가
        /// </summary>
        /// <param name="auction"></param>
        /// <param name="aucNum"></param>
        /// <param name="seq"></param>
        /// <param name="lotNum"></param>
        /// <returns></returns>
        [Route("/Auction/{auction}/{aucNum}/{seq}")]
        [Route("/Auction/{auction}/{aucNum}/Lot{lotNum}")]
        public IActionResult Work(string auction, string aucNum, string seq, string lotNum = "")
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
                return RedirectToAction("Work", new { auction, aucNum, seq, lotNum });
            }

            // 라우팅 규칙에 Lot{번호} 규칙 추가하여 분기 처리 (ex. /Lot1)
            var auctionWork = !string.IsNullOrWhiteSpace(lotNum) 
                ? _auctionRepository.GetAuctionWork(GetAuctionKind(auction), int.TryParse(aucNum, out int _aucNum) ? _aucNum : 0, int.TryParse(lotNum, out int _lotNum) ? _lotNum : 0, User.Identity.IsAuthenticated ? LoginInfo.Uid : 0)
                : _auctionRepository.GetAuctionWork(int.TryParse(seq, out int _seq) ? _seq : 0, User.Identity.IsAuthenticated ? LoginInfo.Uid : 0);

            if (auctionWork == null) return RedirectError();
            
            auctionWork.IsKor = IsKor();

            seq ??= auctionWork.Uid.ToString();

            // 직원이 아닌 경우 진행될 경매 페이지를 접근할시 에러 페이지로 이동 처리. (리스트와 동일 체크 조건)
            if (DateTime.Now < auctionWork.AucPreviewDate)
            {
                if (!(IsLogin() & LoginInfo.ManagerYN.Equals("Y")) && DateTime.Now < auctionWork.AucStartDate)
                {
                    return RedirectError("auction_work", auctionWork.Uid);
                }
            }      

            // 응찰 가능 여부 체크
            DateTime DatabaseTime = _commonRepository.GetDatabaseTime();
            TimeSpan Time = auctionWork.EndTime - DatabaseTime;

            // 자동 낙찰 처리
            var restSecond = (Time.TotalSeconds < 0) ? -1 : Time.TotalSeconds;
            if (restSecond < 0 && auctionWork.FinishYN != null && auctionWork.FinishYN.Equals("N"))
            {
                _auctionRepository.SetAuctionWork("nak_proc", auctionWork.Uid);
            }

            // 출품 작품이며, 활성화 경매 또는 관리자 인 경우 접근 가능하게 처리
            if (auctionWork.ExhiYN.Equals("Y") && (auctionWork.LiveYN.Equals("Y") || auctionWork.PreviewYN.Equals("Y")))
            {
                // 작품이미지 정보
                var auctionWorkImages = _auctionRepository.GetAuctionWorkImages(int.Parse(seq));
                foreach (var item in auctionWorkImages)
                {
                    item.ImgFileName = GetImageUrl(auction, int.Parse(aucNum), item.ImgFileName);
                    item.ImgFileNameThum = GetImageUrl(auction, int.Parse(aucNum), item.ImgFileNameThum);
                }

                // 작품 상세 조회시 사이트 썸네일 이미지를 대표 이미지로 변경
                if (auctionWorkImages.Where(x => x.ImgType.Equals("04")).Any())
                {
                    ViewBag.SiteThumbNailImage = auctionWorkImages.Where(x => x.ImgType.Equals("04")).First().ImgFileName;
                }
                else if (auctionWorkImages.Where(x => x.ImgType.Equals("01")).Any())
                {
                    ViewBag.SiteThumbNailImage = auctionWorkImages.Where(x => x.ImgType.Equals("01")).First().ImgFileName;
                }

                // 작가의 다른 작품 정보
                var artistWorks = _auctionRepository.GetArtistWorks(auctionWork.ArtistUid, auctionWork.Uid, User.Identity.IsAuthenticated ? LoginInfo.Uid : 0);
                foreach (var item in artistWorks)
                {
                    item.IsKor = IsKor();
                    item.ThumFileName = GetImageUrl(item.AucKind, item.AucNum, item.ThumFileName);
                }

                // 추가 정보
                var auctionWorkAdditionalInfo = _auctionRepository.GetAuctionWorkAdditionalInfos("W", auctionWork.Uid, 0);
                var artistWorkAdditionalInfo = _auctionRepository.GetAuctionWorkAdditionalInfos("A", auctionWork.Uid, 0);
                var artistImages = _auctionRepository.GetImages(auctionWork.ArtistUid);
                var AuctionWorkCondition = _auctionRepository.GetAuctionWorkCondition(auctionWork.Uid, IsKor() ? "KO" : "EN");

                // Lot 목록 정보
                var lotInfo = _auctionRepository.GetAuctionWorks(auctionWork.AucKind, auctionWork.AucNum, 0, null, "lot_list");
                foreach (var item in lotInfo)
                {
                    item.IsKor = IsKor();                    
                    item.ThumFileURL = GetThumbImageUrl(auction, int.Parse(aucNum), item.ThumFileName);
                }

                ViewBag.Auction = auction;
                ViewBag.AucNum = aucNum;
                ViewBag.AucKind = auctionWork.AucKind;

                var memberMobileAuth = _memberRepository.GetMember(new Member { ID = LoginInfo.ID }, "mobile_agree_check");

                // 주소 정보가 등록되었는지 체크
                var memberAddress = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid);

                // 국내개인/법인 휴대폰/신용카드 인증 유무 체크
                var memberAuth = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "check_mem_auth", ["mem_uid"] = LoginInfo.Uid });

                var member = _memberRepository.GetMember(Uid);
                
                ViewData["Title"] = GetPageTitle("auction", (DateTime.Now > auctionWork.EndTime ? "detailresult" : "detail"), (IsKor() ? auctionWork.AucTitle : auctionWork.AucTitleEn) + " Lot." + auctionWork.LotNum.ToString());
                var auctionWorkAdditionalInfos = auctionWorkAdditionalInfo as AuctionWorkAdditionalInfo[] ?? auctionWorkAdditionalInfo.ToArray();
                return View(new AuctionWorkViewModel
                {
                    LotList = lotInfo,
                    IsRequireMobileAuth = memberMobileAuth.Result,
                    MobileAuthType = memberMobileAuth.Type,
                    BreadcrumbLevel1 = GetAuctionKind(auction).Equals("1") ? "Auction" : "Online Auction",
                    BreadcrumbLevel2 = MessageHelper.GetTitleFromAucKind(GetAuctionKind(auction), IsKor() ? "KOR" : "ENG"),
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    IsBid = !auctionWork.PreviewYN.Equals("Y") && Time.TotalSeconds > 0,
                    BidRemainTime = auctionWork.EndTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    BidStartRemainTime = auctionWork.StartTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    AuctionWork = auctionWork,
                    AuctionWorkAdditionalInfos = auctionWorkAdditionalInfos,
                    AuctionWorkImages = auctionWorkImages,
                    AuctionWorkCondition = AuctionWorkCondition,
                    ArtistWorks = artistWorks,
                    WorkAddtionalInfo = auctionWorkAdditionalInfos.Where(x => x.TCode.Equals("W") && (x.Flag.Equals("EL") || x.Flag.Equals("EB")) && !string.IsNullOrWhiteSpace(x.Value1) && !string.IsNullOrWhiteSpace(x.Value2)),
                    WorkAdditionalImages = auctionWorkAdditionalInfos.Where(x => x.TCode.Equals("W") && (x.Flag.Equals("EI") || x.Flag.Equals("EU") || x.Flag.Equals("ED"))),
                    ArtistAdditionalInfo = artistWorkAdditionalInfo.Where(x => x.TCode.Equals("A") && (x.Flag.Equals("EL") || x.Flag.Equals("EB")) && !string.IsNullOrWhiteSpace(x.Value1) && !string.IsNullOrWhiteSpace(x.Value2)),
                    ArtistAdditionalImages = artistWorkAdditionalInfo.Where(x => x.TCode.Equals("A") && (x.Flag.Equals("EI") || x.Flag.Equals("EU") || x.Flag.Equals("ED"))),
                    ArtistImages = artistImages,
                    ServerTime = DateTime.Now.ToString("MMM dd, yyyy HH|mm|ss zzz", CultureInfo.InvariantCulture).Replace(":", "").Replace('|', ':'),
                    DeliveryPrice = _auctionRepository.GetDeliveryPrice(auctionWork.AucKind, auctionWork.AucNum, LoginInfo.Uid).TotalPrice.ToString(),
                    AddressFlag = memberAddress.Any(),
                    AuthFlag = memberAuth.Any() && memberAuth.First().Result.Equals("Y"),
                    IsOverSeas = member?.Type is "002" or "004"
                });
            }
            else
            {
                return auctionWork.AucEndDate < DateTime.Now ? RedirectError("closedLot") : RedirectError();
            }
        }

        #endregion

        #region # 인쇄 #
        
        [Route("/Print/{auction}/{aucNum}/{seq}")]
        public IActionResult Print(string auction, string aucNum, string seq)
        {
            var AuctionWork = _auctionRepository.GetAuctionWork(int.Parse(seq), User.Identity.IsAuthenticated ? LoginInfo.Uid : 0);

            AuctionWork.IsKor = IsKor();

            var AuctionWorkImages = _auctionRepository.GetAuctionWorkImages(int.Parse(seq));
            var AuctionWorkCondition = _auctionRepository.GetAuctionWorkCondition(AuctionWork.Uid, IsKor() ? "KO" : "EN");
            
            // 추가 정보
            var AuctionWorkAdditionalInfo = _auctionRepository.GetAuctionWorkAdditionalInfos("W", AuctionWork.Uid, 0);
            var ArtistWorkAdditionalInfo = _auctionRepository.GetAuctionWorkAdditionalInfos("A", AuctionWork.Uid, 0);

            foreach (var item in AuctionWorkImages)
            {
                item.ImgFileName = GetImageUrl(auction, int.Parse(aucNum), item.ImgFileName);
                item.ImgFileNameThum = GetImageUrl(auction, int.Parse(aucNum), item.ImgFileNameThum);
            }


            ViewBag.Auction = auction;
            ViewBag.AucNum = aucNum;
            ViewBag.AucKind = AuctionWork.AucKind;

            // 출품 작품이며, 활성화 경매 또는 관리자 인 경우 접근 가능하게 처리
            if (AuctionWork.ExhiYN.Equals("Y") && (AuctionWork.LiveYN.Equals("Y")))
            {
                return View(new AuctionWorkViewModel
                {
                    BreadcrumbLevel1 = GetAuctionKind(auction).Equals("1") ? "Auction" : "Online Auction",
                    BreadcrumbLevel2 = MessageHelper.GetTitleFromAucKind(GetAuctionKind(auction), IsKor() ? "KOR" : "ENG"),
                    AuctionWork = AuctionWork,
                    AuctionWorkCondition = AuctionWorkCondition,
                    AuctionWorkImages = AuctionWorkImages,
                    AuctionWorkAdditionalInfos = AuctionWorkAdditionalInfo,
                    WorkAddtionalInfo = AuctionWorkAdditionalInfo.Where(x => x.TCode.Equals("W") && (x.Flag.Equals("EL") || x.Flag.Equals("EB")) && !string.IsNullOrWhiteSpace(x.Value1) && !string.IsNullOrWhiteSpace(x.Value2)),
                    WorkAdditionalImages = AuctionWorkAdditionalInfo.Where(x => x.TCode.Equals("W") && (x.Flag.Equals("EI") || x.Flag.Equals("EU") || x.Flag.Equals("ED"))),
                    ArtistAdditionalInfo = ArtistWorkAdditionalInfo.Where(x => x.TCode.Equals("A") && (x.Flag.Equals("EL") || x.Flag.Equals("EB")) && !string.IsNullOrWhiteSpace(x.Value1) && !string.IsNullOrWhiteSpace(x.Value2)),
                    ArtistAdditionalImages = ArtistWorkAdditionalInfo.Where(x => x.TCode.Equals("A") && (x.Flag.Equals("EI") || x.Flag.Equals("EU") || x.Flag.Equals("ED")))
                });
            }
            else
            {
                return RedirectError();
            }            
        }
        #endregion

        #endregion

        #region # 메이저 경매 서면/전화 응찰 신청 #

        #region [WebApi]

        [Authorize]
        [Route("/api/Auction/SetBidApplication/{uid}")]
        public JObject SetBidApplication([FromBody] JObject json)
        {
            if (!User.Identity.IsAuthenticated || LoginInfo.Uid < 1)
            {
                return JsonHelper.GetApiResultLang("ka.msg.common.expired", IsKor());
            }

            // 회원 응찰 가능 여부 체크
            var member = _memberRepository.GetMember(LoginInfo.Uid);
            if (member.BidAllowYN == null || member.BidAllowYN.Equals("N"))
            {
                return JsonHelper.GetApiResultLang("ka.msg.auction.bid_block", IsKor());
            }

            // 서면 접수 가능 여부 체크
            var auctionWork = _auctionRepository.GetAuctionWork(int.TryParse(JsonHelper.GetString(json, "work_uid", "0"), out int workUid) ? workUid : 0);
            if (auctionWork == null || auctionWork.BidYN.Equals("N"))
            {
                return JsonHelper.GetApiResultLang("ka.msg.auction.placebidEnd", IsKor());
            }

            // 메일 제목에 상향/하향 문구 추가
            var bidType = JsonHelper.GetString(json, "bid_type");
            var currentBidPre = Convert.ToDecimal(JsonHelper.GetString(json, "price_bid_pre", "0"));
            var subjectTag = string.Empty;
            var beforeBidPre = 0.0m;
            var beforeBidFlag = false;
            if (!bidType.Equals("2"))
            {
                var auctionBids = _auctionRepository.GetMemberBidPreinfo(workUid, LoginInfo.Uid);
                if (auctionBids.Any())
                {
                    beforeBidPre = auctionBids.First().PriceBid;
                    beforeBidFlag = true;
                    subjectTag = beforeBidPre > currentBidPre ? "(하향)" : (beforeBidPre < currentBidPre) ? "(상향)" : "(동일)";
                }
            }

            // 신청 처리
            json["mode"] = "major_bid_pre";
            json["mem_uid"] = LoginInfo.Uid;
            json["reg_ip"] = HttpContext.Connection.RemoteIpAddress.ToString();
            json["lang"] = IsKor() ? "K" : "E";

            var result = _auctionRepository.SetAuctionBidPre(json);

            if (result.Result.Equals("00"))
            {
                var serverTime = _commonRepository.GetDatabaseTime();
                var work = _auctionRepository.GetAuctionWork(int.Parse(JsonHelper.GetString(json, "work_uid", "0")));

                StringBuilder emailBody = new();

                // 담당자 메일 발송
                var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "BidAppToManager", ["uid"] = LoginInfo.Uid });
                if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                {
                    emailBody.Append($"{serverTime:yyyy년 MM월 dd일 HH:mm}에 홈페이지를 통해 서면/전화 응찰이 접수되었으니 케이오피스에서 확인 부탁 드립니다.<br /><br />");
                    emailBody.Append("[응찰자 정보]<br />");
                    emailBody.Append($"이름 : {StringHelper.GetPrivateInfoMask(member.Name, "N")}<br />");
                    emailBody.Append($"담당자 : {member.MngName}<br />");
                    emailBody.Append($"휴대전화 : {StringHelper.GetPrivateInfoMask(member.Mobile, "M")}<br />");
                    emailBody.Append($"회원번호(홈페이지) : {member.Uid}<br />");
                    emailBody.Append($"회원번호(케이오피스) : {member.KofficeUid}<br /><br />");

                    emailBody.Append("[응찰 작품 정보]<br />");
                    emailBody.Append($"경매명 : {work.AucTitle}<br />");
                    emailBody.Append($"응찰 방식 : {(bidType.Equals("0") ? "서면응찰" : bidType.Equals("2") ? "전화응찰" : "서면응찰 & 전화응찰")}<br />");
                    emailBody.Append($"Lot : {work.LotNum}<br />");
                    emailBody.Append($"작가명 : {work.ArtistName}<br/>");
                    emailBody.Append($"작품명 : {work.Title}<br/>");
                    emailBody.Append($"낮은 추정가 : {(work.SeparateInquiryYN.Equals("Y") ? "별도문의" : string.Concat("KRW ", work.PriceEstimatedLow.ToString("##,###,##0")))}<br />");
                    if (beforeBidFlag)
                    {
                        emailBody.Append($"기존 최대 응찰 금액 : {(string.Concat("KRW ", beforeBidPre.ToString("##,###,##0")))}<br />");
                    }
                    if (!bidType.Equals("2"))
                    {
                        emailBody.Append($"최대 응찰 금액 : {(string.Concat("KRW ", currentBidPre.ToString("##,###,##0")))}<br />");
                    }

                    var emailInfo = new Email()
                    {
                        AddToEmail = GetEmail(code.Extra1),
                        AddToName = GetEmail(code.Extra1, "N"),
                        AddCcEmail = code.Extra3.Equals("Y")
                            ? GetEmail(code.Extra2).Concat(GetMemberTeamEmail(member.MngTeamEmail)).ToArray()
                            : (code.Extra3.Equals("M") && !string.IsNullOrWhiteSpace(member.MngEmail) ? new string[] { member.MngEmail } : GetEmail(code.Extra2)),
                        AddCcName = code.Extra3.Equals("Y")
                            ? GetEmail(code.Extra2, "N").Concat(GetMemberTeamEmail(member.MngTeamEmail, "N")).ToArray()
                            : (code.Extra3.Equals("M") && !string.IsNullOrWhiteSpace(member.MngEmail) ? new string[] { member.MngName } : GetEmail(code.Extra2, "N")),
                        SubJect = "[홈페이지알림] 홈페이지 서면/전화 응찰 접수" + subjectTag,
                        Body = emailBody.ToString(),
                        Type = "default",
                        IsBodyHtml = true,
                        Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                    };
                    emailInfo.Site = "P";
                    emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                    emailInfo.Type = "BidAppToManager";
                    emailInfo.RegUid = LoginInfo.Uid;
                    _logRepository.SetEmailLog(emailInfo);

                    emailBody.Clear();
                }

                // 신청자 메일 발송
                code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "BidAppToMember", ["uid"] = LoginInfo.Uid });
                if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                {
                    emailBody.Append($"{serverTime:yyyy년 MM월 dd일 HH:mm}에 홈페이지를 통해 서면/전화 응찰이 접수되었으니 'My Page > 라이브 경매 응찰 내역' 에서 확인 부탁 드립니다.<br /><br />");
                    emailBody.Append("[응찰자 정보]<br />");
                    emailBody.Append($"이름 : {StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N")}<br />");
                    if (!string.IsNullOrWhiteSpace(LoginInfo.MngEmail))
                    {
                        emailBody.Append($"담당자 : {LoginInfo.MngName}<br />");
                    }
                    emailBody.Append($"휴대전화 : {StringHelper.GetPrivateInfoMask(LoginInfo.Mobile, "M")}<br /><br />");

                    emailBody.Append("[응찰 작품 정보]<br />");
                    emailBody.Append($"경매명 : {work.AucTitle}<br />");
                    emailBody.Append($"응찰 방식 : {(bidType.Equals("0") ? "서면응찰" : bidType.Equals("2") ? "전화응찰" : "서면응찰 & 전화응찰")}<br />");
                    emailBody.Append($"Lot : {work.LotNum}<br />");
                    emailBody.Append($"작가명 : {work.ArtistName}<br/>");
                    emailBody.Append($"작품명 : {work.Title}<br/>");
                    emailBody.Append($"낮은 추정가 : {(work.SeparateInquiryYN.Equals("Y") ? "별도문의" : string.Concat("KRW ", work.PriceEstimatedLow.ToString("##,###,##0")))}<br />");
                    if (!bidType.Equals("2"))
                    {
                        emailBody.Append($"최대 응찰 금액 : {(string.Concat("KRW ", Convert.ToDecimal(JsonHelper.GetString(json, "price_bid_pre", "0")).ToString("##,###,##0")))}<br />");
                    }

                    var emailInfo = new Email()
                    {
                        AddToEmail = GetEmail(code.Extra1),
                        AddToName = GetEmail(code.Extra1, "N"),
                        AddCcEmail = code.Extra3.Equals("Y")
                            ? GetEmail(code.Extra2).Concat(GetMemberTeamEmail(member.MngTeamEmail)).ToArray()
                            : (code.Extra3.Equals("M") && !string.IsNullOrWhiteSpace(member.MngEmail) ? new string[] { member.MngEmail } : GetEmail(code.Extra2)),
                        AddCcName = code.Extra3.Equals("Y")
                            ? GetEmail(code.Extra2, "N").Concat(GetMemberTeamEmail(member.MngTeamEmail, "N")).ToArray()
                            : (code.Extra3.Equals("M") && !string.IsNullOrWhiteSpace(member.MngEmail) ? new string[] { member.MngName } : GetEmail(code.Extra2, "N")),
                        SubJect = "[케이옥션] 홈페이지 서면/전화 응찰 신청이 접수 되었습니다.",
                        Body = emailBody.ToString(),
                        Type = "default",
                        IsBodyHtml = true,
                        Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                    };
                    emailInfo.Site = "P";
                    emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                    emailInfo.Type = "BidAppToMember";
                    emailInfo.RegUid = LoginInfo.Uid;
                    _logRepository.SetEmailLog(emailInfo);
                }
            }

            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        [Authorize]
        [Route("/Auction/BidApplication")]
        [Route("/Auction/BidApplication/{uid}")]
        public IActionResult BidApplication(string uid = "")
        {
            int workUid = int.TryParse(uid, out int result) ? result : 0;
            
            bool existBidPreType0 = false;
            bool existBidPreType2 = false;

            decimal priceBidPreType0 = 0;

            if (string.IsNullOrWhiteSpace(uid)) return RedirectError();

            ViewBag.Uid = uid;

            var auctionWork = _auctionRepository.GetAuctionWork(workUid);
            auctionWork.IsKor = IsKor();

            var auctionSchedule = _auctionRepository.GetAuctionSchedule("1", auctionWork.AucNum, LoginInfo.Uid, "detail");
            auctionSchedule.IsKor = IsKor();

            var auctionWorkImages = _auctionRepository.GetAuctionWorkImages(result);

            var auctionBids  =  _auctionRepository.GetMemberBidPreinfo(workUid, LoginInfo.Uid); 

            foreach (var item in auctionBids)
            {
                if (item.BidType.Equals("0") && item.PriceBid > 0)
                {
                    existBidPreType0 = true;
                    priceBidPreType0 = item.PriceBid;
                } else if (item.BidType.Equals("2")) {
                    existBidPreType2 = true;
                }
            }

            foreach (var item in auctionWorkImages)
            {
                switch (auctionWork.AucKind.ToUpper())
                {
                    case "1": item.ImgFileName = $"{Config.ImageDomain}/www/Work/{auctionWork.AucNum.ToString().PadLeft(4, '0')}/{item.ImgFileName}"; break;
                    case "2": item.ImgFileName = $"{Config.ImageDomain}/www/KMall/Work/{auctionWork.AucNum.ToString().PadLeft(4, '0')}/{item.ImgFileName}"; break;
                    case "4": item.ImgFileName = $"{Config.ImageDomain}/www/Konline/Work/{auctionWork.AucNum.ToString().PadLeft(4, '0')}/{item.ImgFileName}"; break;
                }
            }

            var member = _memberRepository.GetMember(LoginInfo.Uid);

            // [운영] 첫 경매 참여 전 주소정보 수집 (#643)
            // - 서면/전화 응찰 시 주소정보 등록 여부에 따른 프로세스 분기
            var memberAddress = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid);

            ViewData["Title"] = GetPageTitle("auction", "bidapplication");
            return View(new BidApplicationViewModel
            {
                ExistBidPreType0 = existBidPreType0,
                ExistBidPreType2 = existBidPreType2,
                PriceBidPreType0 = priceBidPreType0,
                MemberBidAllowYn = member.BidAllowYN.Equals("Y"),
                AuctionWork = auctionWork,
                AuctionSchedule = auctionSchedule,
                AuctionWorkImages = auctionWorkImages,
                AddressFlag = memberAddress.Any()
            });
        }

        #endregion

        #endregion

        #region # 온라인 라이브 응찰 신청 #

        #region [WebApi]

        [Authorize]
        [Route("/api/Auction/SetLiveRequest/{AucNum}")]
        public JObject SetLiveRequest(string aucNum)
        {
            if (string.IsNullOrWhiteSpace(aucNum)) return JsonHelper.GetApiResultLang("90", IsKor());

            if (!User.Identity.IsAuthenticated || LoginInfo.Uid < 1)
            {
                return JsonHelper.GetApiResultLang("ka.msg.common.expired", IsKor());
            }

            // 온라인 응찰 접수 가능 여부 체크
            var auction = _auctionRepository.GetAuctionSchedule("1", int.TryParse(aucNum, out int resultAucNum) ? resultAucNum : 0, LoginInfo.Uid);
            if (auction == null || auction.OnlineBidYN.Equals("N"))
            {
                return JsonHelper.GetApiResultLang("ka.msg.auction.bid_end", IsKor());
            }

            var result = _auctionRepository.SetAuctionLiveRequest(new JObject
            {
                ["auc_kind"] = "1",
                ["auc_num"] = aucNum,
                ["mem_uid"] = LoginInfo.Uid,
                ["bid_type"] = "5",
                ["location_flag"] = IsKor() ? "K" : "E",
                ["reg_ip"] = HttpContext.Connection.RemoteIpAddress.ToString()
            });

            string resultCode = (int.TryParse(result, out int rsl) ? rsl : -99) switch
            {
                1 => "104",
                -2 => "101",
                -3 => "102",
                -4 => "103",
                -99 => "99",
                _ => "99",
            };

            if (resultCode.Equals("104"))
            {
                var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "LiveRequest", ["uid"] = LoginInfo.Uid });
                if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                {
                    var serverTime = _commonRepository.GetDatabaseTime();
                    var member = _memberRepository.GetMember(LoginInfo.Uid);

                    StringBuilder emailBody = new();
                    emailBody.Append($"{serverTime:yyyy년 MM월 dd일 HH:mm}에 홈페이지를 통해 라이브 경매 응찰이 접수되었으니 케이오피스에서 확인 부탁드립니다.<br /><br />");
                    emailBody.Append("[응찰자 정보]<br />");
                    emailBody.Append($"경매명 : {auction.AucTitle}<br />");
                    emailBody.Append($"이름 : {StringHelper.GetPrivateInfoMask(member.Name, "N")}<br />");
                    emailBody.Append($"담당자 : {member.MngName}<br />");
                    emailBody.Append($"휴대전화 : {StringHelper.GetPrivateInfoMask(member.Mobile, "M")}<br />");
                    emailBody.Append($"회원번호(홈페이지) : {member.Uid}<br />");
                    emailBody.Append($"회원번호(케이오피스) : {member.KofficeUid}<br />");

                    var emailInfo = new Email()
                    {
                        AddToEmail = GetEmail(code.Extra1),
                        AddToName = GetEmail(code.Extra1, "N"),
                        AddCcEmail = code.Extra3.Equals("Y")
                            ? GetEmail(code.Extra2).Concat(GetMemberTeamEmail(member.MngTeamEmail)).ToArray()
                            : (code.Extra3.Equals("M") && !string.IsNullOrWhiteSpace(member.MngEmail) ? new string[] { member.MngEmail } : GetEmail(code.Extra2)),
                        AddCcName = code.Extra3.Equals("Y")
                            ? GetEmail(code.Extra2, "N").Concat(GetMemberTeamEmail(member.MngTeamEmail, "N")).ToArray()
                            : (code.Extra3.Equals("M") && !string.IsNullOrWhiteSpace(member.MngEmail) ? new string[] { member.MngName } : GetEmail(code.Extra2, "N")),
                        SubJect = "[홈페이지알림] 홈페이지 라이브 경매 응찰 신청 접수",
                        Body = emailBody.ToString(),
                        Type = "default",
                        IsBodyHtml = true,
                        Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                    };
                    emailInfo.Site = "P";
                    emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                    emailInfo.Type = "LiveRequest";
                    emailInfo.RegUid = LoginInfo.Uid;
                    _logRepository.SetEmailLog(emailInfo);
                }
            }

            return JsonHelper.GetApiResultLang(resultCode, IsKor());
        }

        #endregion

        #region [View]

        [Authorize]
        [Route("/Auction/LiveRequest")]
        [Route("/Auction/LiveRequest/{AucNum}")]
        public IActionResult LiveRequest(string aucNum)
        {
            if (string.IsNullOrWhiteSpace(aucNum)) return RedirectToAction("Error", "Home");

            var member = _memberRepository.GetMember(LoginInfo.Uid);
            ViewBag.MemberBidAllowYn = member.BidAllowYN.Equals("Y");
            ViewBag.AucNum = aucNum;
            // [운영] 첫 경매 참여 전 주소정보 수집 (#643)
            // - 서면/전화 응찰 시 주소정보 등록 여부에 따른 프로세스 분기
            ViewBag.AddressFlag = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid).Any();
            ViewData["Title"] = GetPageTitle("auction", "liverequest");
            return View();
        }

        #endregion

        #endregion

        #region # Common Function #

        /// <summary>
        /// 수신/참조 이메일 규칙에 맞게 주소 리턴
        /// </summary>
        /// <param name="emailInfo">LOGIN_USER (로그인한 사용자), MANAGER_APPROVER (담당자의 승인권자)</param>
        /// <param name="loginUserMail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string[] GetEmail(string emailInfo, string type = "")
        {
            if (string.IsNullOrWhiteSpace(emailInfo)) return Array.Empty<string>();

            if (emailInfo.Contains("|"))
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
            else if (emailInfo.Equals("LOGIN_USER"))
            {
                return type.ToUpper().Equals("N") ? new string[] { LoginInfo.Name } : new string[] { LoginInfo.Email };
            }
            else if (emailInfo.Equals("MANAGER_APPROVER"))
            {
                var member = _memberRepository.GetMemberKOffice(LoginInfo.Uid);
                if (member != null && !string.IsNullOrWhiteSpace(member.MngApprover))
                {
                    var list = new List<string>();
                    foreach (var item in member.MngApprover.Split(';'))
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
                else
                {
                    return Array.Empty<string>();
                }
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        #endregion
    }
}
