using KA.Entities.Models.Common;
using KA.Entities.Models.Main;
using System.Collections.Generic;

namespace KA.Web.Admin.ViewModels.Main
{
    public class TopNoticeViewModel
    {
        public string Uid { get; set; }

        public Notice Notice { get; set; }

        public IEnumerable<Notice> Notices { get; set; }

        public IEnumerable<Menu> Menus { get; set; }
    }
}
