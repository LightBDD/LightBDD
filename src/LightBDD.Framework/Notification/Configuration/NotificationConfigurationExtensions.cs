using LightBDD.Core.Configuration;

namespace LightBDD.Notification.Configuration
{
    /// <summary>
    /// Configuration class allowing to retrieve progress notification configurations for further customizations.
    /// See also: <seealso cref="LightBddConfiguration"/>.
    /// </summary>
    public static class NotificationConfigurationExtensions
    {
        /// <summary>
        /// Retrieves <see cref="FeatureProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static FeatureProgressNotifierConfiguration FeatureProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<FeatureProgressNotifierConfiguration>();
        }
        /// <summary>
        /// Retrieves <see cref="ScenarioProgressNotifierConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ScenarioProgressNotifierConfiguration ScenarioProgressNotifierConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<ScenarioProgressNotifierConfiguration>();
        }
    }
}