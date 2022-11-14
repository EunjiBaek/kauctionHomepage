using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Entities.Models.Main;
using KA.Repositories;
using KA.Web.Admin.Models;
using KA.Web.Admin.ViewModels.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    [Authorize]
    public class MainController : BaseController
    {
        #region # Constructor #

        public MainController(ICommonRepository commonRepository,
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

        #region # 메인 관리 > 메인 팝업배너 #

        #region [WebApi]

        [Route("/api/Main/GetNotices")]
        public JObject GetNotices([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json["mode"] = "admin";
            json["lang"] = "K";
            var data = mainRepository.GetNotices(json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Main/SetNotice")]
        public JObject SetNotice([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            var uid = JsonHelper.GetString(json, "uid", "0");
            json["mode"] = uid.Equals("0") ? "INSERT" : "UPDATE";
            json["reg_uid"] = LoginInfo.UID;

            // X, Y, Z: 메인 관리 컨텐츠 (1구간, 2구간, 3구간)
            var type = JsonHelper.GetString(json, "type");
            FileUploadResult fileResult;
            if ((type.Equals("L") || type.Equals("M") || type.Equals("V") || type.Equals("X") || type.Equals("Y") || type.Equals("Z")) && JsonHelper.GetString(json, "upload_image") != "")
            {
                // AWS 파일 업로드 처리
                fileResult = FileHelper.CopyTo(new FileUploadInfo()
                {
                    ServerType = Config.ContentMode,
                    Target = "Notice",
                    FilePath = FileHelper.GetFolderPath("Notice", Config.ContentPath),
                    FileNames = (new string[] { json["upload_image"].ToString() }).ToList(),
                    AccessKey = Config.AWS.AccessKey,
                    SecretKey = Config.AWS.Secretkey,
                    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                    IsKofficeCopy = false
                });

                if (fileResult.Result)
                {
                    var fileName = fileResult.FileNames.ToList()[0];
                    json["image"] = fileName[(fileName.LastIndexOf('/') + 1)..];
                    json.Remove("upload_image");
                }
            }
            else
            {
                fileResult = new FileUploadResult { Result = true };
            }

            if (fileResult.Result)
            {
                if (json["mode"].ToString() == "UPDATE" && (type.Equals("L") || type.Equals("M") || type.Equals("V") || type.Equals("X") || type.Equals("Y") || type.Equals("Z")) && json["image"].ToString() == "")
                {
                    json["image"] = json["image_original"].ToString();
                }

                json.Remove("image_original");

                var result = mainRepository.SetNotice(json);
                CheckErrorLog(result);
                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        [Route("/api/Main/DelNotice")]
        public JObject DelNotice([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json.Remove("image_original");
            json.Remove("upload_image");

            json["mode"] = "DELETE";
            var result = mainRepository.SetNotice(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View] 

        [Route("/Main/TopNotice")]
        public IActionResult TopNoticeList()
        {
            return View();
        }

        [Route("/Main/TopNotice/{Uid}")]
        public IActionResult TopNotice(string uid)
        {
            var notice = !string.IsNullOrWhiteSpace(uid) && !uid.Equals("0") ? mainRepository.GetNotice(uid) : new Notice();
            if (notice.Uid.Equals(0))
            {
                notice.StartDate = DateTime.Now;
                notice.EndDate = DateTime.Now.AddDays(7);
            }
            return View(new TopNoticeViewModel { Uid = uid, Notice = notice, Menus = commonRepository.GetMenus("TOP") });
        }

        #endregion

        #endregion

        #region # 메인 관리 > 메인 롤링배너 #

        #region [WebApi]

        [Route("/api/Main/GetBanners")]
        public JObject GetBanners([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json["mode"] = "admin";

            var data = mainRepository.GetMainBannerList(json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            foreach (var item in data)
            {
                item.ImageFilePath = Config.ImageDomain + "/www/Main/" + item.ImageFileName;
                item.MobileImageFilePath = !string.IsNullOrWhiteSpace(item.MobileImageFileName) ? Config.ImageDomain + "/www/Main/" + item.MobileImageFileName : "";
            }
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Main/SetBanner")]
        public JObject SetBanner([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            var uid = JsonHelper.GetString(json, "uid", "0");
            json["mode"] = uid.Equals("0") ? "INSERT" : "UPDATE";
            json["reg_uid"] = LoginInfo.UID;

            FileUploadResult fileResult;
            if (JsonHelper.GetString(json, "upload_image") != "")
            {
                // AWS 파일 업로드 처리
                fileResult = FileHelper.CopyTo(new FileUploadInfo()
                {
                    ServerType = Config.ContentMode,
                    Target = "Main",
                    FilePath = FileHelper.GetFolderPath("Main", Config.ContentPath),
                    FileNames = (new string[] { json["upload_image"].ToString() }).ToList(),
                    AccessKey = Config.AWS.AccessKey,
                    SecretKey = Config.AWS.Secretkey,
                    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                    IsKofficeCopy = false
                });

                if (fileResult.Result)
                {
                    var fileName = fileResult.FileNames.ToList()[0];
                    json["image_file_name"] = fileName[(fileName.LastIndexOf('/') + 1)..];
                    if (json["image_file_name"].Contains("\\"))
                    {
                        json["image_file_name"] = json["image_file_name"].ToString()[(json["image_file_name"].ToString().LastIndexOf("\\") + 1)..];
                    }
                    json.Remove("upload_image");
                }
            }
            else
            {
                fileResult = new FileUploadResult { Result = true };
            }

            if (fileResult.Result && JsonHelper.GetString(json, "upload_image_en") != "")
            {
                // AWS 파일 업로드 처리
                fileResult = FileHelper.CopyTo(new FileUploadInfo()
                {
                    ServerType = Config.ContentMode,
                    Target = "MainEn",
                    FilePath = FileHelper.GetFolderPath("Main", Config.ContentPath),
                    FileNames = (new string[] { json["upload_image_en"].ToString() }).ToList(),
                    AccessKey = Config.AWS.AccessKey,
                    SecretKey = Config.AWS.Secretkey,
                    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                    IsKofficeCopy = false
                });

                if (fileResult.Result)
                {
                    var fileName = fileResult.FileNames.ToList()[0];
                    json["image_file_name_en"] = fileName[(fileName.LastIndexOf('/') + 1)..];
                    if (json["image_file_name_en"].Contains("\\"))
                    {
                        json["image_file_name_en"] = json["image_file_name_en"].ToString()[(json["image_file_name_en"].ToString().LastIndexOf("\\") + 1)..];
                    }
                    json.Remove("upload_image_en");
                }
            }
            else
            {
                fileResult = new FileUploadResult { Result = true };
            }

            if (fileResult.Result && JsonHelper.GetString(json, "upload_mobile_image") != "")
            {
                // AWS 파일 업로드 처리
                fileResult = FileHelper.CopyTo(new FileUploadInfo()
                {
                    ServerType = Config.ContentMode,
                    Target = "MainMobile",
                    FilePath = FileHelper.GetFolderPath("Main", Config.ContentPath),
                    FileNames = (new string[] { json["upload_mobile_image"].ToString() }).ToList(),
                    AccessKey = Config.AWS.AccessKey,
                    SecretKey = Config.AWS.Secretkey,
                    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                    IsKofficeCopy = false
                });

                if (fileResult.Result)
                {
                    var fileName = fileResult.FileNames.ToList()[0];
                    json["mobile_image_file_name"] = fileName[(fileName.LastIndexOf('/') + 1)..];
                    if (json["mobile_image_file_name"].Contains("\\"))
                    {
                        json["mobile_image_file_name"] = json["mobile_image_file_name"].ToString()[(json["mobile_image_file_name"].ToString().LastIndexOf("\\") + 1)..];
                    }
                    json.Remove("upload_mobile_image");
                }
            }
            else
            {
                fileResult = new FileUploadResult { Result = true };
            }

            if (fileResult.Result && JsonHelper.GetString(json, "upload_mobile_image_en") != "")
            {
                // AWS 파일 업로드 처리
                fileResult = FileHelper.CopyTo(new FileUploadInfo()
                {
                    ServerType = Config.ContentMode,
                    Target = "MainMobileEn",
                    FilePath = FileHelper.GetFolderPath("Main", Config.ContentPath),
                    FileNames = (new string[] { json["upload_mobile_image_en"].ToString() }).ToList(),
                    AccessKey = Config.AWS.AccessKey,
                    SecretKey = Config.AWS.Secretkey,
                    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                    IsKofficeCopy = false
                });

                if (fileResult.Result)
                {
                    var fileName = fileResult.FileNames.ToList()[0];
                    json["mobile_image_file_name_en"] = fileName[(fileName.LastIndexOf('/') + 1)..];
                    if (json["mobile_image_file_name_en"].Contains("\\"))
                    {
                        json["mobile_image_file_name_en"] = json["mobile_image_file_name_en"].ToString()[(json["mobile_image_file_name_en"].ToString().LastIndexOf("\\") + 1)..];
                    }
                    json.Remove("upload_mobile_image_en");
                }
            }
            else
            {
                fileResult = new FileUploadResult { Result = true };
            }

            if (fileResult.Result)
            {
                if (json["mode"].ToString() == "UPDATE" && json["image_file_name"].ToString() == "")
                {
                    json["image_file_name"] = json["image_file_name_original"].ToString();
                }

                if (json["mode"].ToString() == "UPDATE" && json["image_file_name_en"].ToString() == "")
                {
                    json["image_file_name_en"] = json["image_file_name_en_original"].ToString();
                }

                if (json["mode"].ToString() == "UPDATE" && json["mobile_image_file_name"].ToString() == "")
                {
                    json["mobile_image_file_name"] = json["mobile_image_file_name_original"].ToString();
                }

                if (json["mode"].ToString() == "UPDATE" && json["mobile_image_file_name_en"].ToString() == "")
                {
                    json["mobile_image_file_name_en"] = json["mobile_image_file_name_en_original"].ToString();
                }

                var result = mainRepository.SetBanner(json);

                if (result.Result.Equals("00") && json["button_info"] != null)
                {
                    foreach (var item in json["button_info"])
                    {
                        var temp = JObject.FromObject(item);
                        temp["uid"] = json["uid"].ToString();
                        mainRepository.SetBannerButton(temp);
                    }
                }

                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        [Route("/api/Main/DelBanner")]
        public JObject DelBanner([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json.Remove("image_file_name_original");
            json.Remove("upload_image");

            json["mode"] = "DELETE";
            var result = mainRepository.SetBanner(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View] 

        [Route("/Main/Banner")]
        public IActionResult BannerList()
        {
            return View();
        }

        [Route("/Main/Banner/{Uid}")]
        public IActionResult Banner(string uid)
        {
            var banner = !string.IsNullOrWhiteSpace(uid) && !uid.Equals("0") ? mainRepository.GetBanner(uid) : new Banner();

            if (banner == null) return RedirectToAction("Error", "Home");

            if (banner.Uid.Equals(0))
            {
                banner.StartDate = DateTime.Now;
                banner.EndDate = DateTime.Now.AddDays(7);
            }
            return View(new BannerViewModel
            {
                Banner = banner,
                BannerButtons = mainRepository.GetBannerButtons("admin", int.TryParse(uid, out int result) ? result : 0)
            });
        }

        #endregion

        #endregion

        #region # 메인 관리 > 작품 하이라이트 #

        #region [WebAPI]

        [Route("/api/Main/GetWorkHighlight")]
        public JObject GetWorkHighlight([FromBody] JObject json)
        {
            var data = mainRepository.GetWorkHighlight("main");
            foreach (var item in data)
            {
                item.IsKor = true;
                item.ThumFileName = GetImagePath(item.AucKind, item.AucNum, item.ThumFileName);
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, 1));
        }

        [Route("/api/Main/GetAuctionSchedule")]
        public JObject GetAuctionSchedule()
        {
            return JsonHelper.GetApiResult("00", auctionRepository.GetAuctionSchedules("", "highlight"));
        }

        [Route("/api/Main/GetAuctionWork")]
        public JObject GetAuctionWork([FromBody] JObject json)
        {
            var data = auctionRepository.GetAuctionWorks(JsonHelper.GetString(json, "auc_kind"), int.Parse(JsonHelper.GetString(json, "auc_num", "0")), 0, json);
            foreach (var item in data)
            {
                item.IsKor = true;
                item.ThumFileName = GetImagePath(item.AucKind, item.AucNum, item.ThumFileName);
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            var page = JsonHelper.GetString(json, "page", "1");
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.TryParse(page, out int result) ? result : 1));
        }

        [Route("/api/Main/SetWorkHighlight")]
        public JObject SetWorkHighlight([FromBody] JObject json)
        {
            var result = mainRepository.SetWorkHighlight(JsonHelper.GetString(json, "mode"), int.Parse(JsonHelper.GetString(json, "work_uid", "0")));
            CheckErrorLog(result);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        public IActionResult WorkHighlight()
        {
            return View();
        }

        #endregion

        #endregion

        #region # 메인 관리 > 경매일정 #

        #region [WebAPI]

        [Route("/api/Main/GetAuctionSchedules")]
        public JObject GetAuctionSchedules([FromBody] JObject json)
        {
            var data = mainRepository.GetAuctionSchedules("admin", JsonHelper.GetString(json, "auc_kind"), json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Main/SetAuctionSchedule")]
        public JObject SetAuctionSchedule([FromBody] JObject json)
        {
            json["reg_uid"] = LoginInfo.UID;
            var result = mainRepository.SetAuctionSchedule(json);
            CheckErrorLog(result);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        public IActionResult AuctionSchedule()
        {
            return View();
        }

        #endregion

        #endregion

        #region # 메인 관리 > 외부 컨텐츠 #

        #region [WebApi]

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/Main/Content")]
        public string SetApiContent([FromBody] JObject json)
        {
            return "OK";
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/Main/ContentCode")]
        public JObject GetContentCode()
        {
            return JsonHelper.GetApiResult("00", commonRepository.GetCodeList("CRAWLING_DATA_TYPE"));
        }

        [Route("/api/Main/GetContents")]
        public JObject GetContents([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            json["mode"] = "admin";
            var data = mainRepository.GetCrawlingDatas(json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/Main/SetContent")]
        public JObject SetContent([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            var result = mainRepository.SetCrawlingData(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        public IActionResult Content()
        {
            ViewBag.Code = commonRepository.GetCodeList("CRAWLING_DATA_TYPE", "list", "K");
            return View();
        }

        #endregion

        #endregion

        #region # 메인 관리 > 관리 컨텐츠 #

        #region [View]

        [Route("/Main/ImageContent")]
        public IActionResult ImageContentList()
        {
            return View();
        }

        [Route("/Main/ImageContent/{Uid}")]
        public IActionResult ImageContent(string uid)
        {
            var notice = !string.IsNullOrWhiteSpace(uid) && !uid.Equals("0") ? mainRepository.GetNotice(uid) : new Notice();
            if (notice.Uid.Equals(0))
            {
                notice.StartDate = DateTime.Now;
                notice.EndDate = DateTime.Now.AddDays(7);
            }
            return View(new TopNoticeViewModel { Uid = uid, Notice = notice });
        }

        #endregion

        #endregion

        #region # 메인 관리 > 메뉴 이미지배너 #

        #region [View]

        [Route("/Main/MenuImageBanner")]
        public IActionResult MenuImageBannerList()
        {
            return View();
        }

        [Route("/Main/MenuImageBanner/{uid}")]
        public IActionResult MenuImageBanner(string uid)
        {
            var notice = !string.IsNullOrWhiteSpace(uid) && !uid.Equals("0") ? mainRepository.GetNotice(uid) : new Notice();
            if (notice.Uid.Equals(0))
            {
                notice.StartDate = DateTime.Now;
                notice.EndDate = DateTime.Now.AddDays(7);
            }
            return View(new TopNoticeViewModel { Uid = uid, Notice = notice });
        }

        #endregion

        #endregion

        #region # 히든 메뉴 > 템플릿 #

        public IActionResult Template()
        {
            return View();
        }

        #endregion
    }
}