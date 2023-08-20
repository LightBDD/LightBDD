#nullable enable
using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.Helpers
{
    public class TestMetadataProvider
    {
        public static CoreMetadataProvider Default { get; } = Create();

        public static CoreMetadataProvider Create(Action<LightBddConfiguration>? onConfigure = null)
        {
            var configuration = new LightBddConfiguration();
            configuration.Services.ConfigureNameFormatter(x => x.Use<UnderscoreToSpaceFormatter>());
            configuration.ForMetadata().RegisterEngineAssembly(typeof(TestMetadataProvider).Assembly);
            onConfigure?.Invoke(configuration);
            return configuration.BuildContainer().Resolve<CoreMetadataProvider>();
        }
    }
}