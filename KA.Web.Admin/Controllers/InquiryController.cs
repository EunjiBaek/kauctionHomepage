using KA.Entities.Helpers;
using KA.Entities.Models.Email;
using KA.Entities.Models.Member;
using KA.Repositories;
using KA.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    public class InquiryController : BaseController
    {
        #region # Constructor #

        public InquiryController(ICommonRepository commonRepository,
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

        #region # 문의 관리 #

        #region [WebApi]

        [Route("/api/Inquiry/GetList")]
        public JObject GetList([FromBody] JObject json)
        {
            json["mode"] = "admin";

            if (!string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "filter")) && string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "search")))
            {
                return JsonHelper.GetApiResult("80");
            }

            var data = memberRepository.GetMemberInquiries(json);
            foreach (var item in data)
            {
                item.IsKor = true;
                item.MemName = StringHelper.GetPrivateInfoMask(item.MemName, "N");
            }

            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Inquiry/Reply")]
        public JObject SetReply([FromBody] JObject json)
        {
            json["mode"] = "REPLY";
            json["reg_uid"] = LoginInfo.UID;
            json["reg_type"] = "A";

            var result = memberRepository.SetMemberInquiry(json);

            if (result.Result.Equals("00") && !string.IsNullOrWhiteSpace(result.Etc) && result.Target > 0)
            {
                var email = new Email()
                {
                    ToEmail = result.Etc,
                    ToName = result.Etc,
                    SubJect = "문의하신 내용에 대한 답변 메일입니다.",
                    Body = string.Format("{0}<br />\r\n{1}<br /><hr /><br />\r\n{2}<br />"
                                    , "안녕하세요. K옥션입니다."
                                    , "문의하신 내용에 대한 담당자 답변입니다."
                                    , json["contents"].ToString().Replace("\n", "<br />")),
                    IsBodyHtml = true,
                    Footer = "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                };
                email.Site = "A";
                email.Result = emailHelper.SendMail(email) ? "T" : "F";
                email.Type = "Inquiry";
                email.RegUid = LoginInfo.UID;
                logRepository.SetEmailLog(email);

                json["mode"] = "UPDATE_MAIL";
                json["uid"] = result.Target;
                json["mail_yn"] = email.Result;
                memberRepository.SetMemberInquiry(json);
            }

            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        [Route("/Inquiry")]
        public IActionResult List()
        {
            return View();
        }

        [Route("/Inquiry/{uid}")]
        public IActionResult Detail(string uid)
        {
            if (int.TryParse(uid, out int result) && LoginInfo.UID > 0)
            {
                // 조회 이력
                managerRepository.SetManagerViewHst(new JObject { ["mng_uid"] = LoginInfo.UID, ["type"] = "I", ["target"] = result.ToString() });

                MemberInquiry memberData = new();
                List<MemberInquiry> adminData = new();
                var data = memberRepository.GetMemberInquiries(new JObject()
                {
                    ["mode"] = "detail",
                    ["uid"] = result
                });
                foreach (var item in data)
                {
                    item.IsKor = true;

                    if (item.RegType.Equals("M"))
                    {
                        item.ImgFileUrl = GetImagePath(item.AucKind, item.AucNum, item.ImgFileName, true);
                        memberData = item;
                    }
                    else
                    {
                        adminData.Add(item);
                    }
                }

                ViewBag.MemberData = memberData;
                ViewBag.AdminData = adminData;

                return View();
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #endregion
    }
}
