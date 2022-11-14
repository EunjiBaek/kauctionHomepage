using KA.Entities.Models.Member;
using KA.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace KA.Web.Public.Controllers
{
    public class HowToController : BaseController
    {
        #region # Constructor #

        private readonly IMemberRepository memberRepository;

        public HowToController(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        #endregion

        #region # HowTo (신규 ver.2)

        [Route("/HowTo/AuctionIntroduction")]
        [Route("/HowTo/AuctionIntroduction/{menu}")]
        public IActionResult AuctionIntroduction(string menu = "")
        {
            ViewBag.SelectedMenu = menu;
            ViewData["Title"] = GetPageTitle("howto", "auctionintroduction");
            return View();
        }

        [Route("/HowTo/BidGuide")]
        [Route("/HowTo/BidGuide/{menu}")]
        public IActionResult BidGuide(string menu = "")
        {
            ViewBag.SelectedMenu = menu;
            ViewData["Title"] = GetPageTitle("howto", "bidguide");
            return View();
        }

        [Route("/HowTo/ConsignGuide")]
        [Route("/HowTo/ConsignGuide/{menu}")]
        public IActionResult ConsignGuide(string menu = "")
        {
            ViewBag.SelectedMenu = menu;
            ViewData["Title"] = GetPageTitle("howto", "consignguide");
            return View();
        }

        #endregion

        #region # Footer (약관) #

        public IActionResult MajorClause()
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
                return RedirectToAction("MajorClause");
            }

            var clauses = memberRepository.GetMemberClauses();
            var content = string.Empty;
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
                if (item.ClauseType != null && item.ClauseType.Equals("M"))
                {
                    content = item.DisplayContent;
                }
            }
            ViewBag.Content = content;
            ViewData["Title"] = GetPageTitle("howto", "major");
            return View();
        }

        public IActionResult OnlineClause()
        {
            var clauses = memberRepository.GetMemberClauses();
            var content = string.Empty;
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
                if (item.ClauseType != null && item.ClauseType.Equals("O"))
                {
                    content = item.DisplayContent;
                }
            }
            ViewBag.Content = content;
            ViewData["Title"] = GetPageTitle("howto", "online");
            return View();
        }

        [HttpGet]
        [Route("/HowTo/PrivacyClause")]
        public IActionResult PrivacyClause(string version = "")
        {
            if (string.IsNullOrWhiteSpace(version)) version = "0";
            if (!int.TryParse(version, out _)) return RedirectError();

            var clauses = memberRepository.GetTermAndConditionDetails(new JObject()
            {
                ["mode"] = "detail",
                ["type"] = "Private",
                ["version"] = version
            });
            var content = string.Empty;
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
                content = item.DisplayContent;
            }
            ViewBag.Content = content;
            ViewData["Title"] = GetPageTitle("howto", "privacy");
            return View();
        }

        [HttpGet]
        [Route("/HowTo/Clause/{type}")]
        [Route("/HowTo/Clause/{type}/{code}")]
        public IActionResult Clause(string type, string code, string version = "")
        {
            if (string.IsNullOrWhiteSpace(version)) version = "0";
            if (!int.TryParse(version, out _)) return RedirectError();

            var clauses = memberRepository.GetTermAndConditionDetails(new JObject()
            {
                ["mode"] = "detail",
                ["type"] = type,
                ["version"] = version,
                ["code"] = code
            });

            if (type.ToLower().Equals("join") && !string.IsNullOrWhiteSpace(code))
            {
                var data = clauses.Any() && clauses.Where(x => x.SubCode.Equals(code)).Any() ? clauses.Where(x => x.SubCode.Equals(code)).First() : new TermAndConditionDetail();
                data.IsKor = IsKor();
                ViewBag.Content = data.DisplayContent;
                ViewBag.CodeName = data.DisplayCodeName;
            }
            else
            {
                var data = clauses.Any() ? clauses.First() : new TermAndConditionDetail();
                data.IsKor = IsKor();
                ViewBag.Content = data.DisplayContent;
                ViewBag.CodeName = data.DisplayCodeName;
            }
            ViewData["Title"] = GetPageTitle("howto", "clause", type + code);
            return View();
        }

        #endregion
    }
}
