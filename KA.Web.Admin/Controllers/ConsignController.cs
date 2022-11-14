using KA.Entities.Helpers;
using KA.Entities.Models.Email;
using KA.Repositories;
using KA.Web.Admin.Models;
using KA.Web.Admin.ViewModels.Consign;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    public class ConsignController : BaseController
    {
        #region # Constructor #

        public ConsignController(ICommonRepository commonRepository,
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

        #region # 위탁 #

        #region [WebApi]

        [Route("/api/Consign/GetList")]
        public JObject GetConsignList([FromBody] JObject json)
        {
            json["mode"] = "admin";

            if (!string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "filter")) && string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "search")))
            {
                return JsonHelper.GetApiResult("80");
            }

            var data = memberRepository.GetMemberConsigns(json);
            foreach (var item in data)
            {
                item.IsKor = true;
                item.MemName = StringHelper.GetPrivateInfoMask(item.MemName, "N");
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Consign/DelConsign")]
        public JObject DelConsign([FromBody] JObject json)
        {
            if (json == null)
            {
                return JsonHelper.GetApiResult("90");
            }

            json["mode"] = "update_state";
            json["admin_uid"] = LoginInfo.UID;
            json["state"] = "999";

            var result = memberRepository.SetMemberConsign(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        [Route("/api/Consign/SetReview")]
        public JObject SetReview([FromBody] JObject json)
        {
            if (json == null)
            {
                return JsonHelper.GetApiResult("90");
            }

            json["mode"] = "update_review_info";
            json["admin_uid"] = LoginInfo.UID;

            var result = memberRepository.SetMemberConsign(json);

            if (result.Result.Equals("00") && JsonHelper.GetString(json, "chk_email").Equals("Y"))
            {
                // 담당자에게 알리기(메일발송)
                var code = commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "ConsignReview", ["uid"] = LoginInfo.UID });
                if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                {
                    var consignInfo = memberRepository.GetMemberConsign(int.Parse(json["uid"].ToString()));
                    var form = EmailForm.FormConsignAdminReview;
                    
                    // 신청인 정보
                    form = form.Replace("{CONSIGN_UID}", consignInfo.Uid.ToString());
                    form = form.Replace("{NAME}", StringHelper.GetPrivateInfoMask(consignInfo.MemName, "N"));
                    form = form.Replace("{UID}", " 홈페이지 (" + consignInfo.MemUid.ToString() + ") / 케이오피스 (" + consignInfo.KofficeMemUid.ToString() + ")");
                    form = form.Replace("{ADDRESS}", consignInfo.MemZipCode + " " + consignInfo.MemAddress + " " + consignInfo.MemAddress2);
                    form = form.Replace("{MOBILE}", StringHelper.GetPrivateInfoMask(consignInfo.MemMobile, "M"));
                    form = form.Replace("{EMAIL}", StringHelper.GetPrivateInfoMask(consignInfo.MemEmail, "E"));

                    // 검토상태
                    form = form.Replace("{RECEIPT_YN}", json["receipt_yn"].ToString().Equals("Y") ? "적합" : "부적합");
                    form = form.Replace("{RECOMMENDED_PRICE}", StringHelper.GetMoneyFormat(json["recommended_price"].ToString()) + " 원");
                    form = form.Replace("{MEMO}", json["memo"].ToString().Replace("\n", "<br />"));
                    form = form.Replace("{SERVICE_DOMAIN}", Config.ServiceDomain);

                    // 위탁작품 정보
                    form = form.Replace("{ARTIST}", consignInfo.Artist);
                    form = form.Replace("{TITLE}", consignInfo.Title);
                    form = form.Replace("{MATERIAL}", consignInfo.MaterialName);
                    form = form.Replace("{SIZE}", consignInfo.WorkX.ToString() + "x" + consignInfo.WorkY.ToString() + "x" + consignInfo.WorkZ.ToString() + " " + consignInfo.Ho.ToString() + "호");
                    form = form.Replace("{EDITION}", consignInfo.Edition);
                    form = form.Replace("{MAKEDATE}", consignInfo.MakeDate);
                    form = form.Replace("{DESC}", consignInfo.Desc);
                    form = form.Replace("{ETC}", consignInfo.Etc);
                    form = form.Replace("{PRICE_PURCHASE}", StringHelper.GetMoneyFormat(consignInfo.PricePurchase));
                    form = form.Replace("{PRICE_DESIRED}", StringHelper.GetMoneyFormat(consignInfo.PriceDesired));

                    var imgTag = string.Empty;
                    var filePath = Config.ImageDomain + "/www/Consign/" + consignInfo.RegDate.ToString("yyyy/MM/dd").Replace('-', '/');
                    var consignImages = memberRepository.GetMemberConsignImgs(int.Parse(json["uid"].ToString()));
                    if (consignImages.Any())
                    {
                        foreach (var imageName in consignImages)
                        {
                            imgTag += $"<div><img width='300' border='0' style='margin: 10px; display: block; width: 100%; max-width: 300px;' src='{filePath}/{imageName.ImgFileName}' /></div>";
                        }
                        form = form.Replace("{IMAGES}", imgTag);
                    }

                    var email = new Email()
                    {
                        AddToEmail = string.IsNullOrWhiteSpace(consignInfo.MngEmail) ? GetEmail(code.Extra2) : GetEmail(consignInfo.MngEmail),
                        AddToName = string.IsNullOrWhiteSpace(consignInfo.MngEmail) ? GetEmail(code.Extra2) : GetEmail(consignInfo.MngName, "N"),
                        AddCcEmail = string.IsNullOrWhiteSpace(consignInfo.MngEmail) ? Array.Empty<string>() : GetEmail(code.Extra2),
                        AddCcName = string.IsNullOrWhiteSpace(consignInfo.MngEmail) ? Array.Empty<string>() : GetEmail(code.Extra2, "N"),
                        SubJect = $"[홈페이지알림] {StringHelper.GetPrivateInfoMask(consignInfo.MemName, "N")}님의 위탁 신청에 대한 검토상태 메일입니다.",
                        Body = form,
                        IsBodyHtml = true,
                        Type = "Consign",
                        ServiceDomain = Config.ServiceDomain,
                        Footer = "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                    };
                    email.Site = "A";
                    email.Type = "Consign";
                    email.Result = emailHelper.SendMail(email) ? "T" : "F";
                    email.RegUid = LoginInfo.UID;
                    logRepository.SetEmailLog(email);
                }
            }

            return JsonHelper.GetApiResult(result.Result);
        }

        public string[] GetEmail(string emailInfo, string type = "")
        {
            if (string.IsNullOrWhiteSpace(emailInfo)) return null;

            if (emailInfo.Contains("|"))
            {
                try
                {
                    var list = new List<string>();
                    foreach (var item in emailInfo.Split(';'))
                    {
                        var name = item.Split('|')[0];
                        var email = item.Split('|')[1];
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            list.Add(type.ToUpper().Equals("N") ? name : email);
                        }
                    }
                    return list.ToArray();
                }
                catch (Exception) { return Array.Empty<string>(); }
            }
            else
            {
                return new string[] { emailInfo };
            }
        }

        #endregion

        #region [View]

        [Route("/Consign")]
        public IActionResult List([FromQuery] string p = "", [FromQuery] string ps = "", [FromQuery] string s = "")
        {
            if (!(LoginInfo.ContainAuth("MngConsign") || LoginInfo.ContainAuth("MngConsignInspection")))
            {
                return RedirectToAction("Error", "Home");
            }

            ViewBag.CodeList = commonRepository.GetCodeList("MEM_CONSIGN_STATE");
            ViewBag.PageSize = ps;
            ViewBag.Page = p;
            ViewBag.State = s;
            return View();
        }

        [Route("/Consign/{uid}")]
        public IActionResult Detail(string uid, [FromQuery] string p = "", [FromQuery] string ps = "", [FromQuery] string s = "")
        {
            if (!(LoginInfo.ContainAuth("MngConsign") || LoginInfo.ContainAuth("MngConsignInspection")))
            {
                return RedirectToAction("Error", "Home");
            }

            var consignInfo = memberRepository.GetMemberConsign(int.TryParse(uid, out int outUid) ? outUid : 0);
            var consignImgInfo = memberRepository.GetMemberConsignImgs(outUid);

            // 조회 이력
            if (consignInfo.Uid > 0)
            {
                managerRepository.SetManagerViewHst(new JObject { ["mng_uid"] = LoginInfo.UID, ["type"] = "C", ["target"] = consignInfo.Uid.ToString() });
            }

            if (!LoginInfo.ContainAuth("MngConsign") && LoginInfo.ContainAuth("MngConsignInspection"))
            {
                consignInfo.MemName = StringHelper.GetPrivateInfoMask(consignInfo.MemName, "N");
                consignInfo.MemAddress = StringHelper.GetPrivateInfoMask(consignInfo.MemZipCode + " " + consignInfo.MemAddress + " " + consignInfo.MemAddress2, "A");
                consignInfo.MemMobile = StringHelper.GetPrivateInfoMask(consignInfo.MemMobile, "M");
                consignInfo.MemEmail = StringHelper.GetPrivateInfoMask(consignInfo.MemEmail, "E");
            }
            else
            {
                consignInfo.MemAddress = consignInfo.MemZipCode + " " + consignInfo.MemAddress + " " + consignInfo.MemAddress2;
            }

            if (consignInfo.State.Equals("001"))
            {
                memberRepository.SetMemberConsign(new JObject()
                {
                    ["mode"] = "update_state",
                    ["state"] = "002",
                    ["uid"] = consignInfo.Uid,
                    ["admin_uid"] = LoginInfo.UID
                });
                consignInfo = memberRepository.GetMemberConsign(consignInfo.Uid);
            }
            return View(new ConsignViewModel()
            {
                MemberConsign = consignInfo,
                MemberConsignImg = consignImgInfo,
                Query = $"?ps={ps}&p={p}&s={s}"
            });
        }

        #endregion

        #endregion
    }
}