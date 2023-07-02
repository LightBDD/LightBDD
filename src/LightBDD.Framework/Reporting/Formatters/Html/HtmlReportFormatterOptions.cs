using System;

namespace LightBDD.Framework.Reporting.Formatters.Html;

internal class HtmlReportFormatterOptions
{
    public string CssContent { get; set; }
    public Tuple<string, byte[]> CustomLogo { get; set; }
    public Tuple<string, byte[]> CustomFavicon { get; set; }
}