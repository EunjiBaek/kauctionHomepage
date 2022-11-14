using System.Collections.Generic;

namespace KA.Entities.Models.Common
{
    public class FileUploadInfo
    {
        public string ServerType { get; set; }

        public string Target { get; set; }

        public string FilePath { get; set; }

        public List<string> FileNames { get; set; }

        public string AccessKey { get; set; }

        public string AccessKeyPrivate { get; set; }

        public string SecretKey { get; set; }

        public string SecretKeyPrivate { get; set; }

        public string BucketNameHomepage { get; set; }

        public string BucketNameHomepatePrivate { get; set; }

        public string BucketNameKoffice { get; set; }

        public string BucketNameKofficePrivate { get; set; }

        public string Etc { get; set; }

        public string Etc2 { get; set; }

        public string Etc3 { get; set; }

        public bool IsKofficeCopy { get; set; }
    }
}
