using KA.Entities.Helpers;
using KA.Entities.Models.Content;
using KA.Repositories;
using KA.Web.Public.Models;
using KA.Web.Public.ViewModels.About;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace KA.Web.Public.Controllers
{
    public class AboutController : BaseController
    {
        #region # Constructor #

        private readonly IContentRepository contentRepository;

        public AboutController(IContentRepository contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        #endregion

        #region # Company #

        #region [View]

        public IActionResult Company()
        {
            ViewData["Title"] = GetPageTitle("about", "company");
            return View();
        }

        #endregion

        #endregion

        #region # 채용공고/언론보도/공지사항 #

        #region [WebApi]

        /// <summary>
        /// 게시글 콘텐츠 목록 조회 Api
        /// - page 정보는 QueryString Draw 값으로 전달
        /// </summary>
        /// <param name="board"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Route("api/Board/{board}/GetList")]
        public JObject GetList(string board, [FromQuery(Name = "draw")] string page = "1")
        {
            BoardInfo boardInfo = contentRepository.GetBoardInfo(board);

            List<BoardDoc> boardDocs = contentRepository.GetBoardDocs("list", board, page);

            int totalCount = boardDocs.Count > 0 ? boardDocs[0].TotalCount : 0;

            return JsonHelper.GetApiListResult(boardInfo, boardDocs, totalCount, totalCount);
        }

        [Route("api/Board/UpdateRead/{id}")]
        public JObject UpdateRead(string id)
        {
            return JsonHelper.GetApiResultLang(contentRepository.UpdateRead(id, HttpContext.Connection.RemoteIpAddress.ToString(), LoginInfo.Uid), IsKor());
        }

        #endregion

        #region [View]

        [HttpGet("[controller]/Press")]
        [HttpGet("[controller]/Notice")]
        [HttpGet("[controller]/Recruit")]
        public IActionResult ContentList()
        {
            var path = HttpContext.Request.Path.Value;
            ViewData["Board"] = path[(path.LastIndexOf('/') + 1)..];
            ViewData["Title"] = GetPageTitle("about", ViewData["Board"].ToString());
            return View();
        }

        [HttpGet("[controller]/Press/{id}")]
        [HttpGet("[controller]/Notice/{id}")]
        [HttpGet("[controller]/Recruit/{id}")]
        public IActionResult ContentView(string id)
        {     
            if (int.TryParse(id, out int result))
            {
                var path = HttpContext.Request.Path.Value;
                ViewData["Title"] = GetPageTitle("about", path.Split('/')[2]) + ":" + id;

                return View(new ContentViewModel
                {
                    BoardDoc = contentRepository.GetBoardDoc(result),
                    BoardAttaches = contentRepository.GetFileUploads("admin", "C", "", 1, result.ToString())
                });
            }
            else
            {
                return View(new BoardDoc());
            }
        }

        #endregion

        #endregion

        #region # Map #

        #region [View]

        public IActionResult Map()
        {
            ViewData["Title"] = GetPageTitle("about", "map");
            return View();
        }

        #endregion

        #endregion
    }
}