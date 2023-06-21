using System;
using System.IO;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters.Html;

namespace LightBDD.Framework.Reporting.Formatters
{
    /// <summary>
    /// Formats feature results as HTML.
    /// </summary>
    public class HtmlReportFormatter : IReportFormatter
    {
        private readonly HtmlReportFormatterOptions _options = new();
        /// <summary>
        /// Formats provided feature results and writes to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write formatted results to.</param>
        /// <param name="features">Feature results to format.</param>
        public void Format(Stream stream, params IFeatureResult[] features)
        {
            using var writer = new HtmlResultTextWriter(stream, features);
            writer.Write(_options);
        }

        /// <summary>
        /// Embeds <paramref name="cssContent"/> in the report HTML file, allowing to override default CSS styles.
        /// </summary>
        /// <param name="cssContent">CSS styles</param>
        public HtmlReportFormatter WithCustomCss(string cssContent)
        {
            _options.CssContent = cssContent;
            return this;
        }

        /// <summary>
        /// Embeds <paramref name="imageBody"/> image in <paramref name="mimeType"/> format into the report HTML file and uses it to override default LightBDD logo.
        /// </summary>
        /// <param name="mimeType">Image MIME type</param>
        /// <param name="imageBody">Image body</param>
        /// <returns></returns>
        public HtmlReportFormatter WithCustomLogo(string mimeType, byte[] imageBody)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
                throw new ArgumentException("MIME type needs to be specified", nameof(mimeType));
            if (imageBody == null)
                throw new ArgumentNullException(nameof(imageBody));

            _options.CustomLogo = Tuple.Create(mimeType, imageBody);
            return this;
        }

        /// <summary>
        /// Embeds <paramref name="imageBody"/> image in <paramref name="mimeType"/> format into the report HTML file and uses it to override default LightBDD favicon.
        /// </summary>
        /// <param name="mimeType">Favicon MIME type</param>
        /// <param name="imageBody">Favicon body</param>
        /// <returns></returns>
        public HtmlReportFormatter WithCustomFavicon(string mimeType, byte[] imageBody)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
                throw new ArgumentException("MIME type needs to be specified", nameof(mimeType));
            if (imageBody == null)
                throw new ArgumentNullException(nameof(imageBody));

            _options.CustomFavicon = Tuple.Create(mimeType, imageBody);
            return this;
        }
    }
}