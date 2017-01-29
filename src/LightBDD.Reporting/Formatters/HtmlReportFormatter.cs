using System.IO;
using LightBDD.Core.Results;
using LightBDD.Reporting.Formatters.Html;

namespace LightBDD.Reporting.Formatters
{
    /// <summary>
    /// Formats feature results as HTML.
    /// </summary>
    public class HtmlReportFormatter : IReportFormatter
    {
        /// <summary>
        /// Formats provided feature results and writes to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write formatted results to.</param>
        /// <param name="features">Feature results to format.</param>
        public void Format(Stream stream, params IFeatureResult[] features)
        {
            using (var writer = new HtmlResultTextWriter(stream, features))
                writer.Write();
        }
    }
}