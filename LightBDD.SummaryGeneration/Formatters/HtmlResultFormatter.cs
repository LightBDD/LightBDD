using System.IO;
using LightBDD.Core.Execution.Results;
using LightBDD.SummaryGeneration.Formatters.Html;

namespace LightBDD.SummaryGeneration.Formatters
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