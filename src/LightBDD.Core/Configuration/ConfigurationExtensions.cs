using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Extension class allowing to configure core parts of LightBDD.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Returns step type configuration.
        /// </summary>
        /// <param name="cfg">Configuration object.</param>
        /// <returns>Step type configuration.</returns>
        public static StepTypeConfiguration StepTypeConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<StepTypeConfiguration>();
        }

        public static LightBddConfiguration ConfigureCultureInfoProvider(this LightBddConfiguration cfg, Action<FeatureConfigurer<ICultureInfoProvider>> onConfigure) => cfg.ConfigureFeature(onConfigure);
        public static LightBddConfiguration ConfigureNameFormatter(this LightBddConfiguration cfg, Action<FeatureConfigurer<INameFormatter>> onConfigure) => cfg.ConfigureFeature(onConfigure);
        public static LightBddConfiguration ConfigureExceptionFormatter(this LightBddConfiguration cfg, Action<FeatureConfigurer<IExceptionFormatter>> onConfigure) => cfg.ConfigureFeature(onConfigure);
        public static LightBddConfiguration ConfigureFixtureFactory(this LightBddConfiguration cfg, Action<FeatureConfigurer<IFixtureFactory>> onConfigure) => cfg.ConfigureFeature(onConfigure);
        public static LightBddConfiguration ConfigureFileAttachmentsManager(this LightBddConfiguration cfg, Action<FeatureConfigurer<IFileAttachmentsManager>> onConfigure) => cfg.ConfigureFeature(onConfigure);
        /// <summary>
        /// Return execution extensions configuration.
        /// </summary>
        /// <param name="cfg">Configuration object.</param>
        /// <returns>Execution extensions configuration.</returns>
        public static ExecutionExtensionsConfiguration ExecutionExtensionsConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<ExecutionExtensionsConfiguration>();
        }

        /// <summary>
        /// Retrieves <see cref="ReportConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ReportConfiguration ReportConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ReportConfiguration>();
        }

        /// <summary>
        /// Retrieves <see cref="ValueFormattingConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ValueFormattingConfiguration ValueFormattingConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ValueFormattingConfiguration>();
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
        /// Retrieves <see cref="MetadataConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static MetadataConfiguration MetadataConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<MetadataConfiguration>();
        }
    }
}