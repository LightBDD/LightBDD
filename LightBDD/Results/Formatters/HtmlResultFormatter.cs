using System.IO;
using System.Text;
using LightBDD.Results.Formatters.Html;

namespace LightBDD.Results.Formatters
{
    /// <summary>
    /// Formats feature results as HTML.
    /// </summary>
    public class HtmlResultFormatter : IResultFormatter
    {
        /// <summary>
        /// Formats feature results.
        /// </summary>
        /// <param name="features">Features to format.</param>
        public string Format(params IFeatureResult[] features)
        {
            using (var memory = new MemoryStream())
            using (var writer = new HtmlResultTextWriter(memory,features))
            {
                writer.Write();
                return Encoding.Default.GetString(memory.ToArray());
            }
        }
    }
}