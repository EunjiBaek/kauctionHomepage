using KA.Entities.Models.Main;
using System.Collections.Generic;

namespace KA.Web.Admin.ViewModels.Main
{
    public class BannerViewModel
    {
        public Banner Banner { get; set; }

        public IEnumerable<BannerButton> BannerButtons { get; set; }
    }
}
