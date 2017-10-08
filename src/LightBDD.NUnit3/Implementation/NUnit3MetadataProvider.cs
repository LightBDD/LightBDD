using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Framework.Formatting.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit3MetadataProvider : CoreMetadataProvider
    {
        public NUnit3MetadataProvider(LightBddConfiguration configuration)
            : base(configuration.NameFormatterConfiguration().Formatter,
                configuration.StepTypeConfiguration(),
                configuration.CultureInfoProviderConfiguration().CultureInfoProvider,
                configuration.ValueFormattingConfiguration()) { }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContextProvider.Current;
            var testMethod = context?.TestMethod;
            if (testMethod == null || !testMethod.GetCustomAttributes<ScenarioAttribute>().Any())
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.");
            return new ScenarioDescriptor(testMethod, context.TestMethodArguments);
        }

        public override MethodBase CaptureCurrentScenarioMethod()
        {
            return CaptureCurrentScenario().MethodInfo;
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