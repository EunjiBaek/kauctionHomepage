using Microsoft.AspNetCore.Mvc;

namespace KA.Web.Public.Controllers
{
    public class ServicesController : BaseController
    {
        #region # 기업컬렉션 #

        public IActionResult Corporation()
        {
            ViewData["Title"] = GetPageTitle("service", "corporation");
            return View();
        }

        #endregion

        #region # 케이옥션앤스트링 #

        public IActionResult KString()
        {
            ViewData["Title"] = GetPageTitle("service", "kstring");
            return View();
        }

        #endregion

        #region # 케이아트스페이스 #

        public IActionResult KArtSpace()
        {
            ViewData["Title"] = GetPageTitle("service", "kartspace");
            return View();
        }

        #endregion

        #region # 케이옥션 주얼리 #

        public IActionResult Jewelry()
        {
            return View();
        }

        #endregion

        #region # 프라이빗 세일 #

        public IActionResult PrivateSale()
        {
            ViewData["Title"] = GetPageTitle("service", "privatesale");
            return View();
        }

        #endregion
    }
}
