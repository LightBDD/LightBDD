using System.IO;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Formatting;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;

namespace LightBDD.Framework.Configuration
{
    /// <summary>
    /// Extensions allowing to apply framework default configuration.
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
    }
}
