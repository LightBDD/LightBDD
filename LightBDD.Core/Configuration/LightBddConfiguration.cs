using System;
using System.Collections.Concurrent;

namespace LightBDD.Configuration
{
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, object> _configuration = new ConcurrentDictionary<Type, object>();

        public TConfiguration Get<TConfiguration>() where TConfiguration : IFeatureConfiguration, new()
        {
            return (TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), t => new TConfiguration());
        }
    }
}
