using KA.Entities.Helpers;
using Microsoft.Extensions.Configuration;

namespace KA.Web.Public.Models
{
    public static class Config
    {
        #region # Root #

        /// <summary>
        /// 서비스 도메인 정보
        /// </summary>
        public static string ServiceDomain => Startup.Config.GetSection("Configs").GetSection("ServiceDomain").Value;

        /// <summary>
        /// 이전 서비스 도메인 정보 (라이브 경매)
        /// </summary>
        public static string PreviousServiceDomain => Startup.Config.GetSection("Configs").GetSection("PreviousServiceDomain").Value;

        /// <summary>
        /// 케이오피스 도메인 정보
        /// </summary>
        public static string KofficeDomain => Startup.Config.GetSection("configs").GetSection("KofficeDomain").Value;

        /// <summary>
        /// 마크애니 문서보안 솔루션 도메인 정보
        /// </summary>
        public static string PageSaferServiceUrl => Startup.Config.GetSection("Configs").GetSection("PageSaferServiceUrl").Value;

        /// <summary>
        /// 마크애니 문서보안 페이지 연동 URL
        /// </summary>
        public static string PageSaferApiUrl => Startup.Config.GetSection("configs").GetSection("PageSaferApiUrl").Value;

        /// <summary>
        /// 이미지 도메인 정보
        /// </summary>
        public static string ImageDomain => Startup.Config.GetSection("Configs").GetSection("ImageDomain").Value;

        /// <summary>
        /// 이미지 도메인 정보
        /// </summary>
        public static string ImageDomainKoffice => Startup.Config.GetSection("Configs").GetSection("ImageDomainKoffice").Value;

        /// <summary>
        /// 컨텐츠 저장 모드 (SERVER/AWS)
        /// </summary>
        public static string ContentMode => Startup.Config.GetSection("Configs").GetSection("ContentMode").Value;

        /// <summary>
        /// 컨테츠 저장시 루트 경로 (SERVER 인 경우)
        /// </summary>
        public static string ContentPath => Startup.Config.GetSection("configs").GetSection("ContentPath").Value;

        /// <summary>
        /// GoogleAnalytics 사용 유무
        /// </summary>
        public static string UseGoogleAnalytics => Startup.Config.GetSection("configs").GetSection("UseGoogleAnalytics").Value;

        /// <summary>
        /// GoogleTagManager 사용 유무
        /// </summary>
        public static string UseGoogleTagManager => Startup.Config.GetSection("configs").GetSection("UseGoogleTagManager").Value;

        /// <summary>
        /// Logger 사용 유무
        /// </summary>
        public static string UseLogger => Startup.Config.GetSection("configs").GetSection("UseLogger").Value;

        /// <summary>
        /// 알림 메일 테스트 유무 (Y인 경우 Address 를 수신자로 지정)
        /// </summary>
        public static string MailTestModeFlag => Startup.Config.GetSection("configs").GetSection("MailTestModeFlag").Value;

        /// <summary>
        /// 알림 메일 테스트 시 수신 계정
        /// </summary>
        public static string MailTestModeAddress => Startup.Config.GetSection("configs").GetSection("MailTestModeAddress").Value;

        /// <summary>
        /// 회원 탈퇴 신규기능 적용 여부
        /// </summary>
        public static string RetireKoffice => Startup.Config.GetSection("configs").GetSection("RetireKoffice").Value;

        /// <summary>
        /// 회원 가입페이지 모드 (2: 신규)
        /// </summary>
        public static string JoinMode => Startup.Config.GetSection("configs").GetSection("JoinMode").Value;

        /// <summary>
        /// 사업자등록번호 API URL
        /// </summary>
        public static string BusinessNumCheckAPiUrl => Startup.Config.GetSection("configs").GetSection("BusinessNumCheckApiUrl").Value;

        /// <summary>
        /// 응찰안내 유투브 URL
        /// </summary>
        public static string BidGuideYoutubeUrl => Startup.Config.GetSection("configs").GetSection("BidGuideYoutubeUrl").Value;

        /// <summary>
        /// 위탁안내 유투브 URL
        /// </summary>
        public static string ConsignGuideYoutubeUrl => Startup.Config.GetSection("configs").GetSection("ConsignGuideYoutubeUrl").Value;

        #endregion

        #region # AWS #

