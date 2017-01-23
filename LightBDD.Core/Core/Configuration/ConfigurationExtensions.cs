namespace LightBDD.Core.Configuration
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
    }
}