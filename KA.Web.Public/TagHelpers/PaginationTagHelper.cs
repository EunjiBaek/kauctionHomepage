using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace KA.Web.Public.TagHelpers
{
    [HtmlTargetElement("pagination", TagStructure = TagStructure.WithoutEndTag)]
    public class PaginationTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            StringBuilder tags = new StringBuilder();
            tags.Append("<div class='row justify-content-center'>");
            tags.Append("<div class='col-md-6 text-center'>");
            tags.Append("<div class='dataTables_paginate paging_full_numbers'><ul class='pagination'></ul></div>");
            tags.Append("</div>");
            tags.Append("</div>");
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "dataTables_wrapper m-b-30");
            output.Content.SetHtmlContent(tags.ToString());
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
