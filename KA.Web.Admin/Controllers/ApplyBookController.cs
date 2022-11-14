using KA.Entities.Helpers;
using KA.Entities.Models.Email;
using KA.Repositories;
using KA.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    public class ApplyBookController : BaseController
    {
        #region # Constructor #

        public ApplyBookController(ICommonRepository commonRepository,
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

        #region # 도록 신청 #

        #region [WebApi]

        [Route("/api/ApplyBook/GetList")]
        public JObject GetApplyBookList([FromBody] JObject json)
        {
            json["mode"] = "admin";

            if (!string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "filter")) && string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "search")))
            {
                return JsonHelper.GetApiResult("80");
            }

            var data = memberRepository.GetMemberApplyBook(json);
            foreach (var item in data)
            {
                item.MemName = StringHelper.GetPrivateInfoMask(item.MemName, "N");
                item.MemMobile = StringHelper.GetPrivateInfoMask(item.MemMobile, "M");
                item.MemEmail = StringHelper.GetPrivateInfoMask(item.MemEmail, "E");
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/ApplyBook/SetProcess")]
        public JObject SetProcess([FromBody] JObject json)
        {
            if (json == null || LoginInfo.UID < 1) return JsonHelper.GetApiResult("ka.msg.common.error");

            json["mng_uid"] = LoginInfo.UID;
            var result = memberRepository.SetMemberApplyBook("process", 0, json);
            if (result.Equals("00"))
            {
                var applyBookInfo = memberRepository.GetMemberApplyBook(new JObject() { ["mode"] = "detail", ["uid"] = json["uid"].ToString() });

                if (applyBookInfo == null || !applyBookInfo.Any()) return JsonHelper.GetApiResult("ka.msg.common.error");

                var info = applyBookInfo.First();
                var isKor = !info.MemType.Equals("002") && !info.MemType.Equals("004");
                var addBodyHtml = (isKor ? EmailForm.FormApplyBookComplete : EmailForm.FormApplyBookCompleteEn).Replace("{APPLY_BOOK_ADDRESS}", info.MemZipCode + " " + info.MemAddress + " " + info.MemAddress2)
                    .Replace("{NAME}", info.MemName)
                    .Replace("{ID}", info.MemId)
                    .Replace("{REG_DATE}", info.RegDate.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Replace("{APPLY_DATE}", info.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Replace("{PERIOD}", info.ApplyDate.ToString("yyyy.MM") + " ~ " + info.ApplyDate.AddMonths(11).ToString("yyyy.MM"))
                    .Replace("_MAIL_SUBJECT_", isKor ? "도록 정기구독 안내" : "Notice on the Catalogue Subscription");

                var emailInfo = new Email()
                {
                    SubJect = isKor ? "[K-Auction] 도록 정기구독 안내" : "[K-Auction] Notice on the Catalogue Subscription",
                    Body = addBodyHtml,
                    IsBodyHtml = true,
                    AddToEmail = new string[] { info.MemEmail },
                    AddToName = new string[] { info.MemName },
                    Type = "Consign",
                    Footer = isKor ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889" : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                };
                emailInfo.Site = "A";
                emailInfo.Result = emailHelper.SendMail(emailInfo) ? "T" : "F";
                emailInfo.Type = "ApplyBookMember";
                emailInfo.RegUid = LoginInfo.UID;
                logRepository.SetEmailLog(emailInfo);

                return JsonHelper.GetApiResult(result);                
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.error");
            }
        }

        #endregion

        #region [View]
        [Route("/ApplyBook")]
        public IActionResult List()
        {
            return View();
        }
        #endregion

        #endregion
    }
}
