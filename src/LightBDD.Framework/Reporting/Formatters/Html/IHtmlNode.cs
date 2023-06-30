namespace LightBDD.Framework.Reporting.Formatters.Html
{
    internal interface IHtmlNode
    {
        HtmlTextWriter Write(HtmlTextWriter writer);
        bool IsEmpty();
    }
}