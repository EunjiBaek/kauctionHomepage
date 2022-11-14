using KA.Entities.Helpers;
using KA.Repositories;
using KA.Web.Admin.ViewModels.Auction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;
using KA.Web.Admin.Models;

namespace KA.Web.Admin.Controllers
{
    public class AuctionController : BaseController
    {
        #region # Constructor #

        public AuctionController(ICommonRepository commonRepository,
            IMemberRepository memberRepository,
            IHttpContextAccessor httpContextAccessor,
            IManagerRepository managerRepository,
            IMainRepository mainRepository,
            IAuctionRepository auctionRepository,
            IContentRepository contentRepository,
            ILogRepository logRepository,
            EmailHelper emailHelper)
            : base(commonRepository, memberRepository, httpContextAccessor, managerRepository, mainRepository, auctionRepository, contentRepository, logRepository, emailHelper) { }

        #endregion

        #region # Auction > 메이저 / 프리미엄 / 위클리 경매 #

        #region [WebAPI]
        
        [HttpPost]
        [Route("/api/Auction/GetScheduleList")]
        public JObject GetScheduleList([FromBody] JObject json)
        {
            var data = auctionRepository.GetAuctionSchedules(GetAucKindFromCode(JsonHelper.GetString(json, "code")), "admin", json);
            foreach (var item in data)
            {
                item.IsKor = true;
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [HttpPost]
        [Route("/api/Auction/GetAuctionWorkList")]
        public JObject GetAuctionWorkList([FromBody] JObject json)
        {                       
            var dataWork = auctionRepository.GetAuctionWorks(GetAucKindFromCode(JsonHelper.GetString(json, "code")), int.Parse(JsonHelper.GetString(json, "auc_num")), 0, json, "admin");
            var totalCount = dataWork.ToList().Count > 0 ? dataWork.ToList()[0].TotalCount : 0;
            foreach (var item in dataWork)
            {
                item.IsKor = true;
                item.ThumFileURL = GetImagePath(item.AucKind, item.AucNum, item.ThumFileName, false);
                item.ImgFileURL = GetImagePath(item.AucKind, item.AucNum, item.ImgFileName, false);
                item.ActiveYN = "Y";
            }
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, dataWork, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [HttpPost]
        [Route("/api/Auction/GetAuctionBidList")]
        public JObject GetAuctionBidList([FromBody] JObject json)
        {            
            var dataBid = auctionRepository.GetAuctionBidsByAdmin(json);
            var totalCount = dataBid.ToList().Count > 0 ? dataBid.ToList()[0].TotalCount : 0;
            var paramBidUid = JsonHelper.GetString(json, "bid_uid", "0");
            var paramMaskType = JsonHelper.GetString(json, "mask_type");
            foreach (var item in dataBid)
            {
                item.DisplayMemID = string.IsNullOrWhiteSpace(paramMaskType) ? StringHelper.GetPrivateInfoMask(item.MemID, "I") : item.MemID;
                item.DisplayMemName = string.IsNullOrWhiteSpace(paramMaskType) ? StringHelper.GetPrivateInfoMask(item.MemName, "N") : item.MemName;
                item.DisplayMemMobile = string.IsNullOrWhiteSpace(paramMaskType) ? StringHelper.GetPrivateInfoMask(item.MemMobile, "M") : item.MemMobile;
                item.BidDeviceType = string.IsNullOrWhiteSpace(item.UserAgent) ? "" : (IsMobile(item.UserAgent) ? "모바일" : "웹");
            }

            // 마스킹 해제 이력 조회
            if (!string.IsNullOrWhiteSpace(paramMaskType) && dataBid.Any())
            {                
                managerRepository.SetManagerViewHst(new JObject { ["mng_uid"] = LoginInfo.UID, ["type"] = "M", ["target"] = dataBid.First().MemUid, ["etc1"] = paramBidUid, ["etc2"] = "auction_bid_uid" });
            }

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, dataBid, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [HttpPost]
        [Route("/api/Auction/SetAuctionBid")]
        public JObject SetAuctionBid([FromBody] JObject json)
        {
            json["mng_uid"] = LoginInfo.UID;
            var result = auctionRepository.SetBidByAdmin(json);
            return JsonHelper.GetApiResultLang(result.Result, true);
        }

        [HttpPost]
        [Route("/api/Auction/SetAuctionBidNak")]
        public JObject SetAuctionBidNak([FromBody] JObject json)
        {
            json["mng_uid"] = LoginInfo.UID;
            json["reg_ip"] = HttpContext.Connection.RemoteIpAddress.ToString();
            var result = auctionRepository.SetBidNakByAdmin(json);
            return JsonHelper.GetApiResultLang(result.Result, true);
        }

        [HttpPost]
        [Route("/api/Auction/SetViewInRoom")]
        public JObject SetViewInRoom([FromBody] JObject json)
        {
            json["mode"] = "view_room";
            auctionRepository.SetAuctionWork(json);
            return JsonHelper.GetApiResult("00");
        }

        [HttpPost]
        [Route("/api/Auction/SetAuctionWork")]
        public JObject SetAuctionWork([FromBody] JObject json)
        {
            if (LoginInfo.UID > 0)
            {
                auctionRepository.SetAuctionWork(json);
                return JsonHelper.GetApiResult("00");
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        #endregion

        #region [View]

        [Route("/Auction/Schedule/{code?}")]
        public IActionResult List(string code)
        {
            ViewBag.Code = code;
            return View();
        }

        [Route("/Auction/Schedule/{code}/{aucNum}/{seq}")]
        public IActionResult Info(string code, string aucNum, string seq)
        {
            ViewBag.Code = code;
            ViewBag.AucNum = aucNum;
            ViewBag.Seq = seq;

            var auctionSchedule = auctionRepository.GetAuctionSchedule(int.Parse(seq), "detail_admin");
            var auctionWorkTypes = GetAuctionWorkTypeFromString(auctionSchedule.WorkType);
            foreach (var item in auctionWorkTypes)
            {
                item.LinkUrl = $"{Config.HomepageDomain}/Auction/{GetAucKindNameFromCode(auctionSchedule.AucKind)}/{aucNum}?type={item.Uid}";
            }

            return View(new AuctionScheduleViewModel
            {
                AuctionSchedule = auctionSchedule,
                AuctionWorkTypes = auctionWorkTypes
            });
        }

        #endregion

        #endregion

        #region # Auction > 낙찰자 변경이력 #

        #region [View]

        [HttpPost]
        [Route("/api/Auction/GetAuctionBidProcs")]
        public JObject GetAuctionBidProcs([FromBody] JObject json)
        {
            if (LoginInfo.UID > 0 && json != null)
            {
                json["mode"] = "admin";
                var data = auctionRepository.GetAuctionBidProcs(json);
                foreach (var item in data)
                {
                    item.MemName = string.IsNullOrWhiteSpace(item.MemName) ? string.Empty : StringHelper.GetPrivateInfoMask(item.MemName, "N");
                    item.OrgMemName = string.IsNullOrWhiteSpace(item.OrgMemName) ? string.Empty : StringHelper.GetPrivateInfoMask(item.OrgMemName, "N");
                    item.BeforeMemName = string.IsNullOrWhiteSpace(item.BeforeMemName) ? string.Empty : StringHelper.GetPrivateInfoMask(item.BeforeMemName, "N");
                    item.KofficeMemName = string.IsNullOrWhiteSpace(item.KofficeMemName) ? string.Empty : StringHelper.GetPrivateInfoMask(item.KofficeMemName, "N");
                }
                var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
                return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        public IActionResult BidHistory()
        {
            return View();
        }

        #endregion

        #endregion

        #region # Condition Report #

        public IActionResult ConditionReport()
        {
            return View();
        }

        #endregion
    }
}
