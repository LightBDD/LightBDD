using LightBDD.Core.Configuration;

namespace LightBDD.Framework.Execution.Dependencies.Configuration
{
    /// <summary>
    /// Configuration class allowing to retrieve <see cref="DependencyContainerConfiguration"/> for further customizations.
    /// See also: <seealso cref="LightBddConfiguration"/>.
    /// </summary>
    public static class DependencyContainerConfigurationExtensions
    {
        /// <summary>
        /// Retrieves <see cref="DependencyContainerConfiguration"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static DependencyContainerConfiguration DependencyContainerConfiguration(this LightBddConfiguration configuration)
        {
            return configuration.Get<DependencyContainerConfiguration>();
        }
    }
}