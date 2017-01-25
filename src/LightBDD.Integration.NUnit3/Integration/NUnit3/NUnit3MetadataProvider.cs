using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3MetadataProvider : CoreMetadataProvider
    {
        public NUnit3MetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider)
            : base(nameFormatter, stepTypeConfiguration, cultureInfoProvider) { }

        public override MethodBase CaptureCurrentScenarioMethod()
        {
            var scenarioMethod = TestMethodInfoProvider.TestMethod;
            if (scenarioMethod == null || !scenarioMethod.GetCustomAttributes<ScenarioAttribute>().Any())
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.");
            return scenarioMethod;
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CategoryAttribute>(member, a => a.Name);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<DescriptionAttribute>(featureType.GetTypeInfo(), a => a.Properties["Description"].Cast<string>().Single());
        }
    }
}