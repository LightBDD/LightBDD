using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal static class Html
    {
        public static TagBuilder Checkbox()
        {
            return Tag(HtmlTextWriterTag.Input).Attribute(HtmlTextWriterAttribute.Type, "checkbox");
        }

        public static TagBuilder Radio()
        {
            return Tag(HtmlTextWriterTag.Input).Attribute(HtmlTextWriterAttribute.Type, "radio");
        }

        public static TagBuilder Tag(HtmlTextWriterTag tag)
        {
            return new TagBuilder(writer => writer.RenderBeginTag(tag));
        }

        public static TagBuilder Tag(Html5Tag tag)
        {
            return new TagBuilder(writer => writer.RenderBeginTag(tag.ToString()));
        }

        public static TextBuilder Text(string text)
        {
            return new TextBuilder(text);
        }

        public static IHtmlNode Br()
        {
            return Text("<br/>");
        }
    }
}