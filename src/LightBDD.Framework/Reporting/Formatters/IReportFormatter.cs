using System.IO;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Formatters
{
    /// <summary>
    /// Report formatter interface.
    /// </summary>
    public interface IReportFormatter
    {
        /// <summary>
        /// Formats provided feature results and writes to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write formatted results to.</param>
        /// <param name="features">Feature results to format.</param>
        void Format(Stream stream, params IFeatureResult[] features);
    }
}