        public class AWS
        {
            public static IConfigurationSection RootSection => Startup.Config.GetSection("AWS");

            /// <summary>
            /// AWS AccessKey (s3-kauction, s3-kauction-www, s3-kauction-release, s3-kauction-www-release)
            /// </summary>
            public static string AccessKey => DESCryptoHelper.DESDecrypt(RootSection.GetSection("AccessKey").Value);

            /// <summary>
            /// AWS SecretKey (s3-kauction, s3-kauction-www, s3-kauction-release, s3-kauction-www-release)
            /// </summary>
            public static string Secretkey => DESCryptoHelper.DESDecrypt(RootSection.GetSection("SecretKey").Value);

            /// <summary>
            /// AWS AccessKey Private (s3-kauction-p, s3-kauction-p-release)
            /// </summary>
            public static string AccessKeyPrivate => DESCryptoHelper.DESDecrypt(RootSection.GetSection("AccessKeyPrivate").Value);

            /// <summary>
            /// AWS SecretKey Private (s3-kauction-p, s3-kauction-p-release)
            /// </summary>
            public static string SecretkeyPrivate => DESCryptoHelper.DESDecrypt(RootSection.GetSection("SecretKeyPrivate").Value);

            /// <summary>
            /// AWS 홈페이지 버킷명
            /// </summary>
            public static string BucketNameHomepage => RootSection.GetSection("BucketNameHomepage").Value;

            /// <summary>
            /// AWS 케이오피스 버킷명
            /// </summary>
            public static string BucketNameKoffice => RootSection.GetSection("BucketNameKoffice").Value;

            /// <summary>
            /// AWS 케이오피스 (개인정보파일) 버킷명
            /// </summary>
            public static string BucketNameKofficePrivate => RootSection.GetSection("BucketNameKofficePrivate").Value;
        }

        #endregion

        #region # OkCert3 #

        public class OkCert3
        {
            public static IConfigurationSection RootSection => Startup.Config.GetSection("OkCert3");

            /// <summary>
            /// KGB 로 부터 부여받은 회원사 코드 12자리
            /// </summary>
            public static string CP_CD => DESCryptoHelper.DESDecrypt(RootSection.GetSection("CP_CD").Value);

            /// <summary>
            /// 서버 타겟
            /// </summary>
            public static string TARGET => RootSection.GetSection("TARGET").Value;

            /// <summary>
            /// 운영 URL
            /// </summary>
            public static string MOBILE_POPUP_URL => RootSection.GetSection("MOBILE_POPUP_URL").Value;

            /// <summary>
            /// 휴대폰 인증 라이센스 경로
            /// </summary>
            public static string MOBILE_LICENSE => RootSection.GetSection("MOBILE_LICENSE").Value.Replace("{CP_CD}", OkCert3.CP_CD).Replace("{TARGET}", OkCert3.TARGET);

            /// <summary>
            /// 휴대폰 인증 송신 서비스명
            /// </summary>
            public static string MOBILE_SERVICE_START_NAME => RootSection.GetSection("MOBILE_SERVICE_START_NAME").Value;

            /// <summary>
            /// 휴대폰 인증 수신 서비스명
            /// </summary>
            public static string MOBILE_SERVICE_RESULT_NAME => RootSection.GetSection("MOBILE_SERVICE_RESULT_NAME").Value;

            /// <summary>
            /// 운영 URL
            /// </summary>
            public static string CARD_POPUP_URL => RootSection.GetSection("CARD_POPUP_URL").Value;

            /// <summary>
            /// 카드 인증 라이센스 경로
            /// </summary>
            public static string CARD_LICENSE => RootSection.GetSection("CARD_LICENSE").Value.Replace("{CP_CD}", OkCert3.CP_CD).Replace("{TARGET}", OkCert3.TARGET);

            /// <summary>
            /// 카드 인증 송신 서비스명
            /// </summary>
            public static string CARD_SERVICE_START_NAME => RootSection.GetSection("CARD_SERVICE_START_NAME").Value;

            /// <summary>
            /// 카드 인증 수신 서비스명
            /// </summary>
            public static string CARD_SERVICE_RESULT_NAME => RootSection.GetSection("CARD_SERVICE_RESULT_NAME").Value;

            /// <summary>
            /// 인코딩 모드
            /// </summary>
            public static string ENCODING => RootSection.GetSection("ENCODING").Value;
        }

        #endregion
    }
}
