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

        /// <summary>
        /// Returns culture info provider configuration.
        /// </summary>
        /// <param name="cfg">Configuration object.</param>
        /// <returns>Culture info provider configuration.</returns>
        public static CultureInfoProviderConfiguration CultureInfoProviderConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<CultureInfoProviderConfiguration>();
        }

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
        /// Returns exception handling configuration.
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static ExceptionHandlingConfiguration ExceptionHandlingConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<ExceptionHandlingConfiguration>();
        }

        /// <summary>
        /// Retrieves <see cref="DependencyContainerConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static DependencyContainerConfiguration DependencyContainerConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<DependencyContainerConfiguration>();
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
        /// Retrieves <see cref="NameFormatterConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static NameFormatterConfiguration NameFormatterConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<NameFormatterConfiguration>();
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
    }
}