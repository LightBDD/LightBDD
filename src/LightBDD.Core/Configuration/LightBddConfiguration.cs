using System;
using System.Collections.Concurrent;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// LightBDD feature configuration class allowing to configure and/or obtain LightBDD features configuration.
    /// </summary>
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, FeatureConfiguration> _configuration = new();

        /// <summary>
        /// Returns current feature configuration of requested type.
        /// If there was no configuration specified for given feature, the default configuration would be instantiated and returned for further customizations.
        /// </summary>
        /// <typeparam name="TConfiguration">Feature configuration type.</typeparam>
        /// <returns>Feature configuration instance.</returns>
        public TConfiguration Get<TConfiguration>() where TConfiguration : FeatureConfiguration, new()
        {
            return (TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), _ => SealIfNeeded(new TConfiguration()));
        }

        private TConfiguration SealIfNeeded<TConfiguration>(TConfiguration config) where TConfiguration : FeatureConfiguration
        {
            if (IsSealed)
                config.Seal();
            return config;
        }

        /// <summary>
        /// Seals configuration making it immutable.
        /// It calls <see cref="FeatureConfiguration.Seal"/>() method on all configuration items that implements the <see cref="FeatureConfiguration"/> interface.
        /// Since this call, the <see cref="Get{TConfiguration}"/>() method will return only sealed configuration (current, and future default one).
        /// </summary>
        /// <returns>Self.</returns>
        public LightBddConfiguration Seal()
        {
            if (IsSealed)
                return this;
            IsSealed = true;
            foreach (var value in _configuration.Values)
                value.Seal();
            return this;
        }

        /// <summary>
        /// Returns true if configuration is sealed.
        /// </summary>
        public bool IsSealed { get; private set; }
    }
}
