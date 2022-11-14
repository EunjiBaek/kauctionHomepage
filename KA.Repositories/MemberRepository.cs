using System;
using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Entities.Models.Common;
using KA.Entities.Models.Member;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;

namespace KA.Repositories
{
    public interface IMemberRepository
    {
        /// <summary>
        /// 회원 정보 조회 처리 함수
        /// </summary>
        /// <param name="uid">유저 UID</param>
        /// <param name="mode">SP 모드</param>
        /// <returns></returns>
        Member GetMember(int uid, string mode = "detail");

        /// <summary>
        /// 회원 정보 조회 처리 함수
        /// </summary>
        /// <param name="member"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        Member GetMember(Member member, string mode = "");

        /// <summary>
        /// 케이오피스 회원 정보 조회 처리 함수
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        MemberKOffice GetMemberKOffice(int uid);

        /// <summary>
        /// 약관 정보 조회 처리 함수
        /// </summary>
        /// <returns></returns>
        IEnumerable<MemberClause> GetMemberClauses(string mode = "join", string code = "", string version = "");

        /// <summary>
        /// 약관 정보를 저장 처리 하는 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string SetMemberClause(JObject json);

        /// <summary>
        /// 회원 정보 목록 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<Member> GetMembers(JObject json);

        /// <summary>
        /// 회원 정보 입력/수정 처리 함수
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mode"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        DbResult SetMember(Member model, string mode);

        DbResult SetMemberTest(string str);

        /// <summary>
        /// 회원 탈퇴 처리 함수
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        DbResult SetMemberRetire(MemberRetire model, string mode = "");

        /// <summary>
        /// 회원 탈퇴 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetMemberRetire(JObject json);

        /// <summary>
        /// 회원 탈퇴 목록 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberRetire> GetMemberRetires(JObject json);

        /// <summary>
        /// 회원의 위시리스트 작품 설정 처리 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="memUid"></param>
        /// <param name="workUid"></param>
        /// <returns></returns>
        string SetMemberWishWork(string mode, int memUid, int workUid);

        /// <summary>
        /// 회원의 위시리스트 작품 목록 처리 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="memUid"></param>
        /// <param name="aucKind"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetMemberWishWorks(string mode, int memUid, string aucKind = "");

        /// <summary>
        /// 회원의 주소 목록 처리 함수
        /// </summary>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<MemberAddress> GetMemberAddresses(string mode, int memUid, int uid = 0, string primary = "");

        /// <summary>
        /// 회원의 주소 저장/삭제/수정 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string SetMemberAddress(JObject json);

        /// <summary>
        /// 가입시 휴대폰 인증정보 처리
        /// </summary>
        /// <param name="memberMobileAuth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        string SetMemberMobileAuth(MemberMobileAuth memberMobileAuth, string mode = "");

        /// <summary>
        /// 가입시 휴대폰 인증정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberMobileAuth> GetMemberMobileAuths(JObject json);

        /// <summary>
        /// 회원 도록 신청 내역 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberApplyBook> GetMemberApplyBook(JObject json);

        /// <summary>
        /// 회원 도록 신청 처리
        /// </summary>
        /// <param name="mode">SP 종류, REQUEST: 신청, PROCESS: 관리자 처리</param>
        /// <param name="memUid">멤버 UID</param>
        /// <param name="json">json 요청</param>
        /// <param name="forceNew">강제 신청 여부, 기본값 false</param>
        /// <returns>결과코드</returns>
        string SetMemberApplyBook(string mode, int memUid, JObject json, bool forceNew = false);

        /// <summary>
        /// 가입시 휴대폰 인증정보 처리 (해외)
        /// </summary>
        /// <param name="memberMobileForeignAuth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        DbResult SetMemberMobileForeignAuth(MemberMobileForeignAuth memberMobileForeignAuth, string mode = "");

        /// <summary>
        /// 회원 접속 정보 처리 (Middleware)
        /// </summary>
        /// <param name="memUid"></param>
        /// <param name="token"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        Member GetMemberLogin(int memUid, string token, string userAgent, string ip, string autoLogin = "", string type = "");

        /// <summary>
        /// 회원 접속 정보 처리
        /// </summary>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<MemberLogin> GetMemberLogins(JObject json);

        /// <summary>
        /// 회원 문의하기 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetMemberInquiry(JObject json);

        /// <summary>
        /// 회원 문의하기 목록 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberInquiry> GetMemberInquiries(JObject json);

        /// <summary>
        /// 회원 위탁내역 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberConsign> GetMemberConsigns(JObject json);

