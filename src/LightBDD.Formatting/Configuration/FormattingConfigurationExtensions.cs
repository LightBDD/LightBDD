using LightBDD.Core.Configuration;

namespace LightBDD.Formatting.Configuration
{
    public static class FormattingConfigurationExtensions
    {

        public static NameFormatterConfiguration NameFormatterConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<NameFormatterConfiguration>();
        }
    }
}