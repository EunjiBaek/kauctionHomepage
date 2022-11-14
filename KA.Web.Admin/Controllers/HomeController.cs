using KA.Entities.Helpers;
using KA.Entities.Models.Manager;
using KA.Repositories;
using KA.Web.Admin.Models;
using KA.Web.Admin.ViewModels.Member;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KA.Web.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ICommonRepository commonRepository,
            IMemberRepository memberRepository,
            IHttpContextAccessor httpContextAccessor,
            IManagerRepository managerRepository,
            IMainRepository mainRepository,
            IAuctionRepository auctionRepository,
            IContentRepository contentRepository,
            ILogRepository logRepository,
            EmailHelper emailHelper)
            : base(commonRepository, memberRepository, httpContextAccessor, managerRepository, mainRepository, auctionRepository, contentRepository, logRepository, emailHelper) { }

        public IActionResult Index()
        {
            return View();
        }

        #region # 로그인 (SSO) 처리 #

        [HttpGet]
        public IActionResult Login()
        {
            string token = httpContextAccessor.HttpContext.Request.Query["token"].ToString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                string uID = managerRepository.GetUIDByToken(token);

                if (!uID.Equals("-1"))
                {
                    Manager manager = managerRepository.GetManagerFromLogin(uID, HttpContext.Connection.RemoteIpAddress.ToString(), token);
                    if (manager != null && manager.UID > -1)
                    {
                        manager.Token = token;

                        SignIn(manager);

                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Logout");
            }

            // 관리자 로그인 페이지 활성화 여부 비교 후 처리
            return Config.AdminLogin != null && Config.AdminLogin.Equals("Y") ? View() : RedirectToAction("Logout");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ID))
            {
                ModelState.AddModelError("Message", "아이디를 입력하세요.");
            }
            if (string.IsNullOrWhiteSpace(model.Pw))
            {
                ModelState.AddModelError("Message", "비밀번호를 입력하세요.");
            }

            if (ModelState.IsValid)
            {
                Manager manager = managerRepository.GetManager(new Manager() { ID = model.ID, Pw = model.Pw, UID = -999 }, "login", HttpContext.Connection.RemoteIpAddress.ToString());
                if (manager != null && manager.UID > -1)
                {
                    SignIn(manager);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Message", "로그인에 실패하였습니다.");
                    return View("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        #endregion

        #region # 로그아웃 #

        public IActionResult Logout()
        {
            AdminSignOut();
            return View();
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
