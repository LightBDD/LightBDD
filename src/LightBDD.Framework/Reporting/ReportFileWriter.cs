using System.IO;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.Framework.Reporting.Implementation;

namespace LightBDD.Framework.Reporting
{
    /// <summary>
    /// Summary file writer class allows to save feature results by using associated result formatter and output path.
    /// </summary>
    public class ReportFileWriter : IReportWriter
    {
        private readonly ReportFormattablePath _path;

        /// <summary>
        /// Returns configured formatter.
        /// </summary>
        public IReportFormatter Formatter { get; }
        /// <summary>
        /// Returns configured output path.
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// Returns output path resolved to a full path.
        /// </summary>
        public string FullOutputPath => _path.FormattablePath;

        /// <summary>
        /// Constructor allowing to create ReportFileWriter with associated result formatter and output path.
        /// Please note that full output path is being resolved at time when constructor is called, not when results are saved, so any relative paths will be resolved at the construction of this class.
        /// </summary>
        /// <param name="formatter">Report formatter.</param>
        /// <param name="outputPath">Output path. If starts with <c>~</c>, it would be resolved to <c>AppContext.BaseDirectory</c>. It can contain string.Format() like parameters of {name:format} syntax.
        /// This constructor uses default <see cref="ReportPathFormatter"/> to format these parameters. See <see cref="ReportPathFormatter.CreateDefault"/>() for more details on available parameter types.</param>
        public ReportFileWriter(IReportFormatter formatter, string outputPath)
            : this(formatter, outputPath, ReportPathFormatter.CreateDefault()) { }

        /// <summary>
        /// Constructor allowing to create ReportFileWriter with associated result formatter, output path and path formatter.
        /// Please note that full output path is being resolved at time when constructor is called, not when results are saved, so any relative paths will be resolved at the construction of this class.
        /// </summary>
        /// <param name="formatter">Report formatter.</param>
        /// <param name="outputPath">Output path. If starts with <c>~</c>, it would be resolved to <c>AppContext.BaseDirectory</c>. It can contain string.Format() like parameters of {name:format} syntax.</param>
        /// <param name="pathFormatter"><see cref="ReportPathFormatter"/> instance used to format <paramref name="outputPath"/>.</param>
        public ReportFileWriter(IReportFormatter formatter, string outputPath, ReportPathFormatter pathFormatter)
        {
            OutputPath = outputPath;
            _path = pathFormatter.ToFormattablePath(outputPath);
            Formatter = formatter;
        }

        /// <summary>
        /// Saves formatted feature <c>results</c> to file specified in constructor.
        /// If output path refers to directory which does not exist, it will be created.
        /// </summary>
        /// <param name="results">Results to save.</param>
        public void Save(params IFeatureResult[] results)
        {
            var outputPath = _path.Resolve(results);
            FilePathHelper.EnsureOutputDirectoryExists(outputPath);
            using (var stream = File.Create(outputPath))
                Formatter.Format(stream, results);
        }
    }
}