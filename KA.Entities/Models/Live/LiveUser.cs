using System;
using System.Collections.Generic;
using System.Text;

namespace KA.Entities.Models.Live
{
    public class LiveUser
    {
        /// <summary>
        /// 경매종류
        /// </summary>
        public string auc_kind => "1";

        /// <summary>
        /// 경매번호
        /// </summary>
        public string auc_num { get; set; }

        /// <summary>
        /// 웹순번
        /// </summary>
        public int www_seq { get; set; }

        /// <summary>
        /// 세션키
        /// </summary>
        public string session_key { get; set; }

        /// <summary>
        /// 클라이언트값
        /// </summary>
        public string client_val { get; set; }

        /// <summary>
        /// 아이피값
        /// </summary>
        public string ip_val { get; set; }

        /// <summary>
        /// 캐시일자
        /// </summary>
        public string cache_date { get; set; }

        /// <summary>
        /// 연결종료일시
        /// </summary>
        public string disconnection_date { get; set; }

        /// <summary>
        /// 연결고유키
        /// </summary>
        public string connection_id { get; set; }

    }


    public class LiveStat
    {
        public int lot_num { get; set; }

        public Int64 bid_hst_seq { get; set; }

    }
}
