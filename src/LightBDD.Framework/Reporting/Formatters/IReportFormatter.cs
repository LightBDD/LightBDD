using System.IO;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Reporting.Formatters
{
    /// <summary>
    /// Report formatter interface.
    /// </summary>
    public interface IReportFormatter
    {
        /// <summary>
        /// Formats provided test results and writes to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write formatted results to.</param>
        /// <param name="result">Test result to format.</param>
        void Format(Stream stream, ITestRunResult result);
    }
}