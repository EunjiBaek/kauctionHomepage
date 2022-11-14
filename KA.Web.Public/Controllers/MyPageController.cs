using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Entities.Models.Common;
using KA.Entities.Models.Email;
using KA.Entities.Models.Member;
using KA.Repositories;
using KA.Web.Public.Models;
using KA.Web.Public.Services;
using KA.Web.Public.ViewModels.Auction;
using KA.Web.Public.ViewModels.MyPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;

namespace KA.Web.Public.Controllers
{
    [Authorize]
    public class MyPageController : BaseController
    {
        #region # Constructor #

        private readonly IAuctionRepository _auctionRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogRepository _logRepository;
        private readonly EmailHelper _emailHelper;
        private readonly CommonService _commonService;

        public MyPageController(IAuctionRepository auctionRepository,
            IMemberRepository memberRepository,
            ICommonRepository commonRepository,
            ILogRepository logRepository,
            EmailHelper emailHelper,
            CommonService commonService)
        {
            _auctionRepository = auctionRepository;
            _memberRepository = memberRepository;
            _commonRepository = commonRepository;
            _logRepository = logRepository;
            _emailHelper = emailHelper;
            _commonService = commonService;
        }

        #endregion

        #region # Api #

        /// <summary>
        /// My Page > 낙찰내역 - 목록 처리 함수
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("/api/MyPage/GetSuccessfulBidList")]
        public JObject GetSuccessfulBidList([FromBody] JObject json)
        {
            if (IsLogin())
            {
                json["mem_uid"] = LoginInfo.Uid;
                json["page_size"] = 10;
                json["mode"] = "successful_bid_list";
                var list = _auctionRepository.GetAuctionWorkByUserBid(json);
                foreach (var item in list)
                {
                    item.IsKor = IsKor();

                    // 케이오피스에서 출력완료한 경우 처리
                    if (!item.KofficePrintDate.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                    {
                        item.CertificatePrintDate = item.KofficePrintDate;
                        item.CertificatePrintYN = "Y";
                        item.CertificateYN = "N";
                    }
                }
                return JsonHelper.GetApiResult("00", list);
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        /// <summary>
        /// My Page > 응찰내역 - 경매 별 상세 응찰 정보 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route("/api/MyPage/GetBidDetailList")]
        public JObject GetBidDetailList([FromBody] JObject json)
        {
            if (IsLogin())
            {
                json["mem_uid"] = LoginInfo.Uid;
                var list = _auctionRepository.GetAuctionBidWork(json);
                foreach (var item in list)
                {
                    item.IsKor = IsKor();
                }
                return JsonHelper.GetApiResult("00", list);
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        /// <summary>
        /// My Page > 도록신청 - 신청처리 함수
        /// </summary>
        /// <returns></returns>
        [Route("/api/MyPage/SetApplyBook")]
        public JObject SetApplyBook([FromBody] JObject json)
        {
            if (IsLogin())
            {
                // 신청 or 변경 체크
                var member = _memberRepository.GetMember(LoginInfo.Uid);
                var forceNew = !string.IsNullOrWhiteSpace(member.ApplyBookKind) &&
                               member.ApplyBookRegDate <= DateTime.Now.AddYears(-1);
                var stateTitle = string.IsNullOrWhiteSpace(member.ApplyBookKind) || forceNew ? "신청" : " 주소변경";

                var result = _memberRepository.SetMemberApplyBook("REQUEST", LoginInfo.Uid, json, forceNew);
                if (result.Equals("00"))
                {
                    var mailReceiver = string.IsNullOrWhiteSpace(member.ApplyBookKind) ? "ApplyBookToManager" : "ApplyBookToManager2";
                    var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = mailReceiver, ["uid"] = LoginInfo.Uid });
                    if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                    {
                        var addBodyHtml = string.Empty;
                        var applyBookAddresses = _memberRepository.GetMemberAddresses("apply_book", LoginInfo.Uid);
                        if (applyBookAddresses.Any())
                        {
                            var applyBookAddress = applyBookAddresses.First();

                            addBodyHtml = EmailForm.FormApplyBook.Replace("{APPLY_BOOK_ADDRESS}", StringHelper.GetPrivateInfoMask(applyBookAddress.ZipCode + " " + applyBookAddress.Address + " " + applyBookAddress.Address2, "A"))
                                .Replace("{APPLY_BOOK_NAME}", StringHelper.GetPrivateInfoMask(applyBookAddress.ApplyBookReceiver, "N"))
                                .Replace("{APPLY_BOOK_MOBILE}", StringHelper.GetPrivateInfoMask(applyBookAddress.ApplyBookContact, "M"));

                            addBodyHtml = addBodyHtml.Replace("{NAME}", StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N"));
                            addBodyHtml = addBodyHtml.Replace("{UID}", " 홈페이지 (" + LoginInfo.Uid.ToString() + ") / 케이오피스 (" + LoginInfo.KofficeUid.ToString() + ")");
                            addBodyHtml = addBodyHtml.Replace("{ADDRESS}", StringHelper.GetPrivateInfoMask(LoginInfo.ZipCode + " " + LoginInfo.Address + " " + LoginInfo.Address2, "A"));
                            addBodyHtml = addBodyHtml.Replace("{MOBILE}", StringHelper.GetPrivateInfoMask(LoginInfo.Mobile, "M"));
                            addBodyHtml = addBodyHtml.Replace("{EMAIL}", StringHelper.GetPrivateInfoMask(LoginInfo.Email, "E"));

                            addBodyHtml = addBodyHtml.Replace("_MAIL_SUBJECT_", $"도록회원{stateTitle}");
                        }

                        var emailInfo = new Email
                        {
                            SubJect = $"[도록회원{stateTitle}] " + LoginInfo.ID + "(" + StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N") + $")님이 도록회원 {stateTitle}하셨습니다.",
                            Body = string.IsNullOrWhiteSpace(addBodyHtml) ? $"[도록회원{stateTitle}] " + LoginInfo.ID + "(" + StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N") + $")님이 도록회원 {stateTitle}하셨습니다." : addBodyHtml,
                            IsBodyHtml = true,
                            AddToEmail = GetEmail(code.Extra1),
                            AddToName = GetEmail(code.Extra1, "N"),
                            AddCcEmail = GetEmail(code.Extra2),
                            AddCcName = GetEmail(code.Extra2, "N"),
                            Type = "Consign",
                            Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                            Site = "P"
                        };
                        emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                        emailInfo.Type = "ApplyBook";
                        emailInfo.RegUid = LoginInfo.Uid;
                        _logRepository.SetEmailLog(emailInfo);
                    }
                }
                return JsonHelper.GetApiResult(result);
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        [Route("/api/MyPage/SetCertificateRequest")]
        public JObject SetCertificateRequest([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                json["mode"] = "INSERT";
                json["mem_uid"] = LoginInfo.Uid;
                var result = _memberRepository.SetMemberCertificateRequest(json);

                #region # 정상적으로 데이터 처리되었으면 메일 발송 #

                if (result.Result.Equals("00"))
                {
                    var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "MemCertRequest", ["uid"] = LoginInfo.Uid });
                    if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                    {
                        // 홍길동(ID: / 회원번호 : *) 님께서 맥유저로 보증서 직접 출력이 불가하여, 보증서 출력 신청되었으니 업무에 참고 바랍니다.
                        //
                        //- 신청사항 확인: 작품관리 > 보증서(홈페이지 요청)
                        //- 신청한 내용은 주 1회 작품관리팀에서 우편발송되며, 예외적인 경우 담당자가 기안 승인 받아 직접 출력할 수 있습니다.
                        StringBuilder emailBody = new();
                        emailBody.Append($"{StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N")}(ID: {StringHelper.GetPrivateInfoMask(LoginInfo.ID, "I")} / 회원번호 : {LoginInfo.Uid}) 님께서 맥유저로 보증서 직접 출력이 불가하여, 보증서 출력 신청되었으니 업무에 참고 바랍니다.<br /><br />");
                        emailBody.Append("- 신청사항 확인: 작품관리 > 보증서(홈페이지 요청)<br />");
                        emailBody.Append("- 신청한 내용은 주 1회 작품관리팀에서 우편발송되며, 예외적인 경우 담당자가 기안 승인 받아 직접 출력할 수 있습니다.");

                        var emailInfo = new Email()
                        {
                            AddToEmail = GetEmail(code.Extra1),
                            AddToName = GetEmail(code.Extra1, "N"),
                            AddCcEmail = GetEmail(code.Extra2),
                            AddCcName = GetEmail(code.Extra2, "N"),
                            SubJect = "[홈페이지알림] 보증서 출력 및 발송요청 접수 알림",
                            Body = emailBody.ToString(),
                            Type = "default",
                            IsBodyHtml = true,
                            Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                            : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                        };
                        emailInfo.Site = "P";
                        emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                        emailInfo.Type = "CertReq";
                        emailInfo.RegUid = LoginInfo.Uid;
                        _logRepository.SetEmailLog(emailInfo);
                    }

                    if (JsonHelper.GetString(json, "email_flag").Equals("Y"))
                    {
                        var member = _memberRepository.GetMember(LoginInfo.Uid);
                        SignIn(member, member.IsSaved == null ? "F" : member.IsSaved);
                    }
                }

                #endregion

                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        /// <summary>
        /// My Page > 낙찰내역 - 낙찰결과통보서 내용 가져오기
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("/api/MyPage/GetBidNotification")]
        public JObject GetBidNotification([FromBody] JObject json)
        {
            bool firstFlag = true;

            int TotalDeliveryFee = 0;

            decimal TotalBuyCommSum = 0;

            long TotalPriceSuccessfulBid = 0;

            AuctionNakViewModel model = new();

            model.AuctionNak = _auctionRepository.GetAuctionBidsNak(json["auc_kind"].ToString(), Convert.ToInt16(json["auc_num"]), LoginInfo.Uid);

            foreach (AuctionNak item in model.AuctionNak)
            {
                if (firstFlag)
                {
                    item.IsKor = IsKor();

                    if (IsKor())
                    {
                        model.ExpireDate = Convert.ToDateTime(item.AucDate).AddDays(7).ToString("MM월 dd일");
                    }
                    else
                    {
                        model.ExpireDate = Convert.ToDateTime(item.AucDate).AddDays(7).ToString("dd MMM", CultureInfo.CreateSpecificCulture("en-US"));
                    }

                    model.AucTitle = IsKor() ? item.AucTitle : string.IsNullOrWhiteSpace(item.AucTitleEn) ? item.AucTitle : item.AucTitleEn;
                    model.MailSendYN = item.MailSendYN;
                }

                item.DisplayDeliveryFee = (item.DeliveryFee > 0 ? TotalDeliveryFee > 0 ? 10000 : item.DeliveryFee : 0).ToString("#,##0");

                // 2021.10.05 :: 작품 2개 이상 낙찰 시 배송료 처리 수정 (두번째부터 10,000 계산)
                // TotalDeliveryFee += item.DeliveryFee;
                TotalDeliveryFee += item.DeliveryFee > 0 ? TotalDeliveryFee > 0 ? 10000 : item.DeliveryFee : 0;
                TotalBuyCommSum += item.BuyCommSum;
                TotalPriceSuccessfulBid += item.PriceSuccessfulBid;

                firstFlag = false;
            }

            model.TotalDeliveryFee = TotalDeliveryFee.ToString("#,##0");
            model.TotalBuyCommSum = TotalBuyCommSum.ToString("#,##0");
            model.TotalPriceSuccessfulBid = TotalPriceSuccessfulBid.ToString("#,##0");
            model.TotalPrice = (TotalPriceSuccessfulBid + TotalBuyCommSum).ToString("#,##0");
            model.TotalPriceFee = (TotalPriceSuccessfulBid + TotalBuyCommSum + TotalDeliveryFee).ToString("#,##0");

            return JsonHelper.GetApiResult("00", model);
        }

        /// <summary>
        /// My Page > 낙찰내역 - 보증서 생성 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("/api/MyPage/SuccessfulBidReport")]
        public JObject GetSuccessfulBidReport([FromBody] JObject json)
        {
            if (json == null || string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "uid"))) return JsonHelper.GetApiResult("90");

            var workUid = int.TryParse(JsonHelper.GetString(json, "uid"), out int _uid) ? _uid : 0;

            if (workUid < 1) return JsonHelper.GetApiResult("90");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                // 출력 여부 체크 (테스트 모드가 아닐경우)
                if (JsonHelper.GetString(json, "test_mode") != "Y")
                {
                    var printCheck = _memberRepository.SetMemberCertificateHistory("CHECK", new MemberCertificateHistory { WorkUid = workUid });
                    if (printCheck.Result.Equals("80"))
                    {
                        return JsonHelper.GetApiResult("80");
                    }
                }

                #region [이전 프로세스]

                //var userFileName = Guid.NewGuid().ToString();

                //// 템플릿양식 로드 -> 데이터매핑 -> 임시파일 저장 처리
                //var templatePath = JsonHelper.GetString(json, "test_mode") == "N" ? Config.ContentPath + "\\Report\\Template\\CommonWarranty.html" : Config.ContentPath + "\\Report\\Template\\CommonWarrantyTest.html";
                //if (System.IO.File.Exists(templatePath))
                //{
                //    // 템플릿 양식 로드
                //    var content = System.IO.File.ReadAllText(templatePath, Encoding.GetEncoding("euc-kr"));

                //    // 데이터매핑 (작품 정보)
                //    var auctionWork = _auctionRepository.GetAuctionWork(workUid);

                //    // 데이터매핑 (작품이미지)
                //    var AuctionWorkImages = _auctionRepository.GetAuctionWorkImages(workUid);
                //    foreach (var item in AuctionWorkImages)
                //    {
                //        item.ImgFileName = GetImageUrl(auctionWork.AucKind, auctionWork.AucNum, item.ImgFileName);
                //    }

                //    content = content.Replace("{WARRANTY_NO}", GetWarrantyNo(auctionWork, false));

                //    var imageTag = string.Empty;
                //    if (AuctionWorkImages.Where(x => x.ImgType.Equals("04")).Any())
                //    {
                //        var workImage = AuctionWorkImages.Where(x => x.ImgType.Equals("04")).First();
                //        var width = workImage.ImgW > workImage.ImgH && workImage.ImgW > 454 ? "width: 454px;" : "";
                //        var height = workImage.ImgH > workImage.ImgW && workImage.ImgH > 472 ? "height: 472px;" : "";

                //        imageTag = $"<img src='{workImage.ImgFileName}' style='{width} {height}' alt='' />";
                //    }
                //    content = content.Replace("{IMAGE_TAG}", imageTag);

                //    content = content.Replace("{SALE_DATE}", string.Format("{0}. {1}. {2}.", auctionWork.AucDate.ToString("yyyy"), auctionWork.AucDate.ToString("MM"), auctionWork.AucDate.ToString("dd")));
                //    content = content.Replace("{LOT_NO}", auctionWork.LotNum.ToString());

                //    var artistTag = string.Empty;
                //    if (!string.IsNullOrWhiteSpace(auctionWork.ArtistName))
                //    {
                //        //< dl class="author">
                //        //            <dt>
                //        //                오명희
                //        //                <span class="year">b.1956</span>
                //        //            </dt>
                //        //            <dd>
                //        //                <span>Oh MyungHee 吳明憙</span>
                //        //            </dd>
                //        //        </dl>
                //        artistTag = $"<dl class='author'><dt>{auctionWork.ArtistName}<span class='year'>{auctionWork.DirectDate}</span></dt><dd><span>{auctionWork.ArtistNameEn} {auctionWork.ArtistNameCn}</span></dd></dl>";

                //    }
                //    content = content.Replace("{ARTIST_TAG}", artistTag);

                //    var workNameTag = string.Empty;
                //    if (!string.IsNullOrWhiteSpace(auctionWork.Title))
                //    {
                //        //<p class="title">
                //        //    <span>
                //        //        절정 Zenith
                //        //    </span>
                //        //</p>
                //        workNameTag = $"<p class='title'><span>{auctionWork.Title} {auctionWork.TitleEn}</span></p>";
                //    }
                //    content = content.Replace("{WORK_NAME_TAG}", workNameTag);

                //    var descriptionTag = string.Empty;
                //    //<ul class="description">
                //    //    <li>
                //    //        <ul class="size">
                //    //            <li><span>55×55cm</span></li>
                //    //            <li><span>21.7×21.7inch</span></li>
                //    //        </ul>
                //    //    </li>
                //    //    <li>
                //    //        <span>pure silver leaf, pigment and color, ink, mother of pearl, lacquerware</span>
                //    //    </li>
                //    //    <li>
                //    //        <span>2021</span>
                //    //    </li>
                //    //</ul>
                //    descriptionTag += "<ul class='description'>";
                //    var auctionWorkKoffice = _auctionRepository.GetAuctionWorkKoffice(auctionWork.Uid, LoginInfo.Uid);
                //    if (auctionWorkKoffice != null)
                //    {
                //        if (!string.IsNullOrWhiteSpace(GetWorkSize(auctionWorkKoffice, "cm")))
                //        {
                //            var sizeCm = GetWorkSize(auctionWorkKoffice, "cm");
                //            var sizeInch = GetWorkSize(auctionWorkKoffice, "inch");

                //            sizeCm = !string.IsNullOrWhiteSpace(sizeCm) && !string.IsNullOrWhiteSpace(sizeInch) ? sizeCm + "," : sizeCm;
                //            sizeInch = !string.IsNullOrWhiteSpace(sizeCm) && !string.IsNullOrWhiteSpace(sizeInch) ? "<br />" + sizeInch : sizeInch;
                //            descriptionTag += $"<li><ul class='size'><li><span>{sizeCm}{sizeInch}</span></li></ul></li>";
                //        }                    

                //        content = content.Replace("{CARE_NUMBER}", !string.IsNullOrWhiteSpace(auctionWorkKoffice.CareNumber) ? auctionWorkKoffice.CareNumber : "");
                //    }
                //    else
                //    {
                //        content = content.Replace("{CARE_NUMBER}", "");
                //    }

                //    if (!string.IsNullOrWhiteSpace(auctionWork.MaterialEn))
                //    {
                //        descriptionTag += $"<li><span>{auctionWork.MaterialEn}</span></li>";
                //    }
                //    if (!string.IsNullOrWhiteSpace(auctionWork.MakeDate))
                //    {
                //        descriptionTag += $"<li><span>{auctionWork.MakeDate}</span></li>";
                //    }
                //    descriptionTag += "</ul>";
                //    content = content.Replace("{DESCRIPTION_TAG}", descriptionTag);

                //    // 임시파일 저장 처리
                //    if (!Directory.Exists(Config.ContentPath + "\\Report\\Document"))
                //    {
                //        Directory.CreateDirectory(Config.ContentPath + "\\Report\\Document");
                //    }
                //    System.IO.File.WriteAllText(Config.ContentPath + "\\Report\\Document\\" + userFileName + ".html", content, Encoding.GetEncoding("euc-kr"));

                //    // 조회 이력 처리
                //    var dbResult = _memberRepository.SetMemberCertificateHistory("INSERT", new MemberCertificateHistory
                //    {
                //        WorkUid = workUid,
                //        MemUid = LoginInfo.Uid,
                //        FileName = userFileName,
                //        TestMode = JsonHelper.GetString(json, "test_mode")
                //    });
                //    if (!dbResult.Result.Equals("00"))
                //    {
                //        return JsonHelper.GetApiResult("90");
                //    }
                //}
                //else
                //{
                //    return JsonHelper.GetApiResult("90");
                //}

                #endregion

                // 2022.04.25 :: 신규 프로세스 적용 (koffice 서비스 페이지 호출)
                try
                {
                    using var client = new WebClient();
                    string userFileName = client.DownloadString(Config.PageSaferApiUrl
                        .Replace("{OW_UID}", workUid.ToString())
                        .Replace("{MEM_UID}", LoginInfo.KofficeUid.ToString())
                        .Replace("{TEST_YN}", JsonHelper.GetString(json, "test_mode")));

                    if (string.IsNullOrWhiteSpace(userFileName)) throw new Exception();

                    return JsonHelper.GetApiResult("00", Config.PageSaferServiceUrl + "?mode=1&q=" + userFileName + (!IsKor() ? "&lang=2" : "&lang=1"));
                }
                catch (Exception) { return JsonHelper.GetApiResult("90"); }
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        #endregion

        #region # 응찰내역 #

        #region [WebApi]

        /// <summary>
        /// My Page > 응찰내역 - 목록 처리 함수
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("/api/MyPage/GetBidList")]
        public JObject GetBidList([FromBody] JObject json)
        {
            json["mem_uid"] = LoginInfo.Uid;
            json["page_size"] = 10;
            json["mode"] = "bid_list";
            var list = _auctionRepository.GetAuctionWorkByUserBid(json);
            var isKor = IsKor();
            var isLogin = IsLogin();
            foreach (var item in list)
            {
                item.IsKor = isKor;
                item.IsLogin = isLogin;
            }
            return JsonHelper.GetApiResult("00", list);
        }

