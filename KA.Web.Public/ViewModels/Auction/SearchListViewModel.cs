using KA.Entities.Models.Auction;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KA.Web.Public.ViewModels.Auction
{
    public class SearchListViewModel
    {
        [JsonProperty("search_result")]
        public string SearchResult { get; set; }

        [JsonProperty("list")]
        public IEnumerable<AuctionWork> AuctionWorks { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
