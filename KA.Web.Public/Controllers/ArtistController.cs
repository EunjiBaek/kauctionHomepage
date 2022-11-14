using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KA.Web.Public.Controllers
{
    public class ArtistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

       
        [Route("/Artist/Detail/{artist_name}")]
        public IActionResult Detail(string artist_name = "")
        {
            
            ViewBag.aName = artist_name;
            return View();
        }
    }
}