        #endregion

        #region [View]

        public IActionResult Bid()
        {
            return View();
        }

        #endregion

        #endregion

        #region # 낙찰내역 #

        public IActionResult SuccessfulBid()
        {
            return View();
        }

        public IActionResult Bids()
        {
            return View();
        }

        public IActionResult SuccessfulBids()
        {
            return View("Bids");
        }
        
        [Route("/mypage/inquiries/consignments")]
        public IActionResult ConsignmentInquiries()
        {
            return View("Inquiries");
        }

        [Route("/mypage/inquiries/works")]
        public IActionResult WorkInquiries()
        {
            return View("Inquiries");
        }

        [AllowAnonymous]
        [Route("/Report/{token}")]
        [Route("/Office/{token}")]
        public IActionResult SuccessfulBidReport(string token = "")
        {
            ViewBag.Token = token;
            ViewBag.Auth = "N";
            ViewBag.Type = _commonService.GetPage.ToUpper().Contains("/OFFICE/") ? "Office" : "Report";
            ViewBag.DocumentName = "낙찰 결과 통보서";
            if (ViewBag.Type.ToString().Equals("Office") && !string.IsNullOrWhiteSpace(token))
            {
                var document = _memberRepository.GetMemberDocument(token);
                ViewBag.DocumentName = document.DocumentName;
                if (document != null && document.Uid > 0 && !string.IsNullOrWhiteSpace(document.SecurityYn) && document.SecurityYn.Equals("N"))
                {
                    ViewBag.Auth = "Y";
                    ViewBag.ReportData = document.DocumentHtml;
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/Report/{token}")]
        [Route("/Office/{token}")]
        public IActionResult SuccessfulBidReport(MemberNotiToken model)
        {
            ViewBag.Token = model.TokenVal;
            ViewBag.Auth = "N";
            ViewBag.Message = "정보가 일치하지 않습니다. 확인 후 다시 이용해 주세요.";
            ViewBag.Type = _commonService.GetPage.ToUpper().Contains("/OFFICE/") ? "Office" : "Report";

            // 사용자 입력값이 없는 경우
            if (string.IsNullOrWhiteSpace(model.UserVal))
            {
                ViewBag.Message = "생년월일 또는 사업자등록번호를 입력하세요.";
                return View();
            }

            // URL 토큰 값이 없는 경우
            if (string.IsNullOrWhiteSpace(model.TokenVal)) { return View(); }

            var userVal = model.UserVal;
            if (ViewBag.Type.ToString().Equals("Office"))
            {
                var document = _memberRepository.GetMemberDocument(model.TokenVal);

                // 토큰 정보가 없거나, 생년월일/사업자등록번호가 없는 경우
                if (document == null || string.IsNullOrWhiteSpace(document.SecurityCode))
                {
                    ViewBag.Message = "고객님은 생년월일 또는 사업자등록번호 정보를 제공하지 않아 확인이 불가합니다. 담당자를 통해 업데이트 후 이용하시기 바랍니다.";
                    return View();
                }

                if (userVal.Equals(document.SecurityCode) && document != null && document.Uid > 0)
                {
                    ViewBag.Auth = "Y";
                    ViewBag.ReportData = document.DocumentHtml;
                }
            }
            else
            {
                model = _memberRepository.GetMemberNotiToken(model.TokenVal);

                // 토큰 정보가 없거나, 생년월일/사업자등록번호가 없는 경우
                if (model == null || string.IsNullOrWhiteSpace(model.AuthVal))
                {
                    ViewBag.Message = "고객님은 생년월일 또는 사업자등록번호 정보를 제공하지 않아 확인이 불가합니다. 담당자를 통해 업데이트 후 이용하시기 바랍니다.";
                    return View();
                }

                // 등록값과 일치할 경우
                if (userVal.Equals(model.AuthVal))
                {
                    // 토큰에 대한 상세 정보가 없는 경우
                    if (string.IsNullOrWhiteSpace(model.AucKind) || model.AucNum < 1 || model.KoffMemUid < 1) { return View(); }
                    else
                    {
                        var reportData = _auctionRepository.GetAuctionReports(new JObject() { ["auc_kind"] = model.AucKind, ["auc_num"] = model.AucNum, ["mem_uid"] = model.KoffMemUid });
                        if (!reportData.Any()) { return View(); }
                        else
                        {
                            foreach (var item in reportData)
                            {
                                item.IsKor = IsKor();
                            }
                            ViewBag.Auth = "Y";
                            ViewBag.ReportData = reportData;
                            ViewBag.Message = "";

                            _memberRepository.SetMemberNotiRead(model.TokenVal);
                        }
                    }
                }
            }

            return View();
        }

        [Route("/MyPage/SuccessfulBidDocument/{AucKind}/{AucNum}")]
        public IActionResult SuccessfulBidDocument(string aucKind, string aucNum)
        {
            if (string.IsNullOrWhiteSpace(aucKind) || string.IsNullOrWhiteSpace(aucNum))
            {
                return RedirectError();
            }
            ViewBag.AucKind = aucKind;
            ViewBag.AucNum = aucNum;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/MyPage/SuccessfulBidDocument/Result")]
        public void SuccessfulBidDocumentResult()
        {

        }

        /// <summary>
        /// 보증서 출력 클릭 시 테스트 출력 완료 여부 체크 (버튼 상태 토글)
        /// </summary>
        /// <param name="json"></param>
        /// <returns>00:미출력,01:출력:99:오류</returns>
        [Route("/api/MyPage/CheckTestPrint")]
        public JObject CheckTestPrint([FromBody] JObject json)
        {
            if (json == null || LoginInfo.Uid < 1) return JsonHelper.GetApiResult("90");

            json["mode"] = "print_check";
            json["mem_uid"] = LoginInfo.Uid;
            json["test_yn"] = "Y";
            var data = _memberRepository.GetMemberCertificateHistories(json);
            
            return JsonHelper.GetApiResult(data.Any() ? "01" : "00");
        }

        /// <summary>
        /// [As-Is] 낙찰통보서 문자 URL 규칙 적용 (기존과 동일) 하여 낙찰 내역 페이지로 이동 처리
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <returns></returns>
        [Route("/mr/{aucKind}/{aucNum}")]
        public IActionResult RoutedMyResult(string aucKind, string aucNum)
        {
            if (string.IsNullOrWhiteSpace(aucKind) || (aucKind != "oa" && aucKind != "ko") || !int.TryParse(aucNum, out _))
            {
                return RedirectError();
            }

            return RedirectToAction("SuccessfulBid", "MyPage");
        }

        #endregion

        #region # 위탁내역 #

        #region [WebApi]

        [Route("/api/MyPage/ConsignList")]
        public JObject GetConsignList([FromBody] JObject json)
        {
            if (IsLogin())
            {
                json["mode"] = "list";
                json["mem_uid"] = LoginInfo.Uid;

                var data = _memberRepository.GetMemberConsigns(json);
                foreach (var item in data)
                {
                    item.IsKor = IsKor();
                }
                var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
                return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        #endregion

        #region [View]

        [Route("/MyPage/Consign")]
        public IActionResult Consign()
        {
            return View();
        }

        [Route("/MyPage/Consign/{uid}")]
        public IActionResult ConsignDetail(string uid)
        {
            if (int.TryParse(uid, out int result))
            {
                return View(new MemberConsignViewModel()
                {
                    MemberConsign = _memberRepository.GetMemberConsign(result),
                    MemberConsignImg = _memberRepository.GetMemberConsignImgs(result)
                });
            }
            else
            {
                return RedirectError();
            }
        }

        #endregion

        #endregion

        #region # 위탁신청 #

        #region [WebApi]

        [Authorize]
        [HttpPost]
        [Route("/api/MyPage/SetConsignRequest")]
        public JObject SetConsignRequest([FromBody] JObject json)
        {
            string rtnUid = string.Empty;

            try
            {
                if (LoginInfo.Uid < 1)
                {
                    throw new Exception("LoginInfo.Uid is 0");
                }

                var materialCode = JsonHelper.GetString(json, "material_code");
                var materialName = JsonHelper.GetString(json, "material_name");
                var materialEtc = JsonHelper.GetString(json, "material_etc");

                json["mode"] = "INSERT";
                json["mem_uid"] = LoginInfo.Uid;
                json["material_etc"] = materialCode.Equals("007") ? materialEtc : "";

                var result = _memberRepository.SetMemberConsign(json);
                rtnUid = result.Etc;

                if (result.Result.Equals("00"))
                {
                    if (json["file_info"] != null)
                    {
                        List<string> fileNames = new();
                        List<string> newFileNames = new();
                        var fileResult = new FileUploadResult();

                        foreach (var item in json["file_info"])
                        {
                            fileNames.Add(item["filename"].ToString().ToUpper().Contains(".HEIC") ? item["filename"].ToString().ToUpper().Replace(".HEIC", ".jpg") : item["filename"].ToString());
                            newFileNames.Add(item["target"] != null ? item["target"].ToString() : "F");
                        }

                        fileResult = FileHelper.CopyTo(new FileUploadInfo()
                        {
                            ServerType = Config.ContentMode,
                            Target = "Consign",
                            FilePath = FileHelper.GetFolderPath("Consign", Config.ContentPath),                            
                            FileNames = fileNames,
                            AccessKey = Config.AWS.AccessKey,
                            SecretKey = Config.AWS.Secretkey,
                            BucketNameHomepage = Config.AWS.BucketNameHomepage,
                            BucketNameKoffice = Config.AWS.BucketNameKoffice,
                            Etc = result.Etc,
                            Etc2 = newFileNames.Aggregate((x, y) => x + "|" + y),
                            IsKofficeCopy = false
                        });

                        fileNames.Clear();
                        newFileNames.Clear();

                        if (!fileResult.Result)
                        {
                            throw new Exception("File Upload Error - " + LoginInfo.Uid.ToString());
                        }
                        else
                        {
                            var imgResult = _memberRepository.SetMemberConsignImg(new JObject()
                            {
                                ["mode"] = "INSERT",
                                ["code"] = result.Etc,
                                ["filenames"] = string.Join('|', fileResult.FileNames)
                            });
                            if (imgResult.Result.Equals("00"))
                            {
                                #region # 위탁 신청 메일 발송 #

                                var form = EmailForm.FormConsign;
                                form = form.Replace("{NAME}", StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N"));
                                form = form.Replace("{UID}", " 홈페이지 (" + LoginInfo.Uid.ToString() + ") / 케이오피스 (" + LoginInfo.KofficeUid.ToString() + ")");
                                form = form.Replace("{ADDRESS}", LoginInfo.ZipCode + " " + LoginInfo.Address + " " + LoginInfo.Address2);
                                form = form.Replace("{MOBILE}", StringHelper.GetPrivateInfoMask(LoginInfo.Mobile, "M"));
                                form = form.Replace("{EMAIL}", StringHelper.GetPrivateInfoMask(LoginInfo.Email, "E"));

                                form = form.Replace("{ARTIST}", JsonHelper.GetString(json, "artist"));
                                form = form.Replace("{TITLE}", JsonHelper.GetString(json, "title"));
                                form = form.Replace("{MATERIAL}", materialCode.Equals("") ? "" : (materialCode.Equals("007") ? $"{materialName} ({materialEtc})" : materialName));

                                if (JsonHelper.GetString(json, "work_x") != "" || JsonHelper.GetString(json, "work_y") != "" || JsonHelper.GetString(json, "work_z") != "" || JsonHelper.GetString(json, "ho") != "")
                                {
                                    var workX = string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "work_x")) ? "(미입력)" : JsonHelper.GetString(json, "work_x");
                                    var workY = string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "work_y")) ? "(미입력)" : JsonHelper.GetString(json, "work_y");
                                    var workZ = string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "work_z")) ? "(미입력)" : JsonHelper.GetString(json, "work_z");
                                    var ho = string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "ho")) ? "(미입력)" : JsonHelper.GetString(json, "ho");
                                    form = form.Replace("{SIZE}", workX + "x" + workY + "x" + workZ + " " + ho + "호");
                                }
                                else
                                {
                                    form = form.Replace("{SIZE}", "");
                                }

                                form = form.Replace("{EDITION}", JsonHelper.GetString(json, "edition"));
                                form = form.Replace("{MAKEDATE}", JsonHelper.GetString(json, "make_date"));
                                form = form.Replace("{DESC}", JsonHelper.GetString(json, "desc"));
                                form = form.Replace("{ETC}", JsonHelper.GetString(json, "etc"));
                                form = form.Replace("{PRICE_PURCHASE}", JsonHelper.GetString(json, "price_purchase"));
                                form = form.Replace("{PRICE_DESIRED}", JsonHelper.GetString(json, "price_desired"));

                                var imgTag = string.Empty;
                                var filePath = Config.ImageDomain + "/www/Consign/" + DateTime.Now.ToString("yyyy/MM/dd").Replace('-', '/');
                                foreach (var imageName in fileResult.FileNames)
                                {
                                    imgTag += $"<div><img width='300' border='0' style='margin: 10px; display: block; width: 100%; max-width: 300px;' src='{filePath}/{imageName}' /></div>";
                                }
                                form = form.Replace("{IMAGES}", imgTag);

                                form = form.Replace("{SERVICE_DOMAIN}", Config.ServiceDomain);
                                // form = form.Replace("{CONSIGN_UID}", result.Etc);
                                form = form.Replace("{CONSIGN_UID}", DESCryptoHelper.DESEncrypt(result.Etc));

                                // As-Is : 이지현, 양승아, 김도형, 안동경, 고객관리팀
                                //       : 2021.04.13 임아름 과장 요청으로 고관팀만 설정 (By Teams)
                                var testMailMode = Config.MailTestModeFlag.Equals("Y") && !string.IsNullOrWhiteSpace(Config.MailTestModeAddress);
                                var email = new Email()
                                {
                                    ToEmail = testMailMode ? Config.MailTestModeAddress : "inbound-team@k-auction.com",
                                    ToName = testMailMode ? Config.MailTestModeAddress : "# 고객관리팀",
                                    SubJect = $"[홈페이지알림] {LoginInfo.ID} / {StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N")}님의 위탁 신청 메일입니다.",
                                    Body = form,
                                    IsBodyHtml = true,
                                    Type = "Consign",
                                    ServiceDomain = Config.ServiceDomain,
                                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                                };
                                email.Site = "P";
                                email.Result = _emailHelper.SendMail(email) ? "T" : "F";
                                email.Type = "ConsignRequest";
                                email.RegUid = LoginInfo.Uid;
                                _logRepository.SetEmailLog(email);

                                #endregion
                            }
                            else
                            {
                                throw new Exception("SetMemberConsignImg error");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("file info is null");
                    }
                }
                else
                {
                    _commonService.CheckErrorLog(result);
                }

                // 임시 업로드 파일 삭제 처리
                if (Config.ContentMode.Equals("AWS"))
                {
                    DirectoryInfo directory = new(FileHelper.GetFolderPath("Consign", Config.ContentPath));
                    if (directory.Exists)
                    {
                        directory.Delete(true);
                    }
                }

                return JsonHelper.GetApiResult(result.Result);
            }
            catch (Exception Ex)
            {
                if (!string.IsNullOrWhiteSpace(rtnUid))
                {
                    DeleteMemberConsign(rtnUid);
                }
                _logRepository.SetErrorLog("MYPAGE", "CONSIGNREQUEST", LoginInfo.Uid, Ex);
                return JsonHelper.GetApiResult("90");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/api/MyPage/DelConsign")]
        public JObject DelConsign([FromBody] JObject json)
        {
            if (json == null)
            {
                return JsonHelper.GetApiResult("90");
            }

            json["mode"] = "update_state";
            json["admin_uid"] = LoginInfo.Uid;
            json["state"] = "998";

            var result = _memberRepository.SetMemberConsign(json);
            return JsonHelper.GetApiResult(result.Result);
        }


        private DbResult DeleteMemberConsign(string code)
        {
            return _memberRepository.SetMemberConsign(new JObject()
            {
                ["mode"] = "DELETE",
                ["code"] = code
            });
        }

        #endregion

        #region [View]

        public IActionResult ConsignRequest()
        {
            ViewData["Title"] = GetPageTitle("mypage", "consignrequest");
            return View();
        }

        #endregion

        #endregion

        #region # 도록신청 #

        #region [View]

        public IActionResult ApplyBook()
        {
            var member = _memberRepository.GetMember(LoginInfo.Uid);
            ViewBag.ApplyKind = member.ApplyBookKind;
            ViewBag.ApplyBookRegDate = member.ApplyBookRegDate.ToString("O");
            
            // [#520/ISMS] 도록 신청 시 주소 제 3자제공 동의 화면 구현
            //   - 주소 정보가 있는지 체크하여 팝업 유형 처리
            var memberAddress = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid);
            ViewBag.AddressFlag = memberAddress.Any();

            return View();
        }

