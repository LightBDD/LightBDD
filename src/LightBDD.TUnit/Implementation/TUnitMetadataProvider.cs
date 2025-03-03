using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using CategoryAttribute = TUnit.Core.CategoryAttribute;

namespace LightBDD.TUnit.Implementation
{
    internal class TUnitMetadataProvider : CoreMetadataProvider
    {
        public TUnitMetadataProvider(LightBddConfiguration configuration)
            : base(configuration) { }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContext.Current;
            var testMethod = context?.TestDetails.TestMethod.ReflectionInformation;
            if (testMethod == null || !testMethod.GetCustomAttributes<TestAttribute>().Any())
            {
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.");
            }

            return new ScenarioDescriptor(testMethod, context.TestDetails.TestMethodArguments);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CategoryAttribute>(member, a => a.Category);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<DescriptionAttribute>(featureType.GetTypeInfo(), a => a.Description);
        }

        protected override string GetImplementationSpecificScenarioDescription(ScenarioDescriptor scenarioDescriptor)
        {
            return ExtractAttributePropertyValue<DescriptionAttribute>(scenarioDescriptor.MethodInfo, a => a.Description);
        }
    }
}