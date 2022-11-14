using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Entities.Models.Content;
using KA.Repositories;
using KA.Web.Admin.Models;
using KA.Web.Admin.ViewModels.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    [Authorize]
    public class ContentController : BaseController
    {
        #region # Constructor #

        public ContentController(ICommonRepository commonRepository,
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

        #region # Content #

        #region [WebApi]

        [Route("/api/Content/GetList/{board}")]
        public JObject GetContents(string board, [FromBody] JObject json)
        {
            json["mode"] = "admin_list";
            json["board_key"] = board;

            string page = JsonHelper.GetString(json, "page", "1");
            BoardInfo boardInfo = contentRepository.GetBoardInfo(board);
            List<BoardDoc> boardDocs = contentRepository.GetBoardDocs(json);
            int totalCount = boardDocs.Count > 0 ? boardDocs[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(boardInfo, boardDocs, totalCount, totalCount, int.TryParse(page, out int result) ? result : 1));
        }

        [HttpPost]
        [Route("/api/Content/SetContent")]
        public JObject SetContent([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            FileUploadResult fileResult;
            json["mem_seq"] = LoginInfo.UID;
            json["mem_id"] = LoginInfo.ID;
            json["mem_nick"] = string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "mem_nick")) ? LoginInfo.Name : json["mem_nick"].ToString();
            json["admin_seq"] = LoginInfo.UID;

            // 파일업로드 처리
            if (json["attach-file"] != null && json["attach-file"].Any())
            {
                json["filenames"] = string.Join(",", json["attach-file"]);
                fileResult = FileHelper.CopyTo(new FileUploadInfo()
                {
                    ServerType = Config.ContentMode,
                    Target = "Content",
                    FilePath = FileHelper.GetFolderPath("Content", Config.ContentPath),
                    FileNames = json["filenames"].ToString().Split(',').ToList(),
                    AccessKey = Config.AWS.AccessKey,
                    SecretKey = Config.AWS.Secretkey,
                    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                    IsKofficeCopy = false
                });
            } else
            {
                fileResult = new FileUploadResult() { Result = true };
            }

            if (fileResult.Result)
            {
                var uid = JsonHelper.GetString(json, "board_doc_uid", "0");
                json["mode"] = uid.Equals("0") ? "INSERT" : "UPDATE";

                var result = contentRepository.SetBoardDoc(json);
                return JsonHelper.GetApiResult(result);
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        #endregion

        #region [View]

        [Route("/Content/{boardKey}")]
        public IActionResult Index(string boardKey)
        {
            ViewBag.BoardKey = boardKey;
            return View();
        }

        [Route("/Content/{boardKey}/{uid}")]
        public IActionResult View(string boardKey, string uid)
        {
            ViewBag.BoardKey = boardKey;
            return View(contentRepository.GetBoardDoc(uid));
        }

        [Route("/Content/{boardKey}/Modify")]
        [Route("/Content/{boardKey}/Modify/{uid}")]
        public IActionResult Modify(string boardKey, string uid)
        {
            ViewBag.BoardKey = boardKey;
            if (int.TryParse(uid, out int result) && result > 0)
            {
                return View(new ContentViewModel
                { 
                    BoardDoc = contentRepository.GetBoardDoc(result),
                    BoardAttaches = contentRepository.GetFileUploads("admin", "C", "", 1, result.ToString())
                });
            }
            else
            {
                return View(new ContentViewModel
                {
                    BoardDoc = new BoardDoc()
                });
            }
        }

        #endregion

        #endregion

        #region # File Upload #

        #region [WebApi]

        [Route("/api/Content/GetFileUploadList")]
        public JObject GetFileUploadList([FromBody] JObject json)
        {
            string type = JsonHelper.GetString(json, "type");
            string search = JsonHelper.GetString(json, "search");
            int page = int.TryParse(JsonHelper.GetString(json, "page", "1"), out int result) ? result : 1;
            
            IEnumerable<FileUpload> fileUploads = contentRepository.GetFileUploads("admin", type, search, page);
            int totalCount = fileUploads.ToArray().Length > 0 ? fileUploads.ToArray()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, fileUploads, totalCount, totalCount, page));
        }

        [Route("/api/Content/SetFileUpload")]
        public JObject SetFileUpload([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            FileUploadResult fileResult;
            var mode = JsonHelper.GetString(json, "mode");
            var type = JsonHelper.GetString(json, "type");
            var uploadImage = JsonHelper.GetString(json, "upload_image");
            var explainText = JsonHelper.GetString(json, "explain_text");

            if (mode.Equals("INSERT"))
            {
                if (!string.IsNullOrWhiteSpace(uploadImage))
                {
                    // AWS 파일 업로드 처리
                    fileResult = FileHelper.CopyTo(new FileUploadInfo()
                    {
                        ServerType = Config.ContentMode,
                        Target = "imgFileUpload",
                        FilePath = FileHelper.GetFolderPath("imgFileUpload", Config.ContentPath),
                        FileNames = (new string[] { uploadImage }).ToList(),
                        AccessKey = Config.AWS.AccessKey,
                        SecretKey = Config.AWS.Secretkey,
                        BucketNameHomepage = Config.AWS.BucketNameHomepage,
                        BucketNameKoffice = Config.AWS.BucketNameKoffice,
                        IsKofficeCopy = false
                    });

                    if (fileResult.Result)
                    {
                        var fileName = fileResult.FileNames.ToList()[0];
                        json["name"] = fileName[(fileName.LastIndexOf('/') + 1)..];
                        json.Remove("upload_image");
                    }
                }
                else
                {
                    return JsonHelper.GetApiResult("90");
                }
            }
            else
            {
                fileResult = new FileUploadResult { Result = true };
            }

            if (fileResult.Result)
            {
                if (mode.Equals("INSERT"))
                {
                    json["path"] = $"/imgFileUpload/{DateTime.Now:yyyy_MM}";
                    json["fullpath"] = Config.AWS.BucketNameHomepage + json["path"].ToString() + "/" + json["name"].ToString();
                    json["url"] = Config.ImageDomain + "/www" + json["path"].ToString() + "/" + json["name"].ToString();
                }
                json["reg_uid"] = LoginInfo.UID;
                var result = contentRepository.SetFileUpload(json);
                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        #endregion

        #region [View]

        [Route("/Content/FileUpload")]
        public IActionResult FileUpload()
        {
            ViewBag.Code = commonRepository.GetCodeList("CON_FILE", "list", "K");
            return View();
        }

        #endregion

        #endregion
    }
}
