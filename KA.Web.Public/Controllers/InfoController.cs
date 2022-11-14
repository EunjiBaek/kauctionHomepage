using KA.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace KA.Web.Public.Controllers
{
    public class InfoController : BaseController
    {
        #region # Constructor #

        private readonly IMemberRepository memberRepository;

        public InfoController(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        #endregion

        #region # 경매약관 #

        public IActionResult Clause()
        {
            return View();
        }

        #endregion

        #region # 온라인경매약관 #

        public IActionResult OnlineClause()
        {
            return View();
        }

        #endregion

        #region # 개인정보처리방침 #

        public IActionResult Privacy()
        {
            return View();
        }

        #endregion

        #region # 이메일무단수집거부 #

        public IActionResult RejectionEmail()
        {
            return View();
        }

        #endregion

        #region # 라이브응찰 신청 완료 메일 공지 #

        [HttpGet]
        public IActionResult AuctionImportantNotice(string version = "0")
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
                return RedirectToAction("AuctionImportantNotice");
            }

            if (string.IsNullOrWhiteSpace(version)) version = "0";
            if (!int.TryParse(version, out _)) return RedirectError();

            var clauses = memberRepository.GetTermAndConditionDetails(new JObject()
            {
                ["mode"] = "detail",
                ["type"] = "LiveBid",
                ["version"] = version
            });
            var content = string.Empty;
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
                content = item.DisplayContent;
            }
            ViewBag.Content = content;
            return View();
        }

        #endregion
    }
}
