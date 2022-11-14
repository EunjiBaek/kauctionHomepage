using KA.Entities.Helpers;
using KA.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace KA.Web.Admin.Controllers
{
    public class TermAndCondition : BaseController
    {
        #region # Constructor #

        public TermAndCondition(ICommonRepository commonRepository,
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

        #region # 가입 약관 #

        [Route("/TermAndCondition/Join")]
        public IActionResult Join()
        {
            return View();
        }

        #endregion

        #region # 개인정보 처리방침 / 라이브응찰 유의사항 #

        [Route("/TermAndCondition/{type}")]
        public IActionResult Detail(string type = "")
        {
            ViewBag.Type = type;
            return View();
        }

        #endregion
    }
}
