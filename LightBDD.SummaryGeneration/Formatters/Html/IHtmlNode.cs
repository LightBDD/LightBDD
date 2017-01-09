namespace LightBDD.SummaryGeneration.Formatters.Html
{
    internal interface IHtmlNode
    {
        HtmlTextWriter Write(HtmlTextWriter writer);
    }
}