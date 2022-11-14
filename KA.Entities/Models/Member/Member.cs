using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.05.09 :: MobileAuthType 변수 추가 - 휴대폰 인증 유형 (KCB, 일반 휴대폰 인증)
    /// 2022.05.26 :: ApplyBookRegDate 변수 추가 - 도록 신청 일자
    /// 2022.05.30 :: PwdNotiTarget, PwdNotiValue 변수 추가 - 관리자 비밀번호 변경 시 알림을 위한 별도 입력값 처리    
    /// </summary>
    public class Member
    {
        /// <summary>
        /// 회원 고유번호
        /// </summary>
        [JsonProperty("uid")]
        public int Uid { get; set; }

        /// <summary>
        /// 케이오피스 회원 고유번호
        /// </summary>
        [JsonProperty("koffice_uid")]
        public int KofficeUid { get; set; }

        [JsonProperty("koffice_link")]
        public string KofficeLink { get; set; }

        /// <summary>
        /// 회원 유형 코드
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 회원 유형명
        /// </summary>
        [JsonProperty("type_name")]
        public string TypeName { get; set; }

        /// <summary>
        /// 회원 유형명(영문)
        /// </summary>
        [JsonProperty("type_name_en")]
        public string TypeNameEn { get; set; }

        /// <summary>
        /// 가입시 인증 유형 (Type 001, 003)
        /// </summary>
        [JsonIgnore]
        public string Verification { get; set; }

        /// <summary>
        /// 휴대폰 인증 정보 Guid
        /// </summary>
        [JsonProperty("mobile_auth_seq")]
        public string MobileAuthSeq { get; set; }

        /// <summary>
        /// 아이디
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 생년월일
        /// </summary>
        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        /// <summary>
        /// 생년월일 유형 (S:양력, L:음력)
        /// </summary>
        [JsonIgnore]
        public string BirthType { get; set; }

        /// <summary>
        /// 성별
        /// </summary>
        [JsonProperty("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// 성별 코드명
        /// </summary>
        [JsonProperty("sex_name")]
        public string SexName { get; set; }

        /// <summary>
        /// 휴대폰
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 휴대폰 인증 유형
        /// 2022.05.09 :: 마이페이지 개편으로 인하여, 일반 인증번호 요청-확인 절차로 휴대폰 번호 업데이트 가능
        ///             - M: 일반 휴대폰 인증, K: KCB 인증
        ///             - 일반 휴대폰 인증 시 마이페이지에서 인증됨 미표기 처리  
        /// </summary>
        [JsonProperty("mobile_auth_type")]
        public string MobileAuthType { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// 이메일 (세금계산서)
        /// </summary>
        [JsonProperty("tax_email")]
        public string TaxEmail { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        [JsonIgnore]
        public string Pwd { get; set; }

        /// <summary>
        /// My Page > 정보수정 > 비밀번호 변경 시 신규 비밀번호 값 처리
        /// </summary>
        [JsonIgnore]
        public string PwdNew { get; set; }

        /// <summary>
        /// 비밀번호 변경 타입 (F:비밀번호 찾기, M:정보수정, A:관리자)
        /// </summary>
        [JsonIgnore]
        public string PwdType { get; set; }

        /// <summary>
        /// 비밀번호 변경 시 알람대상 유형 (U:관리자 입력값, A:인증정보)
        /// </summary>
        [JsonIgnore]
        public string PwdNotiTarget { get; set; }

        /// <summary>
        /// 비빌번호 변경 시 알람대상 값 (문자인 경우 관리자 입력 문자번호, 이메일인 경우 관리자 입력 메일주소)
        /// </summary>
        [JsonIgnore]
        public string PwdNotiValue { get; set; }

        /// <summary>
        /// 국가코드
        /// </summary>
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        /// <summary>
        /// 국가 코드명
        /// </summary>
        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        /// <summary>
        /// 신분증 파일명
        /// </summary>
        [JsonProperty("identification")]
        public string Identification { get; set; }

        /// <summary>
        /// 법인명
        /// </summary>
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 대표자명
        /// </summary>
        [JsonProperty("company_president")]
        public string CompanyPresident { get; set; }

        /// <summary>
        /// 대표번호
        /// </summary>
        [JsonProperty("company_rep_tel")]
        public string CompanyRepTel { get; set; }

        /// <summary>
        /// 유선번호
        /// </summary>
        [JsonProperty("company_tel")]
        public string CompanyTel { get; set; }

        /// <summary>
        /// 팩스번호
        /// </summary>
        [JsonProperty("company_fax")]
        public string CompanyFax { get; set; }

        /// <summary>
        /// 사업자 등록번호
        /// </summary>
        [JsonProperty("company_reg_no")]
        public string CompanyRegNo { get; set; }

        /// <summary>
        /// 사업자 등록증 파일
        /// </summary>
        [JsonProperty("company_reg_doc")]
        public string CompanyRegDoc { get; set; }

        /// <summary>
        /// 담당자 명함 파일
        /// </summary>
        [JsonProperty("company_business_card")]
        public string CompanyBusinessCard { get; set; }

        /// <summary>
        /// 직업 코드
        /// </summary>
        [JsonProperty("job_code")]
        public string JobCode { get; set; }

        /// <summary>
        /// 도록신청 상태
        /// </summary>
        [JsonIgnore]
        public string ApplyBookKind { get; set; }

        /// <summary>
        /// 도록신청 일자
        /// 2022.05.09 :: 마이페이지 개편으로 인하여, 도록 신청 일자 정보 표기 (상태 클릭 시 팝업)        
        /// </summary>
        [JsonIgnore]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ApplyBookRegDate { get; set; }

        /// <summary>
        /// 담당자 가입 승인 여부 
        /// </summary>
        [JsonIgnore]
        public string ManagerApproval { get; set; }

        /// <summary>
        /// 응찰 가능 여부
        /// </summary>
        [JsonIgnore]
        public string BidAllowYN { get; set; }

        /// <summary>
        /// 접속 IP
        /// </summary>
        [JsonIgnore]
        public string IP { get; set; }

        /// <summary>
        /// 접속 Agent
        /// </summary>
        [JsonIgnore]
        public string UserAgent { get; set; }

        /// <summary>
        /// 가입시 디바이스 유형 (A: 앱(안드로이드), I:앱(아이폰), M:모바일, W:웹)
        /// </summary>
        [JsonIgnore]
        public string RegType { get; set; }

        /// <summary>
        /// 우편번호
        /// </summary>
        [JsonIgnore]
        public string ZipCode { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        [JsonIgnore]
        public string Address { get; set; }

        /// <summary>
        /// 상세주소
        /// </summary>
        [JsonIgnore]
        public string Address2 { get; set; }

        /// <summary>
        /// 회사 우편번호
        /// </summary>
        [JsonIgnore]
        public string CompanyZipCode { get; set; }

        /// <summary>
        /// 회사 주소
        /// </summary>
        [JsonIgnore]
        public string CompanyAddress { get; set; }

        /// <summary>
        /// 회사 상세주소
        /// </summary>
        [JsonIgnore]
        public string CompanyAddress2 { get; set; }

        /// <summary>
        /// 배송지 우편번호
        /// </summary>
        [JsonIgnore]
        public string DeliveryZipCode { get; set; }

        /// <summary>
        /// 배송지 주소
        /// </summary>
        [JsonIgnore]
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// 배송지 상세주소
        /// </summary>
        [JsonIgnore]
        public string DeliveryAddress2 { get; set; }

        /// <summary>
        /// 필수약관 동의 버전
        /// </summary>
        [JsonIgnore]
        public int EssentialClause { get; set; }

        /// <summary>
        /// 개인정보 약관 동의 여부 (선택사항)
        /// </summary>
        [JsonIgnore]
        public string PrivateClause { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (선택사항)
        /// </summary>
        [JsonIgnore]
        public string ReceivingAdvertising { get; set; }

        /// <summary>
        /// 전체 약관 동의여부 값 처리 (가입시 로그)
        /// </summary>
        [JsonIgnore]
        public string ClauseInfo { get; set; }

        /// <summary>
        /// 약관 동의 일자
        /// </summary>
        [JsonIgnore]
        public DateTime ClauseDate { get; set; }

        /// <summary>
        /// 개인정보 수집/이용 (주소)
        /// </summary>
        [JsonIgnore]
        public string CollectPersonalInfoAddressYN { get; set; }

        /// <summary>
        /// 개인정보 수집/이용 (맞춤 컨텐츠)
        /// </summary>
        [JsonIgnore]
        public string CollectPersonalInfoContentYN { get; set; }

        /// <summary>
        /// 개인정보 제3자 제공 동의
        /// </summary>
        [JsonIgnore]
        public string ProvidePersonalInfoAgreeYN { get; set; }

        /// <summary>
        /// 관심분야
        /// </summary>
        [JsonIgnore]
        public string InterestArea { get; set; }

        /// <summary>
        /// 관심작가
        /// </summary>
        [JsonIgnore]
        public string InterestArtist { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (문자)
        /// </summary>
        [JsonProperty("receive_sms_info")]
        public string ReceiveSmsInfo { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (문자) 최종 변경일자
        /// </summary>
        [JsonIgnore]
        public DateTime ReceiveSmsDate { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (이메일)
        /// </summary>
        [JsonProperty("receive_email_info")]
        public string ReceiveEmailInfo { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (이메일) 최종 변경일자
        /// </summary>
        public DateTime ReceiveEmailDate { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (전화)
        /// </summary>
        [JsonProperty("receive_phone_info")]
        public string ReceivePhoneInfo { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 (전화) 최종 변경일자
        /// </summary>
        [JsonIgnore]
        public DateTime ReceivePhoneDate { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 저장 처리 변수 (Type)
        /// </summary>
        [JsonIgnore]
        public string ReceiveInfoType { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 저장 처리 변수 (Code)
        /// </summary>
        [JsonIgnore]
        public string ReceiveInfoCode { get; set; }

        /// <summary>
        /// 광고성 정보 수신 동의 저장 처리 변수 (Value)
        /// </summary>
        [JsonIgnore]
        public string ReceiveInfoValue { get; set; }

        /// <summary>
        /// 휴면 전환 조건
        /// </summary>
        [JsonProperty("info_validate_period")]
        public string InfoValidatePeriod { get; set; }

        /// <summary>
        /// 핸드폰 인증 버전
        /// </summary>
        [JsonIgnore]
        public string MobileAuthCD { get; set; }

        /// <summary>
        /// 사용자 메인 하이라이트 설정 값 (001:주요출품작,002:인스타그램,003:블로그)
        /// </summary>
        [JsonIgnore]
        public string MainHighlight { get; set; }

        /// <summary>
        /// 사용자 리스트 뷰 모드 설정 값 (001:페이징 방식,002:더보기 방식)
        /// </summary>
        [JsonIgnore]
        public string ListViewMode { get; set; }

        /// <summary>
        /// 마지막 로그인 일시
        /// </summary>
        [JsonIgnore]
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// 가입일자 
        /// </summary>
        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 마지막 수정일자
        /// </summary>
        [JsonIgnore]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ModDate { get; set; }

        /// <summary>
        /// 회원의 담당자 고유번호
        /// </summary>
        [JsonIgnore]
        public int MngUid { get; set; }

        /// <summary>
        /// 회원의 담당자 이름
        /// </summary>
        [JsonIgnore]
        public string MngName { get; set; }

        /// <summary>
        /// 회원의 담당자 이메일
        /// </summary>
        [JsonIgnore]
        public string MngEmail { get; set; }

        /// <summary>
        /// 회원의 담당자 휴대폰
        /// </summary>
        [JsonIgnore]
        public string MngHTel { get; set; }

        /// <summary>
        /// 회원의 담당자 내선번호
        /// </summary>
        [JsonIgnore]
        public string MngExTel { get; set; }

        /// <summary>
        /// 회원의 담당팀 이메일 (팀명|팀이메일^팀명|팀이메일)
        /// </summary>
        [JsonIgnore]
        public string MngTeamEmail { get; set; }

        /// <summary>
        /// 회원의 임직원 여부 (Y/N)
        /// </summary>
        [JsonIgnore]
        public string ManagerYN { get; set; }

        /// <summary>
        /// 회원의 관리기능 여부 (Y/N)
        /// </summary>
        [JsonIgnore]
        public string AdminYN { get; set; }

        /// <summary>
        /// 접속 토큰 (쿠키 처리)
        /// </summary>
        [JsonIgnore]
        public string Token { get; set; }

        /// <summary>
        /// 로그인 상태 유지
        /// </summary>
        [JsonIgnore]
        public string IsSaved { get; set; }

        //[JsonIgnore]
        //public DateTime RecognizeDate { get; set; }

        //[JsonIgnore]
        //public string RecognizeFlag { get; set; }

        //[JsonIgnore]
        //public string CompRegNum { get; set; }

        //[JsonIgnore]
        //public string Company { get; set; }

        //[JsonIgnore]
        //public string CorpRegNum { get; set; }

        //[JsonIgnore]
        //public string President { get; set; }

        /// <summary>
        /// 리스트 처리시 전체 결과 건수
        /// </summary>
        [JsonIgnore]
        public int TotalCount { get; set; }

        /// <summary>
        /// 마이그레이션 여부
        /// </summary>
        [JsonIgnore]
        public string Migration { get; set; }

        [JsonIgnore]
        public string Message1 { get; set; }

        [JsonIgnore]
        public string Message2 { get; set; }

        [JsonIgnore]
        public string Message3 { get; set; }

        [JsonIgnore]
        public string Message4 { get; set; }

        [JsonIgnore]
        public string Result { get; set; }

        /// <summary>
        /// 비밀번호 찾기 이후 비밀번호 변경 페이지로 이동 처리 여부
        /// </summary>
        [JsonIgnore]
        public string RedirectMyPageFlag { get; set; }

        /// <summary>
        /// 개인정보 변경 여부 (수정 시)
        /// </summary>
        [JsonProperty("info_req")]
        public string InfoReq { get; set; }

        /// <summary>
        /// 첨부파일정보 변경 여부 (수정 시)
        /// </summary>
        [JsonProperty("info_req_file")]
        public string InfoReqFile { get; set; }

        /// <summary>
        /// 비밀번호 변경 팝업 처리 여부
        /// </summary>
        [JsonProperty("pwd_noti")]
        public string PwdNoti { get; set; }

        /// <summary>
        /// 로그인시 비밀번호 실패 카운트
        /// </summary>
        [JsonProperty("pwd_fail_cnt")]
        public int PwdFailCnt { get; set; }

        /// <summary>
        /// 로그인 10회 실패 시 잠김 시간 (단위: 분)
        /// - tbl_common_code 테이블에 main_code 가 MEM_LOGIN_LOCK_TIME 로 설정
        /// </summary>
        [JsonProperty("pwd_fail_lock_time")]
        public int PwdFailLockTime { get; set; }

        /// <summary>
        /// 비밀번호 최근 변경 일자
        /// </summary>
        [JsonProperty("pwd_mod_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime PwdModDate { get; set; }

        /// <summary>
        /// 로그인 시 미인증상태로 인스타/네이버 블로그 조회 내역이 있으면 History 처리
        /// </summary>
        [JsonIgnore]
        public string HighlightRead { get; set; }

        [JsonProperty("excp_reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ExcpRegDate { get; set; }

        [JsonProperty("excp_typ_cd")]
        public string ExcpTypCD { get; set; }

        [JsonIgnore]
        public string Auth { get; set; }

        [JsonIgnore]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime AuthDate { get; set; }

        [JsonIgnore]
        public string AuthMobileNo { get; set; }

        [JsonIgnore]
        public string AuthCrdCd { get; set; }

        //[JsonIgnore]
        //public string DisplayAuthMobileNo => !string.IsNullOrWhiteSpace(AuthMobileNo) && AuthMobileNo.Length > 4 ? AuthMobileNo.Substring(AuthMobileNo.Length - 4) : string.Empty;

        [JsonIgnore]
        public string AuthMobileCoName { get; set; }
    }
}
