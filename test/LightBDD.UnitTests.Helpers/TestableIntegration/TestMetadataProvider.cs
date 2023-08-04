using System;
using System.Collections.Generic;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Formatting;
using NUnit.Framework.Internal;
using TestSuite = LightBDD.Core.Metadata.TestSuite;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestMetadataProvider : CoreMetadataProvider
    {
        protected override TestSuite GetTestSuite()
        {
            return TestSuite.Create(TestExecutionContext.CurrentContext.CurrentTest.TypeInfo.Assembly);
        }

        public TestMetadataProvider() : base(Configure(_ => { }))
        {
        }

        public TestMetadataProvider(LightBddConfiguration configuration) :
            base(configuration)
        { }


        public TestMetadataProvider(Action<LightBddConfiguration> onConfigure)
            : base(Configure(onConfigure))
        {
        }

        private static LightBddConfiguration Configure(Action<LightBddConfiguration> onConfigure)
        {
            var configuration = new LightBddConfiguration();
            configuration.NameFormatterConfiguration().UpdateFormatter(DefaultNameFormatter.Instance);
            configuration.MetadataConfiguration().RegisterEngineAssembly(typeof(TestMetadataProvider).Assembly);
            onConfigure(configuration);
            return configuration;
        }
    }
}