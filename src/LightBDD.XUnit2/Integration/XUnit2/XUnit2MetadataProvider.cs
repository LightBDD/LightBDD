using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;

namespace LightBDD.Integration.XUnit2
{
    [DebuggerStepThrough]
    internal class XUnit2MetadataProvider : CoreMetadataProvider
    {
        public XUnit2MetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider) 
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
            return Enumerable.Empty<string>();
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return null;
        }
    }
}