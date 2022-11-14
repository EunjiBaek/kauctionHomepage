namespace KA.Web.Public.Models.NiceCheckPlus
{
    public class DecodeResult
    {
        public bool Success { get; internal set; }

        public int Status { get; internal set; }

        public string Message { get; internal set; }

        public string CipherDateTime { get; internal set; }

        public DecodeData Data { get; internal set; }
    }
}
