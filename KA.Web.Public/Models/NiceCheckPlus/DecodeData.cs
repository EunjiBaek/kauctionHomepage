namespace KA.Web.Public.Models.NiceCheckPlus
{
    public class DecodeData
    {
        public bool Success { get; internal set; }

        public string Code { get; internal set; }

        public string ErrorMessage { get; internal set; }

        public string AuthType { get; set; }

        public string Name { get; internal set; }

        public string MobileCompany { get; internal set; }

        public string MobileNumber { get; internal set; }

        public string BirthDate { get; internal set; }

        public string Gender { get; internal set; }

        public string DI { get; internal set; }

        public string CI { get; internal set; }

        public string RequestSequence { get; internal set; }

        public string ResponseSequence { get; internal set; }

        public string CipherTime { get; internal set; }
    }
}
