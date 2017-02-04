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
    }
}