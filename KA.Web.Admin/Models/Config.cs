using KA.Entities.Helpers;
using Microsoft.Extensions.Configuration;

namespace KA.Web.Admin.Models
{
    public class Config
    {
        #region # Root #

        /// <summary>
        /// 서비스 도메인 정보
        /// </summary>
        public static string ServiceDomain => Startup.Configuration.GetSection("Configs").GetSection("ServiceDomain").Value;

        /// <summary>
        /// 이전 서비스 도메인 정보 (라이브 경매)
        /// </summary>
        public static string PreviousServiceDomain => Startup.Configuration.GetSection("Configs").GetSection("PreviousServiceDomain").Value;

        /// <summary>
        /// 이미지 도메인 정보
        /// </summary>
        public static string ImageDomain => Startup.Configuration.GetSection("Configs").GetSection("ImageDomain").Value;

        /// <summary>
        /// KOffice 도메인 정보 
        /// </summary>
        public static string KofficeDomain => Startup.Configuration.GetSection("Configs").GetSection("KofficeDomain").Value;

        /// <summary>
        /// 홈페이지 도메인 정보
        /// </summary>
        public static string HomepageDomain => Startup.Configuration.GetSection("Configs").GetSection("HomepageDomain").Value;

        /// <summary>
        /// 컨텐츠 저장 모드 (SERVER/AWS)
        /// </summary>
        public static string ContentMode => Startup.Configuration.GetSection("Configs").GetSection("ContentMode").Value;

        /// <summary>
        /// 컨테츠 저장시 루트 경로 (SERVER 인 경우)
        /// </summary>
        public static string ContentPath => Startup.Configuration.GetSection("configs").GetSection("ContentPath").Value;

        /// <summary>
        /// 휴대폰 본인인증 모듈 (사이트코드)
        /// </summary>
        public static string NiceCode => Startup.Configuration.GetSection("NiceCheckPlus").GetSection("SiteCode").Value;

        /// <summary>
        /// 휴대폰 본인인증 모듈 (비밀번호)
        /// </summary>
        public static string NicePwd => Startup.Configuration.GetSection("NiceCheckPlus").GetSection("Password").Value;

        /// <summary>
        /// 관리자 로그인 페이지 활성화 여부
        /// </summary>
        public static string AdminLogin => Startup.Configuration.GetSection("configs").GetSection("AdminLogin").Value;

        #endregion

        #region # AWS #

        public class AWS
        {
            public static IConfigurationSection RootSection => Startup.Configuration.GetSection("AWS");

            /// <summary>
            /// AWS AccessKey
            /// </summary>
            public static string AccessKey => DESCryptoHelper.DESDecrypt(RootSection.GetSection("AccessKey").Value);

            /// <summary>
            /// AWS SecretKey
            /// </summary>
            public static string Secretkey => DESCryptoHelper.DESDecrypt(RootSection.GetSection("SecretKey").Value);

            /// <summary>
            /// AWS 홈페이지 버킷명
            /// </summary>
            public static string BucketNameHomepage => RootSection.GetSection("BucketNameHomepage").Value;

            /// <summary>
            /// AWS 케이오피스 버킷명
            /// </summary>
            public static string BucketNameKoffice => RootSection.GetSection("BucketNameKoffice").Value;
        }

        #endregion

        #region # OkCert3 #

        public class OkCert3
        {
            public static IConfigurationSection rootSection => Startup.Configuration.GetSection("OkCert3");

            /// <summary>
            /// KGB 로 부터 부여받은 회원사 코드 12자리
            /// </summary>
            public static string CP_CD => rootSection.GetSection("CP_CD").Value;

            /// <summary>
            /// 서버 타겟
            /// </summary>
            public static string TARGET => rootSection.GetSection("TARGET").Value;

            /// <summary>
            /// 운영 URL
            /// </summary>
            public static string MOBILE_POPUP_URL => rootSection.GetSection("MOBILE_POPUP_URL").Value;

            /// <summary>
            /// 휴대폰 인증 라이센스 경로
            /// </summary>
            public static string MOBILE_LICENSE => rootSection.GetSection("MOBILE_LICENSE").Value.Replace("{CP_CD}", OkCert3.CP_CD).Replace("{TARGET}", OkCert3.TARGET);

            /// <summary>
            /// 휴대폰 인증 송신 서비스명
            /// </summary>
            public static string MOBILE_SERVICE_START_NAME => rootSection.GetSection("MOBILE_SERVICE_START_NAME").Value;

            /// <summary>
            /// 휴대폰 인증 수신 서비스명
            /// </summary>
            public static string MOBILE_SERVICE_RESULT_NAME => rootSection.GetSection("MOBILE_SERVICE_RESULT_NAME").Value;

            /// <summary>
            /// 운영 URL
            /// </summary>
            public static string CARD_POPUP_URL => rootSection.GetSection("CARD_POPUP_URL").Value;

            /// <summary>
            /// 카드 인증 라이센스 경로
            /// </summary>
            public static string CARD_LICENSE => rootSection.GetSection("CARD_LICENSE").Value.Replace("{CP_CD}", OkCert3.CP_CD).Replace("{TARGET}", OkCert3.TARGET);

            /// <summary>
            /// 카드 인증 송신 서비스명
            /// </summary>
            public static string CARD_SERVICE_START_NAME => rootSection.GetSection("CARD_SERVICE_START_NAME").Value;

            /// <summary>
            /// 카드 인증 수신 서비스명
            /// </summary>
            public static string CARD_SERVICE_RESULT_NAME => rootSection.GetSection("CARD_SERVICE_RESULT_NAME").Value;
        }

        #endregion
    }
}
