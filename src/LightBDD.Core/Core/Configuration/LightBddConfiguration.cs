using System;
using System.Collections.Concurrent;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// LightBDD feature configuration class allowing to configure and/or obtain LightBDD features configuration.
    /// </summary>
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, IFeatureConfiguration> _configuration = new ConcurrentDictionary<Type, IFeatureConfiguration>();

        /// <summary>
        /// Returns current feature configuration of requested type.
        /// If there was no configuration specified for given feature, the default configuration would be instantiated and returned for further customizations.
        /// </summary>
        /// <typeparam name="TConfiguration">Feature configuration type.</typeparam>
        /// <returns>Feature configuration instance.</returns>
        public TConfiguration Get<TConfiguration>() where TConfiguration : IFeatureConfiguration, new()
        {
            return (TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), t => new TConfiguration());
        }
    }
}
