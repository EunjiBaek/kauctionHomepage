using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace KA.Web.Public.TagHelpers
{
    [HtmlTargetElement("breadcrumb", TagStructure = TagStructure.WithoutEndTag)]
    public class BreadcrumbTagHelper : TagHelper
    {
        public string Titles { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var index = 2;

            StringBuilder tags = new StringBuilder();
            tags.Append("<div class='container container-fixed-lg sm-p-l-0 sm-p-r-0'>");
            tags.Append("<div class='inner'>");
            tags.Append("<ol class='breadcrumb'>");
            tags.Append("<li class='breadcrumb-item depth-1'><a href='/'><i class='fas fa-home-lg-alt text-white'></i></a></li>");
            foreach (var item in Titles.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    tags.Append($"<li class='breadcrumb-item depth-{index++} {((Titles.Split('|').Length + 2).Equals(index) ? "active" : "")}'>{item}</li>");
                }
            }
            tags.Append("</ol>");
            tags.Append("</div>");
            tags.Append("</div><div class='inner h-px-40'></div>");

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "jumbotron bg-transparent");
            output.Content.SetHtmlContent(tags.ToString());
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
