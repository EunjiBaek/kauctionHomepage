using Newtonsoft.Json;

namespace KA.Entities.Models.Common
{
    public class ApiBoardResult
    {
        public ApiBoardResult(object info, object data, int recordsTotal, int recordsFiltered, int draw = 1)
        {
            Info = info;
            Data = data;
            RecordsTotal = recordsTotal;
            RecordsFiltered = recordsFiltered;
            Draw = draw;
        }

        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty("recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("info")]
        public object Info { get; set; }
        [JsonProperty("draw")]
        public int Draw { get; set; }
    }
}
