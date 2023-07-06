using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3.Implementation
{
    internal class MsTest3MetadataProvider : CoreMetadataProvider
    {
        private readonly TestSuite _testSuite;

        public MsTest3MetadataProvider(LightBddConfiguration configuration, Assembly testAssembly)
            : base(configuration)
        {
            _testSuite = TestSuite.Create(testAssembly);
        }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContextProvider.Current;
            if (context?.TestMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute.");
            return new ScenarioDescriptor(context.TestMethod, context.TestMethodArguments);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributes<TestCategoryAttribute>(member).SelectMany(a => a.TestCategories);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return null;
        }

        protected override TestSuite GetTestSuite() => _testSuite;
    }
}