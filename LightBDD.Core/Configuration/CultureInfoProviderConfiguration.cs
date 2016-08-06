using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;

namespace LightBDD.Configuration
{
    public class CultureInfoProviderConfiguration : IFeatureConfiguration
    {
        public CultureInfoProviderConfiguration()
        {
            CultureInfoProvider = new DefaultCultureInfoProvider();
        }

        public ICultureInfoProvider CultureInfoProvider { get; private set; }

        public CultureInfoProviderConfiguration UpdateCultureInfoProvider(ICultureInfoProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            CultureInfoProvider = provider;
            return this;
        }
    }
}