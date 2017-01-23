using System.IO;
using LightBDD.Core.Results;
using LightBDD.Reporting.Formatters.Html;

namespace LightBDD.Reporting.Formatters
{
    /// <summary>
    /// Formats feature results as HTML.
    /// </summary>
    public class HtmlResultFormatter : IResultFormatter
    {
        public void Format(Stream stream, params IFeatureResult[] features)
        {
            using (var writer = new HtmlResultTextWriter(stream, features))
                writer.Write();
        }
    }
}