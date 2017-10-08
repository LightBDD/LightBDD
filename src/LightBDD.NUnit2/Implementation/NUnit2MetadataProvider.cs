using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Formatting.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit2.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit2MetadataProvider : CoreMetadataProvider
    {
        public NUnit2MetadataProvider(LightBddConfiguration configuration)
            : base(configuration.NameFormatterConfiguration().Formatter,
                configuration.StepTypeConfiguration(),
                configuration.CultureInfoProviderConfiguration().CultureInfoProvider,
                configuration.ValueFormattingConfiguration()) { }

        public override MethodBase CaptureCurrentScenarioMethod()
        {
            var scenarioMethod = TestContextProvider.Current?.TestMethod;
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
            return ExtractAttributePropertyValue<DescriptionAttribute>(featureType.GetTypeInfo(), a => a.Description);
        }
    }
}