using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2MetadataProvider : CoreMetadataProvider
    {
        public XUnit2MetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration) : base(nameFormatter, stepTypeConfiguration) { }
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            var scenarioMethod = new StackTrace()
                .GetFrames()
                .Skip(2)
                .Select(f => f.GetMethod())
                .FirstOrDefault(f => f.GetCustomAttributes(true).Any(a => a is ScenarioAttribute));

            if (scenarioMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and [assembly:Debuggable(true, true)] attribute is present in test assembly.");
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