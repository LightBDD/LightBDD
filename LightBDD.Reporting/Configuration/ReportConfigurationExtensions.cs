using LightBDD.Core.Configuration;

namespace LightBDD.Reporting.Configuration
{
    public static class ReportConfigurationExtensions
    {
        public static ReportWritersConfiguration ReportWritersConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<ReportWritersConfiguration>();
        }
    }
}