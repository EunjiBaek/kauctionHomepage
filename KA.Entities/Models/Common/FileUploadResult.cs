using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KA.Entities.Models.Common
{
    public class FileUploadResult
    {
        [JsonIgnore]
        public bool Result { get; set; }

        [JsonProperty("file_count")]
        public int FileCount { get; set; }

        [JsonProperty("upload_file_count")]
        public int UploadFileCount { get; set; }

        [JsonProperty("file_names")]
        public IEnumerable<string> FileNames { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public DateTime CreatedTimestamp { get; set; }

        [JsonIgnore]
        public DateTime UpdatedTimestamp { get; set; }

        [JsonIgnore]
        public IEnumerable<string> ContentTypes { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Names { get; set; }

        [JsonIgnore]
        public Object Etc { get; set; }
    }
}
