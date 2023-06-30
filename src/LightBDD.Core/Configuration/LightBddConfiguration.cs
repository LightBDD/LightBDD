using System;
using System.Collections.Concurrent;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// LightBDD feature configuration class allowing to configure and/or obtain LightBDD features configuration.
    /// </summary>
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, IFeatureConfiguration> _configuration = new();

        /// <summary>
        /// Returns current feature configuration of requested type.
        /// If there was no configuration specified for given feature, the default configuration would be instantiated and returned for further customizations.
        /// </summary>
        /// <typeparam name="TConfiguration">Feature configuration type.</typeparam>
        /// <returns>Feature configuration instance.</returns>
        public TConfiguration Get<TConfiguration>() where TConfiguration : IFeatureConfiguration, new()
        {
            return SealIfNeeded((TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), t => new TConfiguration()));
        }

        private TConfiguration SealIfNeeded<TConfiguration>(TConfiguration config) where TConfiguration : IFeatureConfiguration
        {
            if (IsSealed)
                (config as ISealableFeatureConfiguration)?.Seal();
            return config;
        }

        /// <summary>
        /// Seals configuration making it immutable.
        /// It calls <see cref="ISealableFeatureConfiguration.Seal"/>() method on all configuration items that implements the <see cref="ISealableFeatureConfiguration"/> interface.
        /// Since this call, the <see cref="Get{TConfiguration}"/>() method will return only sealed configuration (current, and future default one).
        /// </summary>
        /// <returns>Self.</returns>
        public LightBddConfiguration Seal()
        {
            if (IsSealed)
                return this;
            IsSealed = true;
            foreach (var value in _configuration.Values)
                SealIfNeeded(value);
            return this;
        }

        /// <summary>
        /// Returns true if configuration is sealed.
        /// </summary>
        public bool IsSealed { get; private set; }
    }
}
