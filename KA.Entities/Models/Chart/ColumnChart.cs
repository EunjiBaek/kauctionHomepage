using Newtonsoft.Json;

namespace KA.Entities.Models.Chart
{
    public class ColumnChart
    {
        [JsonProperty("categories")]
        public string[] Categories { get; set; }

        [JsonProperty("series")]
        public Series[] Series { get; set; }
    }

    public class Series
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public int[] Data { get; set; }
    }
}
