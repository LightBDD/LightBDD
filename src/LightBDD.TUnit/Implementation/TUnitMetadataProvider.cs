using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using TUnit.Core.Extensions;
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
            var testMethod = context?.Metadata.TestDetails.Method.MethodMetadata.GetReflectionInfo();
            if (testMethod == null || !testMethod.GetCustomAttributes<TestAttribute>().Any())
            {
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.");
            }

            return new ScenarioDescriptor(testMethod, context.Metadata.TestDetails.TestMethodArguments);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return TestContext.Current?.Metadata.TestDetails.Attributes.AttributesByType[typeof(CategoryAttribute)].OfType<CategoryAttribute>().Select(x => x.Category) ?? [];
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return TestContext.Current?.Metadata.TestDetails.Class.ClassType.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;
        }

        protected override string GetImplementationSpecificScenarioDescription(ScenarioDescriptor scenarioDescriptor)
        {
            return TestContext.Current?.Metadata.TestDetails.Method.MethodMetadata.GetCustomAttributes().OfType<DescriptionAttribute>().FirstOrDefault()?.Description;
        }
    }
}