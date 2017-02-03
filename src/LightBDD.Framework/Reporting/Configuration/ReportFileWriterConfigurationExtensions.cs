using LightBDD.Framework.Reporting.Formatters;

namespace LightBDD.Framework.Reporting.Configuration
{
    /// <summary>
    /// Extension class for <see cref="ReportWritersConfiguration"/> simplifying configuration with <see cref="ReportFileWriter"/> instances.
    /// </summary>
    public static class ReportFileWriterConfigurationExtensions
    {
        /// <summary>
        /// Adds <see cref="ReportFileWriter"/> instance configured to format report with <typeparamref name="TFormatter"/> and write it to <paramref name="outputPath"/>.
        /// </summary>
        /// <typeparam name="TFormatter">Type of report formatter.</typeparam>
        /// <param name="configuration">Configuration.</param>
        /// <param name="outputPath">Output path for the report.</param>
        /// <returns>Configuration.</returns>
        public static ReportWritersConfiguration AddFileWriter<TFormatter>(this ReportWritersConfiguration configuration, string outputPath) where TFormatter : IReportFormatter, new()
        {
            return configuration.Add(new ReportFileWriter(new TFormatter(), outputPath));
        }
    }
}