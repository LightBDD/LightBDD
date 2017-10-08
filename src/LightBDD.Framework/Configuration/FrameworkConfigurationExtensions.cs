using LightBDD.Core.Configuration;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Formatting.Configuration;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Configuration
{
    /// <summary>
    /// Extensions allowing to apply framework default configuration.
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
                .ExecutionExtensionsConfiguration()
                .EnableStepCommenting();

            configuration
                .ValueFormattingConfiguration()
                .RegisterFrameworkDefaultGeneralFormatters();

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
    }
}
