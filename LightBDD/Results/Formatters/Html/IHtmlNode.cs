using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal interface IHtmlNode
    {
        HtmlTextWriter Write(HtmlTextWriter writer);
    }
}