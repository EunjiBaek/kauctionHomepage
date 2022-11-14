namespace KA.Entities.Models.Common
{
    public class RequestLog
    {
        public string UserID { get; set; }

        public string Path { get; set; }

        public string Referer { get; set; }

        public string Ip { get; set; }

        public string UserAgent { get; set; }

        public string Token { get; set; }
    }
}
