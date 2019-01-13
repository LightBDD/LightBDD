using System.IO;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Formatting;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;

namespace LightBDD.Framework.Configuration
{
    /// <summary>
    /// Extensions allowing to apply and configure framework default configuration.
    /// </summary>
    public static class FrameworkConfigurationExtensions
    {
        /// <summary>
        /// Applies framework default configuration.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <returns><paramref name="configuration"/>.</returns>
        public static LightBddConfiguration WithFrameworkDefaults(this LightBddConfiguration configuration)
        {
            configuration
                .ValueFormattingConfiguration()
                .RegisterFrameworkDefaultGeneralFormatters();

            configuration
                .ReportWritersConfiguration()
                .RegisterFrameworkDefaultReportWriters();

            configuration
                .NameFormatterConfiguration()
                .UpdateFormatter(DefaultNameFormatter.Instance);

            return configuration;
        }

        /// <summary>
        /// Applies framework default general formatters.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <returns><paramref name="configuration"/>.</returns>
        public static ValueFormattingConfiguration RegisterFrameworkDefaultGeneralFormatters(this ValueFormattingConfiguration configuration)
        {
            return configuration
                 .RegisterGeneral(new DictionaryFormatter())
                 .RegisterGeneral(new CollectionFormatter());
        }

        /// <summary>
        /// Applies default report generators to generate <c>~\\Reports\\FeaturesReport.html</c>(Win) <c>~/Reports/FeaturesReport.html</c>(Unix) reports.
        /// </summary>
        public static ReportWritersConfiguration RegisterFrameworkDefaultReportWriters(this ReportWritersConfiguration configuration)
        {
            return configuration.Add(new ReportFileWriter(new HtmlReportFormatter(), "~" + Path.DirectorySeparatorChar + "Reports" + Path.DirectorySeparatorChar + "FeaturesReport.html"));
        }

        /// <summary>
        /// Retrieves <see cref="FeatureProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static FeatureProgressNotifierConfiguration FeatureProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<FeatureProgressNotifierConfiguration>();
        }
        /// <summary>
        /// Retrieves <see cref="ScenarioProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ScenarioProgressNotifierConfiguration ScenarioProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ScenarioProgressNotifierConfiguration>();
        }

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
