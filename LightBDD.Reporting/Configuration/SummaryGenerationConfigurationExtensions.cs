using LightBDD.Core.Configuration;

namespace LightBDD.Reporting.Configuration
{
    public static class SummaryGenerationConfigurationExtensions
    {
        public static SummaryWritersConfiguration SummaryWritersConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<SummaryWritersConfiguration>();
        }
    }
}