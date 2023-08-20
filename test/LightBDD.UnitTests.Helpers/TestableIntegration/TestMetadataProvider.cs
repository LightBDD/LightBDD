#nullable enable
using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Formatting;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestMetadataProvider
    {
        public static CoreMetadataProvider Default { get; } = Create();

        public static CoreMetadataProvider Create(Action<LightBddConfiguration>? onConfigure = null)
        {
            var configuration = new LightBddConfiguration();
            configuration.RegisterNameFormatter(x => x.Use(DefaultNameFormatter.Instance));
            configuration.ForMetadata().RegisterEngineAssembly(typeof(TestMetadataProvider).Assembly);
            onConfigure?.Invoke(configuration);
            return configuration.BuildContainer().Resolve<CoreMetadataProvider>();
        }
    }
}