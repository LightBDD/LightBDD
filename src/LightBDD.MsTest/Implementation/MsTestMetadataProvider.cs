using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest.Implementation
{
    [DebuggerStepThrough]
    internal class MsTestMetadataProvider : CoreMetadataProvider
    {
        public MsTestMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider)
            : base(nameFormatter, stepTypeConfiguration, cultureInfoProvider) { }
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            var scenarioMethod = TestMethodInfoProvider.TestMethod;
            if (scenarioMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute.");
            return scenarioMethod;
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributes<TestCategoryAttribute>(member).SelectMany(a => a.TestCategories);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return null;
        }
    }
}