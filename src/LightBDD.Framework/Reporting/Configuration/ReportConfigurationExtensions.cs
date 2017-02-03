using LightBDD.Core.Configuration;

namespace LightBDD.Framework.Reporting.Configuration
{
    /// <summary>
    /// Configuration class allowing to retrieve report writers configurations for further customizations.
    /// See also: <seealso cref="LightBddConfiguration"/>.
    /// </summary>
    public static class ReportConfigurationExtensions
    {
        /// <summary>
        /// Retrieves <see cref="ReportWritersConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ReportWritersConfiguration ReportWritersConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ReportWritersConfiguration>();
        }
    }
}