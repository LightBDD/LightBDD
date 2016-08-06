using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest
{
    internal class MsTestMetadataProvider : CoreMetadataProvider
    {
        public MsTestMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider)
            : base(nameFormatter, stepTypeConfiguration, cultureInfoProvider) { }
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            var scenarioMethod = new StackTrace()
                .GetFrames()
                .Skip(2)
                .Select(f => f.GetMethod())
                .FirstOrDefault(f => f.GetCustomAttributes(true).Any(a => a is TestMethodAttribute));

            if (scenarioMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [TestMethod] attribute and [assembly:Debuggable(true, true)] attribute is present in test assembly.");
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