        /// <summary>
        /// 회원 위탁내역 상세 정보 처리
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        MemberConsign GetMemberConsign(int uid);

        /// <summary>
        /// 회원 위탁내역 저장 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetMemberConsign(JObject json);

        /// <summary>
        /// 회원 위탁내역(이미지) 상세 정보 처리
        /// </summary>
        IEnumerable<MemberConsignImg> GetMemberConsignImgs(int uid);

        /// <summary>
        /// 회원 위탁내영 첨부 저장 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetMemberConsignImg(JObject json);

        /// <summary>
        /// 회원 활동내역 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberActivity> GetMemberActivities(JObject json);

        /// <summary>
        /// 약관 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<TermAndCondition> GetTermAndConditions(JObject json);

        /// <summary>
        /// 약관 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetTermAndCondition(JObject json);

        /// <summary>
        /// 약관 상세 정보
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<TermAndConditionDetail> GetTermAndConditionDetails(JObject json);

        /// <summary>
        /// 약관 상세 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetTermAndConditionDetail(JObject json);

        /// <summary>
        /// 회원 비밀번호 변경 내역 조회
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberPasswordHistory> GetMemberPasswordHistories(JObject json);

        /// <summary>
        /// 회원 통계 내역 조회
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberStatisticJoin> GetMemberStatisticJoins(JObject json);

        /// <summary>
        /// 탈퇴 가능 여부 체크 (케이오피스)
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="kofficeMemUid"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        DbResult CheckProgressDateForException(string mode, int kofficeMemUid, int memUid);

        /// <summary>
        /// 낙찰통보서 뷰어 페이지 오픈 시 토큰값으로 정보 처리
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        MemberNotiToken GetMemberNotiToken(string token);

        /// <summary>
        /// 낙찰통보서 조회 이력 처리
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        DbResult SetMemberNotiRead(string token);

        /// <summary>
        /// 낙찰내역 - 작품 보증서 조회/출력 내역 정보 처리
        /// </summary>
        /// <param name="memberCertificateHistory"></param>
        /// <returns></returns>
        DbResult SetMemberCertificateHistory(string mode, MemberCertificateHistory memberCertificateHistory);

        /// <summary>
        /// 관리자 - 회원 작품 보증서 출력 내역 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberCertificateHistory> GetMemberCertificateHistories(JObject json);

        /// <summary>
        /// 낙찰내역 - 작품 보증서 배송 요청 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetMemberCertificateRequest(JObject json);

        IEnumerable<MemberCertificateRequest> GetMemberCertificateRequests(JObject json);

        /// <summary>
        /// 회원의 선호 정보 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<MemberInterestArtist> GetMemberInterestArtists(JObject json);

        DbResult SetMemberInterestInfo(JObject json);

        DbResult SetMemberPaymentInfo(JObject json);

        IEnumerable<MemberPaymentWork> GetMemberPaymentWorks(JObject json);

        DbResult SetMemberPaymentWork(JObject json);

        // 고객 발송 양식 정보 처리 From Koffice
        MemberDocument GetMemberDocument(string token);

        IEnumerable<MemberDailyAccessStatus> GetMemberDailyAccessStatuses(JObject json);

        IEnumerable<MemberLogin> GetMemberDailyAccessStatusesDetail(JObject json);
    }

    public class MemberRepository : BaseRepository, IMemberRepository
    {
        private readonly ICommonRepository commonRepository;

        public MemberRepository(IConfiguration configuration, ICommonRepository commonRepository)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");

            this.commonRepository = commonRepository;
        }

        public IEnumerable<Member> GetMembers(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            return GetResults<Member>("usp_member_select", parameters);
        }

