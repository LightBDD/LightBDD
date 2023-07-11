﻿using System;
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
                .ReportConfiguration()
                .RegisterFrameworkDefaultReportWriters()
                .RegisterDefaultFileAttachmentManager();

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
        public static ReportConfiguration RegisterFrameworkDefaultReportWriters(this ReportConfiguration configuration)
        {
            return configuration
                .Add(new FileReportGenerator(new HtmlReportFormatter(), $"~{Path.DirectorySeparatorChar}Reports{Path.DirectorySeparatorChar}FeaturesReport.html"));
        }

        /// <summary>
        /// Applies default file attachment manager configuration to generate attachments in <c>~\\Reports\</c>(Win) <c>~/Reports/</c>(Unix) directory.
        /// </summary>
        public static ReportConfiguration RegisterDefaultFileAttachmentManager(this ReportConfiguration configuration)
        {
            return configuration
                .UpdateFileAttachmentsManager(new FileAttachmentsManager($"~{Path.DirectorySeparatorChar}Reports"));
        }

        /// <summary>
        /// Retrieves <see cref="FeatureProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        [Obsolete]
        public static FeatureProgressNotifierConfiguration FeatureProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<FeatureProgressNotifierConfiguration>();
        }

        /// <summary>
        /// Retrieves <see cref="ScenarioProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        [Obsolete]
        public static ScenarioProgressNotifierConfiguration ScenarioProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ScenarioProgressNotifierConfiguration>();
        }

        /// <summary>
        /// Retrieves <see cref="ProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ProgressNotifierConfiguration ProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ProgressNotifierConfiguration>();
        }

        /// <summary>
        /// Adds <see cref="FileReportGenerator"/> instance configured to format report with <typeparamref name="TFormatter"/> and write it to <paramref name="outputPath"/>.
        /// </summary>
        /// <typeparam name="TFormatter">Type of report formatter.</typeparam>
        /// <param name="configuration">Configuration.</param>
        /// <param name="outputPath">Output path for the report.</param>
        /// <returns>Configuration.</returns>
        public static ReportConfiguration AddFileReport<TFormatter>(this ReportConfiguration configuration, string outputPath) where TFormatter : IReportFormatter, new()
        {
            return AddFileReport<TFormatter>(configuration, outputPath, null);
        }

        /// <summary>
        /// Adds <see cref="FileReportGenerator"/> instance configured to format report with <typeparamref name="TFormatter"/> and write it to <paramref name="outputPath"/>.
        /// </summary>
        /// <typeparam name="TFormatter">Type of report formatter.</typeparam>
        /// <param name="configuration">Configuration.</param>
        /// <param name="outputPath">Output path for the report.</param>
        /// <param name="onConfigure">Action to configure the <typeparamref name="TFormatter"/> instance.</param>
        /// <returns>Configuration.</returns>
        public static ReportConfiguration AddFileReport<TFormatter>(this ReportConfiguration configuration, string outputPath, Action<TFormatter> onConfigure) where TFormatter : IReportFormatter, new()
        {
            var formatter = new TFormatter();
            onConfigure?.Invoke(formatter);
            return configuration.Add(new FileReportGenerator(formatter, outputPath));
        }

        /// <summary>
        /// Retrieves <see cref="ObjectTreeConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ObjectTreeConfiguration ObjectTreeConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ObjectTreeConfiguration>();
        }
    }
}
