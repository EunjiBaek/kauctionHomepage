using System;
using System.Collections.Generic;
using System.Text;

namespace KA.Entities.Models.Live
{
    public class LiveRequest
    {
        public int mem_seq { get; set; }
        public string lang_cd { get; set; }
        public int auc_kind { get; set; }
        public int auc_num { get; set; }
        public int t_seq { get; set; }
        public int work_seq { get; set; }
        public int lot_num { get; set; }
        public int page_no { get; set; }
        public int page_size { get; set; }
        public int paddle_num { get; set; }
        public string bid_type_cd { get; set; }
        public string bid_stat_cd { get; set; }
        public decimal? bid_price { get; set; }
        public decimal? bid_increase_price { get; set; }
    }
}