        public Member GetMember(int uid, string mode = "detail")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@uid", uid);
            return GetSingleResult<Member>("usp_member_select", parameters);
        }

        public Member GetMember(Member member, string mode = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@uid", member.Uid);
            parameters.Add("@id", member.ID);
            parameters.Add("@pwd", member.Pwd);
            parameters.Add("@name", member.Name);
            parameters.Add("@email", member.Email);
            parameters.Add("@mobile", member.Mobile);
            parameters.Add("@migration", member.Migration);
            parameters.Add("@type", member.RegType);
            parameters.Add("@ip", member.IP);
            parameters.Add("@user_agent", member.UserAgent);
            parameters.Add("@token", member.Token);
            parameters.Add("@is_saved", member.IsSaved);
            parameters.Add("@highlight_read", member.HighlightRead);
            return GetSingleResult<Member>("usp_member_select", parameters);
        }

        public MemberKOffice GetMemberKOffice(int uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "detail");
            parameters.Add("@uid", uid);
            return GetSingleResult<MemberKOffice>("usp_koffice_member_select", parameters);
        }

        public IEnumerable<MemberClause> GetMemberClauses(string mode = "join", string code = "", string version = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@code", code);
            parameters.Add("@version", version);
            return GetResults<MemberClause>("usp_member_clause_select", parameters);
        }

        public string SetMemberClause(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@id", JsonHelper.GetString(json, "id"));
            parameters.Add("@code", JsonHelper.GetString(json, "code"));
            parameters.Add("@version", JsonHelper.GetString(json, "version"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang"));
            parameters.Add("@content", JsonHelper.GetString(json, "content"));
            parameters.Add("@content2", JsonHelper.GetString(json, "content2"));
            parameters.Add("@seq", JsonHelper.GetString(json, "seq"));

            return GetSingleResult<string>("usp_member_clause_update", parameters);
        }

        public DbResult SetMember(Member model, string mode)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);

            parameters.Add("@uid", model.Uid);
            parameters.Add("@type", model.Type ?? "");
            parameters.Add("@verification", model.Verification ?? "");

            parameters.Add("@mobile_auth", model.MobileAuthSeq ?? "");

            parameters.Add("@id", model.ID ?? "");
            parameters.Add("@name", model.Name ?? "");

            parameters.Add("@pwd", model.Pwd);
            parameters.Add("@pwd_new", model.PwdNew);

            parameters.Add("@pwd_new_original", model.PwdNew);
            parameters.Add("@pwd_type", model.PwdType ?? "");

            parameters.Add("@pwd_noti_target", model.PwdNotiTarget);
            parameters.Add("@pwd_noti_value", model.PwdNotiValue);

            parameters.Add("@birth_date", model.BirthDate == null ? "" : model.BirthDate.Replace('/', '-'));
            parameters.Add("@birth_type", model.BirthType ?? "");
            parameters.Add("@sex", model.Sex ?? "");

            parameters.Add("@email", model.Email ?? "");
            parameters.Add("@country_code", model.CountryCode ?? "");
            parameters.Add("@mobile", StringHelper.GetMobileHyphen(model.Mobile ?? ""));

            parameters.Add("@zipcode", model.ZipCode ?? "");
            parameters.Add("@address", model.Address ?? "");
            parameters.Add("@address2", model.Address2 ?? "");

            parameters.Add("@identification", model.Identification ?? "");

            parameters.Add("@company_name", model.CompanyName ?? "");
            parameters.Add("@company_president", model.CompanyPresident ?? "");
            parameters.Add("@company_tel", StringHelper.GetMobileHyphen(model.CompanyTel ?? ""));
            parameters.Add("@company_rep_tel", StringHelper.GetMobileHyphen(model.CompanyRepTel ?? ""));
            parameters.Add("@company_fax", StringHelper.GetMobileHyphen(model.CompanyFax ?? ""));
            parameters.Add("@company_reg_no", model.CompanyRegNo ?? "");
            parameters.Add("@company_reg_doc", model.CompanyRegDoc ?? "");
            parameters.Add("@company_business_card", model.CompanyBusinessCard ?? "");
            parameters.Add("@tax_email", model.TaxEmail ?? "");

            parameters.Add("@job_code", model.JobCode ?? "");

            parameters.Add("@company_zipcode", model.CompanyZipCode ?? "");
            parameters.Add("@company_address", model.CompanyAddress ?? "");
            parameters.Add("@company_address2", model.CompanyAddress2 ?? "");

            parameters.Add("@delivery_zipcode", model.DeliveryZipCode ?? "");
            parameters.Add("@delivery_address", model.DeliveryAddress ?? "");
            parameters.Add("@delivery_address2", model.DeliveryAddress2 ?? "");

            parameters.Add("@reg_type", model.RegType ?? "");
            parameters.Add("@private_clause", model.PrivateClause ?? "");
            parameters.Add("@clause_info", model.ClauseInfo ?? "");
            parameters.Add("@receiving_advertising", model.ReceivingAdvertising ?? "");
            parameters.Add("@info_validate_period", model.InfoValidatePeriod ?? "001");
            parameters.Add("@receive_sms_info", model.ReceiveSmsInfo ?? "");
            parameters.Add("@receive_email_info", model.ReceiveEmailInfo ?? "");
            parameters.Add("@receive_phone_info", model.ReceivePhoneInfo ?? "");

            parameters.Add("@ip", model.IP ?? "");
            parameters.Add("@user_agent", model.UserAgent ?? "");

            parameters.Add("@info_req", model.InfoReq ?? "");
            parameters.Add("@info_req_file", model.InfoReqFile ?? "");

            parameters.Add("@manager_yn", model.ManagerYN ?? "");
            parameters.Add("@admin_yn", model.AdminYN ?? "");

            parameters.Add("@receive_info_type", model.ReceiveInfoType ?? "");
            parameters.Add("@receive_info_code", model.ReceiveInfoCode ?? "");
            parameters.Add("@receive_info_value", model.ReceiveInfoValue ?? "");

            parameters.Add("@collect_personal_info_address", model.CollectPersonalInfoAddressYN ?? "N");
            parameters.Add("@provide_personal_info_agree", model.ProvidePersonalInfoAgreeYN ?? "N");
            parameters.Add("@collect_personal_info_content", model.CollectPersonalInfoContentYN ?? "N");

            parameters.Add("@interest_area", model.InterestArea ?? "");
            parameters.Add("@interest_artist", model.InterestArtist ?? "");

            parameters.Add("@main_highlight", model.MainHighlight ?? "001");
            parameters.Add("@list_view_mode", model.ListViewMode ?? "001");

            return GetSingleResult<DbResult>("usp_member_update", parameters);
        }

        public DbResult SetMemberTest(string str)
        {
            int result = ExecuteQuery(str);
            return new DbResult { Result = (result > 0 ? "00" : "90") };
        }

        public DbResult SetMemberRetire(MemberRetire model, string mode = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@uid", model.Uid);
            parameters.Add("@retire_reason", model.RetireReason);
            parameters.Add("@retire_option", model.RetireOption);
            parameters.Add("@mng_uid", model.MngUid);
            return GetSingleResult<DbResult>("usp_member_retire_update", parameters);
        }

        public DbResult SetMemberRetire(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@retire_reason", JsonHelper.GetString(json, "retire_reason"));
            parameters.Add("@retire_option", JsonHelper.GetString(json, "retire_option"));
            parameters.Add("@mng_uid", JsonHelper.GetString(json, "mng_uid", "0"));
            return GetSingleResult<DbResult>("usp_member_retire_update", parameters);
        }

        public IEnumerable<MemberRetire> GetMemberRetires(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            return GetResults<MemberRetire>("usp_member_retire_select", parameters);
        }

        public string SetMemberWishWork(string mode, int memUid, int workUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@mem_uid", memUid);
            parameters.Add("@work_uid", workUid);
            return GetSingleResult<string>("usp_member_wish_work_update", parameters);
        }

        public IEnumerable<AuctionWork> GetMemberWishWorks(string mode, int memUid, string aucKind)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@mem_uid", memUid);
            parameters.Add("@auc_kind", aucKind);
            return GetResults<AuctionWork>("usp_member_wish_work_select", parameters);
        }

        public IEnumerable<MemberAddress> GetMemberAddresses(string mode, int memUid, int uid = 0, string primary = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@mem_uid", memUid);
            parameters.Add("@uid", uid);
            parameters.Add("@primary", primary);
            return GetResults<MemberAddress>("usp_member_address_select", parameters);
        }

        public string SetMemberAddress(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "-1"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "-1"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@etc", JsonHelper.GetString(json, "etc"));
            parameters.Add("@country_code", JsonHelper.GetString(json, "country_code"));
            parameters.Add("@zipcode", JsonHelper.GetString(json, "zipcode"));
            parameters.Add("@address1", JsonHelper.GetString(json, "address1"));
            parameters.Add("@address2", JsonHelper.GetString(json, "address2"));
            parameters.Add("@receiver", JsonHelper.GetString(json, "receiver"));
            parameters.Add("@contact", JsonHelper.GetString(json, "contact"));
            parameters.Add("@collect_personal_info_address", JsonHelper.GetString(json, "collect_personal_info_address"));
            parameters.Add("@provide_personal_info_agree", JsonHelper.GetString(json, "provide_personal_info_agree"));
            var result = GetSingleResult<DbResult>("usp_member_address_update", parameters);
            if (!result.Result.Equals("00")) commonRepository.SetErrorLog(result);
            return result.Result;
        }

        public string SetMemberMobileAuth(MemberMobileAuth memberMobileAuth, string mode = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@seq", memberMobileAuth.Seq);
            parameters.Add("@auth", memberMobileAuth.Auth);
            parameters.Add("@type", memberMobileAuth.Type);
            parameters.Add("@type2", memberMobileAuth.Type2);
            parameters.Add("@type_detail", memberMobileAuth.TypeDetail);
            parameters.Add("@state", memberMobileAuth.State);
            parameters.Add("@device", memberMobileAuth.Device);
            parameters.Add("@user_agent", memberMobileAuth.UserAgent);
            parameters.Add("@ip", memberMobileAuth.Ip);
            parameters.Add("@result", memberMobileAuth.Result);
            parameters.Add("@message", memberMobileAuth.Message);
            parameters.Add("@mem_uid", memberMobileAuth.MemUid);
            parameters.Add("@mem_id", memberMobileAuth.MemId);
            parameters.Add("@mem_name", memberMobileAuth.MemName);
            parameters.Add("@mobile_code", memberMobileAuth.MobileCode);
            parameters.Add("@mobile_co", memberMobileAuth.MobileCo);
            parameters.Add("@mobile_no", memberMobileAuth.MobileNo);
            parameters.Add("@crd_cd", memberMobileAuth.CrdCD);
            parameters.Add("@birthdate", memberMobileAuth.BirthDate);
            parameters.Add("@gender", memberMobileAuth.Gender);
            parameters.Add("@di", memberMobileAuth.DI);
            parameters.Add("@ci", memberMobileAuth.CI);
            parameters.Add("@cipher_time", memberMobileAuth.CipherTime);
            parameters.Add("@auth_no", memberMobileAuth.AuthNo);
            parameters.Add("@redirect_url", memberMobileAuth.RedirectUrl);
            return GetSingleResult<string>("usp_member_mobile_auth_update", parameters, _kauctionConnectionString);
        }

        public IEnumerable<MemberMobileAuth> GetMemberMobileAuths(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@seq", JsonHelper.GetString(json, "seq"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetResults<MemberMobileAuth>("usp_member_mobile_auth_select", parameters);
        }

        public IEnumerable<MemberApplyBook> GetMemberApplyBook(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            return GetResults<MemberApplyBook>("usp_member_apply_book_select", parameters);
        }

        /// <inheritdoc />
        public string SetMemberApplyBook(string mode, int memUid, JObject json, bool forceNew = false)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@mem_uid", memUid);
            parameters.Add("@force_new", forceNew);
            if (json != null)
            {
                parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
                parameters.Add("@address_uid", JsonHelper.GetString(json, "address_uid", "0"));
                parameters.Add("@receiver", JsonHelper.GetString(json, "receiver"));
                parameters.Add("@contact", JsonHelper.GetString(json, "contact"));
                parameters.Add("@mng_uid", JsonHelper.GetString(json, "mng_uid", "0"));
                parameters.Add("@pay_s_date", JsonHelper.GetString(json, "pay_s_date"));
            }
            return GetSingleResult<string>("usp_member_apply_book_update", parameters);
        }

        public DbResult SetMemberMobileForeignAuth(MemberMobileForeignAuth memberMobileForeignAuth, string mode = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@seq", memberMobileForeignAuth.Seq);
            parameters.Add("@type", memberMobileForeignAuth.Type);
            parameters.Add("@type_detail", memberMobileForeignAuth.TypeDetail);
            parameters.Add("@device", memberMobileForeignAuth.Device);
            parameters.Add("@user_agent", memberMobileForeignAuth.UserAgent);
            parameters.Add("@ip", memberMobileForeignAuth.Ip);
            parameters.Add("@mem_uid", memberMobileForeignAuth.MemUid);
            parameters.Add("@auth", memberMobileForeignAuth.Auth);
            parameters.Add("@mobile_no", memberMobileForeignAuth.MobileNo);
            parameters.Add("@auth_no", memberMobileForeignAuth.AuthNo);
            parameters.Add("@lang", memberMobileForeignAuth.Lang);
            return GetSingleResult<DbResult>("usp_member_mobile_foreign_auth_update", parameters);
        }

        public Member GetMemberLogin(int memUid, string token, string userAgent, string ip, string autoLogin = "", string type = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "login");
            parameters.Add("@mem_uid", memUid);
            parameters.Add("@token", token);
            parameters.Add("@user_agent", userAgent);
            parameters.Add("@ip", ip);
            parameters.Add("@auto_login", autoLogin);
            parameters.Add("@type", type);
            return GetSingleResult<Member>("usp_member_login_select", parameters);
        }

        public IEnumerable<MemberLogin> GetMemberLogins(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode", "list"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetResults<MemberLogin>("usp_member_login_select", parameters);
        }

        public DbResult SetMemberInquiry(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@org_uid", JsonHelper.GetString(json, "org_uid", "0"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameters.Add("@contents", JsonHelper.GetString(json, "contents"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid", "0"));
            parameters.Add("@reg_type", JsonHelper.GetString(json, "reg_type"));
            parameters.Add("@mail_yn", JsonHelper.GetString(json, "mail_yn"));
            var category = JsonHelper.GetString(json, "category");
            if (!string.IsNullOrEmpty(category))
            {
                parameters.Add("@category", JsonHelper.GetString(json, "category"));
                return GetSingleResult<DbResult>("usp_member_inquiry_update", parameters);
            }
            return GetSingleResult<DbResult>("usp_member_inquiry_update", parameters);
        }

        public IEnumerable<MemberInquiry> GetMemberInquiries(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            parameters.Add("@reg_type", JsonHelper.GetString(json, "reg_type", "M"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid", "0"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            var states = JsonHelper.GetArray(json, "states");
            parameters.Add("@states", states.Length > 0 ? string.Join(" ", states) : string.Empty);
            parameters.Add("@start_date", JsonHelper.GetDateTime(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetDateTime(json, "end_date"));
            return GetResults<MemberInquiry>("usp_member_inquiry_select", parameters);
        }

        public IEnumerable<MemberConsign> GetMemberConsigns(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "-1"));
            parameters.Add("@state", JsonHelper.GetString(json, "state"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            var states = JsonHelper.GetArray(json, "states");
            parameters.Add("@states", states.Length > 0 ? string.Join(" ", states) : string.Empty);
            parameters.Add("@start_date", JsonHelper.GetDateTime(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetDateTime(json, "end_date"));
            return GetResults<MemberConsign>("usp_member_consign_select", parameters);
        }

        public MemberConsign GetMemberConsign(int uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "detail");
            parameters.Add("@uid", uid);
            return GetSingleResult<MemberConsign>("usp_member_consign_select", parameters);
        }

        public DbResult SetMemberConsign(JObject json)
        {
            var workX = decimal.TryParse(JsonHelper.GetString(json, "work_x", "0"), out decimal retX) ? retX : 0;
            var workY = decimal.TryParse(JsonHelper.GetString(json, "work_y", "0"), out decimal retY) ? retY : 0;
            var workZ = decimal.TryParse(JsonHelper.GetString(json, "work_z", "0"), out decimal retZ) ? retZ : 0;
            var ho = int.TryParse(JsonHelper.GetString(json, "ho", "0"), out int retHo) ? retHo : 0;
            var pricePurchase = decimal.TryParse(JsonHelper.GetString(json, "price_purchase", "0"), out decimal retPP) ? retPP : 0;
            var priceDesired = decimal.TryParse(JsonHelper.GetString(json, "price_desired", "0"), out decimal retPD) ? retPD : 0;
            var recommendedPrice = decimal.TryParse(JsonHelper.GetString(json, "recommended_price", "0"), out decimal retRP) ? retRP : 0;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "-1"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "-1"));
            parameters.Add("@code", JsonHelper.GetString(json, "code"));
            parameters.Add("@artist", JsonHelper.GetString(json, "artist"));
            parameters.Add("@title", JsonHelper.GetString(json, "title"));
            parameters.Add("@work_x", workX);
            parameters.Add("@work_y", workY);
            parameters.Add("@work_z", workZ);
            parameters.Add("@ho", ho);
            parameters.Add("@edition", JsonHelper.GetString(json, "edition"));
            parameters.Add("@material_code", JsonHelper.GetString(json, "material_code"));
            parameters.Add("@meterial_etc", JsonHelper.GetString(json, "material_etc"));
            parameters.Add("@make_date", JsonHelper.GetString(json, "make_date"));
            parameters.Add("@desc", JsonHelper.GetString(json, "desc"));
            parameters.Add("@etc", JsonHelper.GetString(json, "etc"));
            parameters.Add("@price_purchase", pricePurchase);
            parameters.Add("@price_desired", priceDesired);
            parameters.Add("@state", JsonHelper.GetString(json, "state"));
            parameters.Add("@admin_uid", JsonHelper.GetString(json, "admin_uid"));
            parameters.Add("@receipt_yn", JsonHelper.GetString(json, "receipt_yn"));
            parameters.Add("@recommended_price", recommendedPrice);
            parameters.Add("@memo", JsonHelper.GetString(json, "memo"));
            parameters.Add("@memo_sales_team", JsonHelper.GetString(json, "memo_sales_team"));
            parameters.Add("@chk_email", JsonHelper.GetString(json, "chk_email"));
            return GetSingleResult<DbResult>("usp_member_consign_update", parameters);
        }

        public IEnumerable<MemberConsignImg> GetMemberConsignImgs(int uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@uid", uid);
            return GetResults<MemberConsignImg>("usp_member_consign_img_select", parameters);
        }

        public DbResult SetMemberConsignImg(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@code", JsonHelper.GetString(json, "code"));
            parameters.Add("@filenames", JsonHelper.GetString(json, "filenames"));
            return GetSingleResult<DbResult>("usp_member_consign_img_update", parameters);
        }

        public IEnumerable<MemberActivity> GetMemberActivities(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetResults<MemberActivity>("usp_member_activity_select", parameters);
        }

        public IEnumerable<TermAndCondition> GetTermAndConditions(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@version", JsonHelper.GetString(json, "version"));
            return GetResults<TermAndCondition>("usp_term_and_condition_select", parameters);
        }

        public DbResult SetTermAndCondition(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@version", JsonHelper.GetString(json, "version"));
            parameters.Add("@description", JsonHelper.GetString(json, "description"));
            parameters.Add("@description_en", JsonHelper.GetString(json, "description_en"));
            parameters.Add("@start_date", JsonHelper.GetString(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetString(json, "end_date"));
            parameters.Add("@date_primary_yn", JsonHelper.GetString(json, "date_primary_yn"));
            parameters.Add("@use_yn", JsonHelper.GetString(json, "use_yn"));
            return GetSingleResult<DbResult>("usp_term_and_condition_update", parameters);
        }

        public IEnumerable<TermAndConditionDetail> GetTermAndConditionDetails(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@version", JsonHelper.GetString(json, "version"));
            parameters.Add("@code", JsonHelper.GetString(json, "code"));
            return GetResults<TermAndConditionDetail>("usp_term_and_condition_detail_select", parameters);
        }

        public DbResult SetTermAndConditionDetail(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@version", JsonHelper.GetString(json, "version", "0"));
            parameters.Add("@code", JsonHelper.GetString(json, "code"));
            parameters.Add("@content", JsonHelper.GetString(json, "content"));
            parameters.Add("@content_en", JsonHelper.GetString(json, "content_en"));
            parameters.Add("@desc", JsonHelper.GetString(json, "desc"));
            parameters.Add("@agree_yn", JsonHelper.GetString(json, "agree_yn"));
            parameters.Add("@login_yn", JsonHelper.GetString(json, "login_yn"));
            parameters.Add("@use_yn", JsonHelper.GetString(json, "use_yn"));
            return GetSingleResult<DbResult>("usp_term_and_condition_detail_update", parameters);
        }

        public IEnumerable<MemberPasswordHistory> GetMemberPasswordHistories(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid"));
            return GetResults<MemberPasswordHistory>("usp_member_password_hst_select", parameters);
        }

        public IEnumerable<MemberStatisticJoin> GetMemberStatisticJoins(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@start_date", JsonHelper.GetString(json, "start_date"));
            return GetResults<MemberStatisticJoin>("usp_member_statistic_select", parameters);
        }

        public DbResult CheckProgressDateForException(string mode, int kofficeMemUid, int memUid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@chk_mode", SqlDbType.VarChar, 10)
                {
                    Value = mode
                },
                new SqlParameter("@koff_mem_uid", SqlDbType.Int)
                {
                    Value = kofficeMemUid
                },
                new SqlParameter("@kauc_mem_uid", SqlDbType.Int)
                {
                    Value = memUid
                },
                new SqlParameter("@req_typ_cd", SqlDbType.VarChar, 10)
                {
                    Value = "KAUCTION"
                }
            };

            using DataSet dataSet = GetDataSet("db_koffice.dbo.usp_Mem_ProgressDateChkForException", sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables.Count == 1 ? dataSet.Tables[0] : dataSet.Tables[1];
                return new DbResult
                {
                    RsltCD = dataTable.Rows.Count > 0 ? dataTable.Rows[0]["rslt_cd"].ToString() : null,
                    RsltMsg = dataTable.Rows.Count > 0 ? dataTable.Rows[0]["rslt_msg"].ToString() : null,
                    RsltReqStatCD = dataTable.Rows.Count > 0 ? dataTable.Rows[0]["rslt_req_stat_cd"].ToString() : null
                };
            }
            else
            {
                return null;
            }
        }

        public MemberNotiToken GetMemberNotiToken(string token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@token_val", token);
            return GetSingleResult<MemberNotiToken>("db_koffice.dbo.usp_Auc_UserNotiTokenInfo_Select", parameters);
        }

        public DbResult SetMemberNotiRead(string token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "UPDATE_READ");
            parameters.Add("@token_val", token);
            return GetSingleResult<DbResult>("db_koffice.dbo.usp_Auc_UserNotiTokenInfo_Update", parameters);
        }

        public DbResult SetMemberCertificateHistory(string mode, MemberCertificateHistory memberCertificateHistory)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@work_uid", memberCertificateHistory.WorkUid);
            parameters.Add("@mem_uid", memberCertificateHistory.MemUid);
            parameters.Add("@filename", memberCertificateHistory.FileName);
            parameters.Add("@test_mode", memberCertificateHistory.TestMode);
            parameters.Add("@print_yn", memberCertificateHistory.PrintYN);
            parameters.Add("@del_yn", memberCertificateHistory.DelYN);
            return GetSingleResult<DbResult>("usp_member_certificate_hst_update", parameters);
        }

        public IEnumerable<MemberCertificateHistory> GetMemberCertificateHistories(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameters.Add("@test_yn", JsonHelper.GetString(json, "test_yn"));
            return GetResults<MemberCertificateHistory>("usp_member_certificate_hst_select", parameters);
        }

        public DbResult SetMemberCertificateRequest(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameters.Add("@address_uid", JsonHelper.GetString(json, "address_uid", "0"));
            parameters.Add("@receiver_name", JsonHelper.GetString(json, "receiver_name"));
            parameters.Add("@receiver_mobile", JsonHelper.GetString(json, "receiver_mobile"));
            parameters.Add("@receiver_email", JsonHelper.GetString(json, "receiver_email"));
            parameters.Add("@email_flag", JsonHelper.GetString(json, "email_flag"));
            return GetSingleResult<DbResult>("usp_member_certificate_request_update", parameters);
        }

        public IEnumerable<MemberCertificateRequest> GetMemberCertificateRequests(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            return GetResults<MemberCertificateRequest>("usp_member_certificate_request_select", parameters);
        }

        public IEnumerable<MemberInterestArtist> GetMemberInterestArtists(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode", "MEM_ARTIST"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@key", JsonHelper.GetString(json, "key"));
            parameters.Add("@sort_column", JsonHelper.GetString(json, "sort_column"));
            parameters.Add("@sort_option", JsonHelper.GetString(json, "sort_option"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            return GetResults<MemberInterestArtist>("usp_member_interest_select", parameters);
        }

        public DbResult SetMemberInterestInfo(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@code", JsonHelper.GetString(json, "code"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            return GetSingleResult<DbResult>("usp_member_interest_update", parameters);
        }

        public DbResult SetMemberPaymentInfo(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid"));
            parameters.Add("@receiver", JsonHelper.GetString(json, "receiver"));
            parameters.Add("@contact", JsonHelper.GetString(json, "contact"));
            parameters.Add("@desired_date_1", JsonHelper.GetString(json, "desired_date_1"));
            parameters.Add("@desired_date_2", JsonHelper.GetString(json, "desired_date_2"));
            parameters.Add("@elevator_type", JsonHelper.GetString(json, "elevator_type"));
            parameters.Add("@vehicle_entry", JsonHelper.GetString(json, "vehicle_entry"));
            parameters.Add("@ladder_truck", JsonHelper.GetString(json, "ladder_truck"));
            parameters.Add("@memo", JsonHelper.GetString(json, "memo"));
            parameters.Add("@installation", JsonHelper.GetString(json, "installation"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetSingleResult<DbResult>("usp_member_payment_info_update", parameters);
        }

        public IEnumerable<MemberPaymentWork> GetMemberPaymentWorks(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind"));
            parameters.Add("@receipt", JsonHelper.GetString(json, "receipt"));
            parameters.Add("@start_date", JsonHelper.GetString(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetString(json, "end_date"));
            parameters.Add("@search", JsonHelper.GetString(json, "search"));
            return GetResults<MemberPaymentWork>("usp_member_payment_work_select", parameters);
        }

        public DbResult SetMemberPaymentWork(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid"));
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid"));
            return GetSingleResult<DbResult>("usp_member_payment_work_update", parameters);
        }

        public MemberDocument GetMemberDocument(string token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@token", token);
            return GetSingleResult<MemberDocument>("db_koffice.dbo.usp_mem_document_select", parameters);
        }

        public IEnumerable<MemberDailyAccessStatus> GetMemberDailyAccessStatuses(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@proc_date", JsonHelper.GetString(json, "proc_date"));
            return GetResults<MemberDailyAccessStatus>("usp_member_login_statistic_select", parameters);
        }

        public IEnumerable<MemberLogin> GetMemberDailyAccessStatusesDetail(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@proc_date", JsonHelper.GetString(json, "proc_date"));
            return GetResults<MemberLogin>("usp_member_login_statistic_select", parameters);
        }
    }
}
