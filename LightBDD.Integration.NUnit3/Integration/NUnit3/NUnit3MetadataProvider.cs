using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3MetadataProvider : CoreMetadataProvider
    {
        public NUnit3MetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration) : base(nameFormatter, stepTypeConfiguration) { }
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            var scenarioMethod = new StackTrace()
                .GetFrames()
                .Skip(2)
                .Select(f => f.GetMethod())
                .FirstOrDefault(f => f.GetCustomAttributes(true).Any(a => a is TestAttribute));

            if (scenarioMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Test] attribute and [assembly:Debuggable(true, true)] attribute is present in test assembly.");
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