using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using Xunit;

namespace LightBDD.XUnit2.Implementation
{
    internal class XUnit2MetadataProvider : CoreMetadataProvider
    {
        public XUnit2MetadataProvider(LightBddConfiguration configuration)
            : base(configuration) { }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContextProvider.Current;
            if (context?.TestMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute.");
            return new ScenarioDescriptor(context.TestMethod, context.TestMethodArguments);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return member.CustomAttributes
                .Where(attr => attr.AttributeType == typeof(TraitAttribute))
                .Select(attr => attr.ConstructorArguments)
                .Where(args =>args.Count == 2 && Equals(args[0].Value, "Category"))
                .Select(args => args[1].Value?.ToString());
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return null;
        }

        protected override string GetImplementationSpecificScenarioDescription(ScenarioDescriptor scenarioDescriptor)
        {
            return null;
        }
    }
}