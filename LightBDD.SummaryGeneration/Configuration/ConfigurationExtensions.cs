using LightBDD.Configuration;

namespace LightBDD.SummaryGeneration.Configuration
{
    public static class ConfigurationExtensions
    {
        public static SummaryWritersConfiguration SummaryWritersConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<SummaryWritersConfiguration>();
        }
    }
}