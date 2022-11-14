using KA.Entities.Attributes;
using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Auction
{
    public class AuctionWorkType
    {
        [JsonIgnore]
        public int Uid { get; set; }

        [JsonIgnore]
        public string AucKind { get; set; }

        [JsonIgnore]
        public int AucNum { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public string NameEn { get; set; }

        [JsonIgnore]
        public string DisplayName { get; set; }

        [JsonIgnore]
        public int Sort { get; set; }

        [JsonIgnore]
        public int RegUid { get; set; }

        [JsonIgnore]
        [JsonConverter(typeof(CustomFullDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonIgnore]

        public int UidOrg { get; set; }

        [JsonIgnore]
        public string LinkUrl { get; set; }
    }
}
