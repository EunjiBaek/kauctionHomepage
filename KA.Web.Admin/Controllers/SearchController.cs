using KA.Entities.Helpers;
using KA.Repositories;
using KA.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    public class SearchController : BaseController
    {
        #region # Constructor #

        public SearchController(ICommonRepository commonRepository,
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

        #region # 검색 관리 > 추천검색어 #

        #region [WebApi]

        [Route("/api/Search/GetSearchTerms")]
        public JObject GetSearchTerms([FromBody] JObject json)
        {
            json["mode"] = "admin";
            var data = mainRepository.GetSearchTerms(json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Search/SetSearchTerm")]
        public JObject SetSearchTerm([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json["reg_uid"] = LoginInfo.UID;
            var result = mainRepository.SetSearchTerm(json);
            CheckErrorLog(result);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        public IActionResult Term()
        {
            return View();
        }

        #endregion

        #endregion
    }
}
