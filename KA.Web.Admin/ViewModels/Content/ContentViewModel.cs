using KA.Entities.Models.Content;
using System.Collections.Generic;

namespace KA.Web.Admin.ViewModels.Content
{
    public class ContentViewModel
    {
        public BoardDoc BoardDoc { get; set; }

        public IEnumerable<FileUpload> BoardAttaches { get; set; }
    }
}
