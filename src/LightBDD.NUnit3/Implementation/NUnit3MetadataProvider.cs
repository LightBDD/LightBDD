using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3MetadataProvider : CoreMetadataProvider
    {
        private readonly TestSuite _testSuite;

        public NUnit3MetadataProvider(LightBddConfiguration configuration, Assembly testAssembly)
            : base(configuration)
        {
            _testSuite = testAssembly != null ? TestSuite.Create(testAssembly) : TestSuite.NotProvided;
        }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContextProvider.Current;
            var testMethod = context?.TestMethod;
            if (testMethod == null || !testMethod.GetCustomAttributes<ScenarioAttribute>().Any())
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.");
            return new ScenarioDescriptor(testMethod, context.TestMethodArguments);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CategoryAttribute>(member, a => a.Name);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<DescriptionAttribute>(featureType.GetTypeInfo(), a => a.Properties["Description"].Cast<string>().Single());
        }

        protected override TestSuite GetTestSuite() => _testSuite;
    }
}