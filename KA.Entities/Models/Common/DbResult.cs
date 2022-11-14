using System;

namespace KA.Entities.Models.Common
{
    public class DbResult
    {
        public string Result { get; set; }

        public int Target { get; set; }

        public int ErrorNumber { get; set; }

        public int ErrorServerity { get; set; }

        public int ErrorState { get; set; }

        public string ErrorProcedure { get; set; }

        public int ErrorLine { get; set; }

        public string ErrorMessage { get; set; }

        public string Etc { get; set; }

        public DateTime RegDate { get; set; }

        public string Code { get; set; }

        public string Msg { get; set; }

        public string RsltCD { get; set; }

        public string RsltMsg { get; set; }

        public string RsltReqStatCD { get; set; }
    }
}
