namespace LightBDD.Configuration
{
    public static class ConfigurationExtensions
    {
        public static StepTypeConfiguration StepTypeConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<StepTypeConfiguration>();
        }

        public static CultureInfoProviderConfiguration CultureInfoProviderConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<CultureInfoProviderConfiguration>();
        }

        public static ExecutionExtensionsConfiguration ExecutionExtensionsConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<ExecutionExtensionsConfiguration>();
        }

        public static FeatureProgressNotifierConfiguration FeatureProgressNotifierConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<FeatureProgressNotifierConfiguration>();
        }

        public static NameFormatterConfiguration NameFormatterConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<NameFormatterConfiguration>();
        }

        public static ScenarioProgressNotifierConfiguration ScenarioProgressNotifierConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<ScenarioProgressNotifierConfiguration>();
        }
    }
}