        #endregion

        #endregion

        #region # 문의내역 #

        #region [WebApi]

        [Route("/api/MyPage/GetInquiry")]
        public JObject GetInquiry([FromBody] JObject json)
        {
            if (IsLogin())
            {
                json["reg_uid"] = LoginInfo.Uid;
                json["reg_type"] = "M";

                var data = _memberRepository.GetMemberInquiries(json);
                foreach (var item in data)
                {
                    item.IsKor = IsKor();
                }
                var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
                return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        #endregion

        [Authorize]
        public IActionResult Inquiry()
        {
            return View();
        }

        #endregion

        #region # 마이페이지 - 회원정보관리 #

        #region [WebApi]

        [Route("/api/MyPage/SetMember")]
        public JObject SetMember([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                var fileResult = true;
                var member = _memberRepository.GetMember(LoginInfo.Uid);

                // 증빙서류부터 처리
                if (member.Type.Equals("002"))
                {
                    #region # 해외개인 - 증빙서류 처리 #

                    var identification = JsonHelper.GetString(json, "identification");

                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        member.Identification = identification;
                        member.InfoReqFile = "Y";

                        List<string> fileNames = new();
                        fileNames.Add("\\Temp\\Member\\Join\\" + LoginInfo.ID + "\\identification\\" + identification);

                        fileResult = FileHelper.CopyTo(new FileUploadInfo()
                        {
                            ServerType = Config.ContentMode,
                            Target = "Join",
                            FilePath = Config.ContentPath,
                            FileNames = fileNames,
                            AccessKey = Config.AWS.AccessKey,
                            AccessKeyPrivate = Config.AWS.AccessKeyPrivate,
                            SecretKey = Config.AWS.Secretkey,
                            SecretKeyPrivate = Config.AWS.SecretkeyPrivate,
                            BucketNameHomepage = Config.AWS.BucketNameKoffice,
                            BucketNameHomepatePrivate = Config.AWS.BucketNameKofficePrivate,
                            Etc = LoginInfo.KofficeUid.ToString(),
                            IsKofficeCopy = false
                        }).Result;
                    }

                    #endregion
                }
                else if (member.Type.Equals("003") || member.Type.Equals("004"))
                {
                    #region # 국내법인/해외법인 - 사업자등록증/명함 처리 #

                    var companyRegDoc = JsonHelper.GetString(json, "company_reg_doc");

                    if (!string.IsNullOrWhiteSpace(companyRegDoc))
                    {
                        member.CompanyRegDoc = companyRegDoc;
                        member.InfoReqFile = "Y";

                        List<string> fileNames = new();
                        fileNames.Add("\\Temp\\Member\\Join\\" + LoginInfo.ID + "\\memBusinessLicenseDoc\\" + companyRegDoc);

                        fileResult = FileHelper.CopyTo(new FileUploadInfo()
                        {
                            ServerType = Config.ContentMode,
                            Target = "Join",
                            FilePath = Config.ContentPath,
                            FileNames = fileNames,
                            AccessKey = Config.AWS.AccessKey,
                            SecretKey = Config.AWS.Secretkey,
                            BucketNameHomepage = Config.AWS.BucketNameHomepage,
                            BucketNameKoffice = Config.AWS.BucketNameKoffice,
                            Etc = LoginInfo.Uid.ToString(),
                            Etc2 = LoginInfo.KofficeUid.ToString(),
                            IsKofficeCopy = true
                        }).Result;
                    }

                    if (fileResult)
                    {
                        var businessCard = JsonHelper.GetString(json, "company_business_card");

                        if (!string.IsNullOrWhiteSpace(businessCard))
                        {
                            member.CompanyBusinessCard = businessCard;

                            List<string> fileNames = new();
                            fileNames.Add("\\Temp\\Member\\Join\\" + LoginInfo.ID + "\\memBusinessCard\\" + businessCard);

                            fileResult = FileHelper.CopyTo(new FileUploadInfo()
                            {
                                ServerType = Config.ContentMode,
                                Target = "Join",
                                FilePath = Config.ContentPath,
                                FileNames = fileNames,
                                AccessKey = Config.AWS.AccessKey,
                                SecretKey = Config.AWS.Secretkey,
                                BucketNameHomepage = Config.AWS.BucketNameHomepage,
                                BucketNameKoffice = Config.AWS.BucketNameKoffice,
                                Etc = LoginInfo.Uid.ToString(),
                                Etc2 = LoginInfo.KofficeUid.ToString(),
                                IsKofficeCopy = true
                            }).Result;
                        }
                    }

                    #endregion
                }

                // 파일업로드 처리 성공 후 DB 처리
                if (fileResult)
                {
                    // 기본정보
                    member.Name = JsonHelper.GetString(json, "name");
                    member.Sex = JsonHelper.GetString(json, "sex");
                    member.BirthDate = JsonHelper.GetString(json, "birth_date");
                    member.Email = JsonHelper.GetString(json, "email");

                    // 인증정보
                    member.MobileAuthSeq = JsonHelper.GetString(json, "mobile_seq");

                    // 부가정보
                    member.JobCode = JsonHelper.GetString(json, "job_code");
                    member.CompanyName = JsonHelper.GetString(json, "company_name");
                    member.CompanyTel = JsonHelper.GetString(json, "company_tel");
                    member.CompanyFax = JsonHelper.GetString(json, "company_fax");

                    member.CompanyRegNo = JsonHelper.GetString(json, "company_reg_no");
                    member.CompanyPresident = JsonHelper.GetString(json, "company_president");
                    member.CompanyRepTel = JsonHelper.GetString(json, "company_rep_tel");
                    member.TaxEmail = JsonHelper.GetString(json, "tax_email");

                    var dbResult = _memberRepository.SetMember(member, "UPDATE_INFO");
                    _commonService.CheckErrorLog(dbResult);
                    dbResult.Result = (dbResult.RsltCD != null && dbResult.RsltCD.Equals("00")) || (dbResult.Result != null && dbResult.Result.Contains("OK|")) ? "00" : (dbResult.RsltCD ?? dbResult.Result);
                    if (dbResult.Result.Equals("00"))
                    {
                        // TODO: 메일발송 처리


                        return JsonHelper.GetApiResult(dbResult.Result);
                    }
                    else
                    {
                        return JsonHelper.GetApiResult("ka.msg.common.error");
                    }
                }
                else
                {
                    return JsonHelper.GetApiResult("ka.msg.common.error");
                }
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        [Route("/api/MyPage/CheckPassword")]
        public JObject CheckPassword([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                var member = _memberRepository.GetMember(LoginInfo.Uid);
                var passwordOld = _commonRepository.GetEncrypt(JsonHelper.GetString(json, "password_old"));
                return JsonHelper.GetApiResult(member.Pwd.Equals(passwordOld) ? "00" : "ka.msg.mypage.passwordOldNotMatch");
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        [Route("/api/MyPage/GetMemberInterestArtists")]
        public JObject GetMemberInterestArtists([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                json["mode"] = JsonHelper.GetString(json, "filter").Equals("F") ? "MEM_ARTIST" : "MEM_SEARCH";
                json["mem_uid"] = LoginInfo.Uid;

                if (!string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "sort")))
                {
                    var sortMode = JsonHelper.GetString(json, "sort");
                    if (sortMode.Equals("S1"))
                    {
                        json["sort_option"] = "DESC";
                        json["sort_column"] = "UID";
                    }
                    else if (sortMode.Equals("S2"))
                    {
                        json["sort_option"] = "ASC";
                        json["sort_column"] = IsKor() ? "NAME" : "NAME_EN";
                    }
                    else if (sortMode.Equals("S3"))
                    {
                        json["sort_option"] = "DESC";
                        json["sort_column"] = IsKor() ? "NAME" : "NAME_EN";
                    }
                }
                else
                {
                    json["sort_option"] = "DESC";
                    json["sort_column"] = "UID";
                }

                var list = _memberRepository.GetMemberInterestArtists(json);
                foreach (var item in list)
                {
                    item.IsKor = IsKor();
                    item.ArtistFileUrl = string.IsNullOrWhiteSpace(item.FileName) ? "" : $"{Config.ImageDomainKoffice}/images/Artist/Image/{item.FileName}";
                    item.ArtistDesc = HttpUtility.HtmlDecode(item.IsKor ? StringHelper.RemoveTag(item.Etc) : StringHelper.RemoveTag(item.EtcEn));

                    if (!string.IsNullOrWhiteSpace(item.AucWorkInfo))
                    {
                        string[] temp = item.AucWorkInfo.Split('|');
                        var aucInfoList = new List<string>();
                        var aucKind = "";
                        var aucNum = "";
                        var workUid = "";

                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (aucKind.Equals(temp[i].Split('^')[1]) && aucNum.Equals(temp[i].Split('^')[2]) && workUid.Equals(temp[i].Split('^')[3])) continue;

                            var aucWorkImgUrl = GetListImageUrl(temp[i].Split('^')[1], int.Parse(temp[i].Split('^')[2]), temp[i].Split('^')[0]);
                            if (temp[i].Split('^')[9].Equals("P"))
                            {
                                aucWorkImgUrl = aucWorkImgUrl.Replace("_L.jpg", ".jpg");
                            }

                            var aucWorkLinkUrl = $"/Auction/{GetAucKindNameFromCode(temp[i].Split('^')[1])}/{temp[i].Split('^')[2]}/{temp[i].Split('^')[3]}";
                            aucInfoList.Add(string.Format("{0}^{1}^{2}^{3}^{4}^{5}"
                                , aucWorkImgUrl
                                , aucWorkLinkUrl
                                , !IsKor() ? temp[i].Split('^')[5] : temp[i].Split('^')[4]
                                , !IsKor() ? temp[i].Split('^')[7] : temp[i].Split('^')[6]
                                , StringHelper.GetMoneyFormat(temp[i].Split('^')[8])
                                , temp[i].Split('^')[9]));

                            aucKind = temp[i].Split('^')[1];
                            aucNum = temp[i].Split('^')[2];
                            workUid = temp[i].Split('^')[3];
                        }
                        item.AucWorkInfo = string.Join('|', aucInfoList);
                    }
                }

                var totalCount = list.ToList().Count > 0 ? list.ToList()[0].TotalCount : 0;
                return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, list, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        [Route("/api/MyPage/SetMemberInterestInfo")]
        public JObject SetMemberInterestInfo([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                json["uid"] = LoginInfo.Uid;

                var dbResult = _memberRepository.SetMemberInterestInfo(json);
                _commonService.CheckErrorLog(dbResult);
                return JsonHelper.GetApiResult(dbResult.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        [Route("/api/MyPage/SetAccount")]
        public JObject SetAccount([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                // 현재 정보 처리
                var member = _memberRepository.GetMember(LoginInfo.Uid);

                var oldInfoValidatePeriod = member.InfoValidatePeriod;
                var oldMainHighlight = string.IsNullOrWhiteSpace(member.MainHighlight) ? "" : member.MainHighlight;
                var oldListViewMode = string.IsNullOrWhiteSpace(member.ListViewMode) ? "" : member.ListViewMode;
                var oldReceiveSmsInfo = string.IsNullOrWhiteSpace(member.ReceiveSmsInfo) ? "" : member.ReceiveSmsInfo;
                var oldReceiveEmailInfo = string.IsNullOrWhiteSpace(member.ReceiveEmailInfo) ? "" : member.ReceiveEmailInfo;
                var oldReceivePhoneInfo = string.IsNullOrWhiteSpace(member.ReceivePhoneInfo) ? "" : member.ReceivePhoneInfo;

                // 신규 정보 처리
                member.InfoValidatePeriod = JsonHelper.GetString(json, "info_validate_period");
                member.MainHighlight = JsonHelper.GetString(json, "main_highlight");
                member.ListViewMode = JsonHelper.GetString(json, "list_view_mode");
                member.ReceiveSmsInfo = JsonHelper.GetString(json, "receive_sms_info");
                member.ReceiveEmailInfo = JsonHelper.GetString(json, "receive_email_info");
                member.ReceivePhoneInfo = JsonHelper.GetString(json, "receive_phone_info");

                var dbResult = _memberRepository.SetMember(member, "UPDATE_ACCOUNT");
                _commonService.CheckErrorLog(dbResult);
                dbResult.Result = dbResult.Result.Contains("OK|") ? "00" : dbResult.Result;
                if (dbResult.Result.Equals("00"))
                {
                    SignIn(member);
                    return JsonHelper.GetApiResult(dbResult.Result);
                }
                else
                {
                    return JsonHelper.GetApiResult("ka.msg.common.error");
                }
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        [Route("/api/MyPage/GetMobileValidationCode")]
        public JObject GetMobileValidationCode([FromBody] JObject json)
        {
            if (IsLogin() && LoginInfo.Uid > 0)
            {
                var authSeq = Guid.NewGuid().ToString();
                var authCode = StringHelper.GetRandomString("num", 6);
                string result = _memberRepository.SetMemberMobileAuth(new MemberMobileAuth()
                {
                    Seq = authSeq,
                    State = "S",
                    Auth = "M",
                    Type = "M",
                    Type2 = "Modify",
                    Device = IsMobile() ? "M" : "W",
                    Message = authCode,
                    MemUid = LoginInfo.Uid,
                    MobileCode = JsonHelper.GetString(json, "dial_code"),
                    MobileNo = JsonHelper.GetString(json, "number")
                }, "MODIFY_REQUEST"); ;

                var data = _commonService.GetHost.ToLower().Contains("localhost") || _commonService.GetHost.ToLower().Contains(".dev.") || _commonService.GetHost.ToLower().Contains(".release.") ? authCode : string.Empty;
                return JsonHelper.GetApiResult(result, authSeq, data);
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        [Route("/api/MyPage/SetMobileValidationCode")]
        public JObject SetMobileValidationCode([FromBody] JObject json)
        {
            if (IsLogin() && LoginInfo.Uid > 0)
            {
                var authSeq = JsonHelper.GetString(json, "auth_seq");
                var authCode = JsonHelper.GetString(json, "auth_code");
                string result = _memberRepository.SetMemberMobileAuth(new MemberMobileAuth()
                {
                    Seq = authSeq,
                    Message = authCode,
                    MemUid = LoginInfo.Uid
                }, "MODIFY_CONFIRM"); ;
                return JsonHelper.GetApiResult(result);
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.expired");
            }
        }

        #endregion

        #region [View]

        [Authorize]
        [Route("/MyPage/Member")]
        [Route("/MyPage/Info")]
        [Route("/MyPage/Artist")]
        [Route("/MyPage/Account")]
        public IActionResult Member()
        {
            var addressList = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid);
            foreach (var item in addressList)
            {
                item.IsKor = IsKor();
            }

            var clauses = _memberRepository.GetMemberClauses();
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
            }

            var memCodeList = _commonService.GetMainCodeList("MEM_COUNTRY|MEM_JOB");

            // 국내개인/법인 휴대폰/신용카드 인증 유무 체크
            var memberAuth = _memberRepository.GetMemberMobileAuths(new JObject { ["mode"] = "check_mem_auth", ["mem_uid"] = LoginInfo.Uid });

            if (LoginInfo.Type is not "001")
            {
                var overseasPendingVerifyRequestExistYn =
                    _memberRepository.GetMember(LoginInfo.Uid, "overseas_pending_verify_request_exists_yn");
                ViewBag.VerifyReqeustExistYn = overseasPendingVerifyRequestExistYn.Result;
                
                var memberActivity = _memberRepository.GetMemberActivities(new JObject { ["mode"] = "mypage", ["mem_uid"] = LoginInfo.Uid });
                foreach (var item in memberActivity)
                {
                    item.IsKor = IsKor();
                }
            
                ViewBag.MemberActivity = memberActivity;
            }

            if (_commonService.GetPage.Split('/').Length > 2)
            {
                ViewData["Title"] = GetPageTitle("mypage", _commonService.GetPage.Split('/')[2]);
            }

            return View(new MemberViewModel()
            {
                Member = _memberRepository.GetMember(LoginInfo.Uid),
                MemCountryCodeList = memCodeList.Where(m => m.MainCode.Equals("MEM_COUNTRY")),
                MemJobCodeList = memCodeList.Where(m => m.MainCode.Equals("MEM_JOB")),
                MemberAddresses = addressList,
                MemberAddressFlag = addressList.Any(),
                MemberClauses = clauses,
                MemberAuthFlag = memberAuth.Any() && memberAuth.First().Result.Equals("Y")
            });
        }

        #endregion

        #endregion

        #region # 마이페이지 - 결제 및 출고요청 #

        #region [WebApi]

        [Route("/api/MyPage/SetPayment")]
        public JObject SetMemberPaymentInfo([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                json["mode"] = "INSERT";
                json["mem_uid"] = LoginInfo.Uid;
                var result = _memberRepository.SetMemberPaymentInfo(json);
                if (result.Result.Equals("00"))
                {
                    var data = _memberRepository.GetMemberPaymentWorks(new JObject()
                    {
                        ["page_size"] = 100,
                        ["mem_uid"] = LoginInfo.Uid,
                        ["receipt"] = "1"
                    });
                    foreach (var item in data)
                    {
                        item.IsKor = IsKor();
                        item.WImage = GetListImageUrl(item.AucKind, item.AucNum, item.ThumFileName);
                    }
                    var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
                    return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(result.RsltMsg, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
                }
                else
                {
                    return JsonHelper.GetApiResult("ka.msg.common.error");
                }
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.error");
            }
        }

        [Route("/api/MyPage/GetPaymentWork")]
        public JObject GetPaymentWork([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                json["page_size"] = 100;
                json["mem_uid"] = LoginInfo.Uid;
                var data = _memberRepository.GetMemberPaymentWorks(json);
                foreach (var item in data)
                {
                    item.IsKor = IsKor();
                    item.WImage = GetListImageUrl(item.AucKind, item.AucNum, item.ThumFileName);
                }
                var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
                return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
            }
            
            return JsonHelper.GetApiResult("ka.msg.common.error");
        }

        [Route("/api/MyPage/SetPaymentWork")]
        public JObject SetPaymentWork([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("ka.msg.common.error");

            if (IsLogin() && LoginInfo.Uid > 0)
            {
                json["mode"] = "INSERT";

                DbResult dbResult = _memberRepository.SetMemberPaymentWork(json);
                if (dbResult.Result.Equals("00"))
                {
                    return JsonHelper.GetApiResult(dbResult.Result);
                }
            }
            return JsonHelper.GetApiResult("ka.msg.common.error");
        }

        #endregion

        #region [View]

        public IActionResult Payment()
        {
            return View();
        }

        #endregion

        #endregion

        /// <summary>
        /// 수신/참조 이메일 규칙에 맞게 주소 리턴
        /// </summary>
        /// <param name="emailInfo">LOGIN_USER (로그인한 사용자), MANAGER_APPROVER (담당자의 승인권자)</param>
        /// <param name="loginUserMail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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
            else if (emailInfo.Equals("LOGIN_USER"))
            {
                return type.ToUpper().Equals("N") ? new string[] { LoginInfo.Name } : new string[] { LoginInfo.Email };
            }
            else if (emailInfo.Equals("MANAGER_APPROVER"))
            {
                var member = _memberRepository.GetMemberKOffice(LoginInfo.Uid);
                if (member != null && !string.IsNullOrWhiteSpace(member.MngApprover))
                {
                    var list = new List<string>();
                    foreach (var item in member.MngApprover.Split(';'))
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
                else
                {
                    return Array.Empty<string>();
                }
            }
            else
            {
                return Array.Empty<string>();
            }
        }
    }
}
