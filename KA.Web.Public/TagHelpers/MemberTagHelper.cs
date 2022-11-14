using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KA.Web.Public.TagHelpers
{
    [HtmlTargetElement("member", TagStructure = TagStructure.WithoutEndTag)]
    public class MemberTagHelper : TagHelper
    {
        public string Label { get; set; } = "";

        public string Id { get; set; } = "";

        public string Type { get; set; } = "text";

        public string RootClass { get; set; } = "";

        public string InputClass { get; set; } = "";

        public string Hint { get; set; } = "";

        public string Value { get; set; } = "";

        public string Disabled { get; set; } = "";

        public string Msg { get; set; } = "";

        public string Style { get; set; } = "";

        public string Param { get; set; } = "";

        public string MaxLength { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Hint = string.IsNullOrWhiteSpace(Hint) ? "" : $"<span class='m-l-10 small-text hint-text'>{Hint}</span>";
            Msg = string.IsNullOrWhiteSpace(Msg) ? "" : $"data-msg='{Msg}'";
            Style = string.IsNullOrWhiteSpace(Style) ? "" : $"style='{Style}'";
            MaxLength = string.IsNullOrWhiteSpace(MaxLength) ? "" : $"maxlength='{MaxLength}'";

            output.TagName = "div";
            output.Content.SetHtmlContent($@"<label>{Label}{Hint}</label><input id={Id} type='{Type}' class='form-control {InputClass}' value='{Value}' param='{Param}' {Disabled} {Msg} {Style} {MaxLength} />");
            output.Attributes.SetAttribute("class", "form-group form-group-default" + (string.IsNullOrWhiteSpace(RootClass) ? "" : " " + RootClass));
            if (!string.IsNullOrWhiteSpace(Id))
            {
                output.Attributes.SetAttribute("id", "fg-" + Id);
            }
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
