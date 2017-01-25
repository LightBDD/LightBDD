using System;
using System.Collections.Concurrent;

namespace LightBDD.Core.Configuration
{
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, IFeatureConfiguration> _configuration = new ConcurrentDictionary<Type, IFeatureConfiguration>();

        public TConfiguration Get<TConfiguration>() where TConfiguration : IFeatureConfiguration, new()
        {
            return (TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), t => new TConfiguration());
        }
    }
}
