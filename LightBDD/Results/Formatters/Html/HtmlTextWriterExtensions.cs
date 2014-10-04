using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal static class HtmlTextWriterExtensions
    {
        public static HtmlTextWriter WriteTag(this HtmlTextWriter writer, IHtmlNode node)
        {
            return node.Write(writer);
        }
    }
}