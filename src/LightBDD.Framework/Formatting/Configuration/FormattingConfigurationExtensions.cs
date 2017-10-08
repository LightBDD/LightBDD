using LightBDD.Core.Configuration;

namespace LightBDD.Framework.Formatting.Configuration
{
    /// <summary>
    /// Configuration class allowing to retrieve <see cref="NameFormatterConfiguration"/> for further customizations.
    /// See also: <seealso cref="LightBddConfiguration"/>.
    /// </summary>
    public static class FormattingConfigurationExtensions
    {
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
    }
}