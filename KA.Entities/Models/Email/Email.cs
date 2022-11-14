namespace KA.Entities.Models.Email
{
    /// <summary>
    /// 2022.05.17 :: IsKor 변수 추가 - 양식변수 국문/영문 분리 경우 발생
    /// </summary>
    public class Email
    {
        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string ToEmail { get; set; }

        public string ToName { get; set; }

        public string[] AddToEmail { get; set; }

        public string[] AddToName { get; set; }

        public string CcEmail { get; set; }

        public string CcName { get; set; }

        public string[] AddCcEmail { get; set; }

        public string[] AddCcName { get; set; }

        public string SubJect { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public bool IsKor { get; set; }

        public string Type { get; set; }

        public string Etc1 { get; set; }

        public string Etc2 { get; set; }

        public string Etc3 { get; set; }

        public string Etc4 { get; set; }

        public string Etc5 { get; set; }

        public string Footer { get; set; }

        public string ServiceDomain { get; set; }

        public string Site { get; set; }

        public int RegUid { get; set; }

        public string Result { get; set; }
        
        public string ErrorMessage { get; set; }

        public string ErrorStacktrace { get; set; }
    }
}
