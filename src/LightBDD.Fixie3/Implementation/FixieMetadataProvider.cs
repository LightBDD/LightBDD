using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;

namespace LightBDD.Fixie3.Implementation
{
    internal class FixieMetadataProvider : CoreMetadataProvider
    {
        private readonly TestSuite _testSuite;

        public FixieMetadataProvider(LightBddConfiguration configuration, Assembly testAssembly)
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

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member) => Enumerable.Empty<string>();

        protected override string GetImplementationSpecificFeatureDescription(Type featureType) => null;

        protected override TestSuite GetTestSuite() => _testSuite;
    }
}