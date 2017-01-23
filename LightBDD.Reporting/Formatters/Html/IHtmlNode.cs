namespace LightBDD.Reporting.Formatters.Html
{
    internal interface IHtmlNode
    {
        HtmlTextWriter Write(HtmlTextWriter writer);
    }
}