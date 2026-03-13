using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation
{
    internal class XUnit3MetadataProvider : CoreMetadataProvider
    {
        public XUnit3MetadataProvider(LightBddConfiguration configuration)
            : base(configuration) { }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContext.Current;
            var testMethod = context?.TestMethod;
            if (testMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute.");

            var fixtureType = context.TestClassInstance?.GetType();
            if (fixtureType == null)
                throw new InvalidOperationException("Unable to locate test class instance. Please ensure that scenario is executed from method with [Scenario] attribute.");

            var methodInfo = fixtureType.GetMethod(testMethod.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
                throw new InvalidOperationException($"Unable to locate method '{testMethod.MethodName}' on type '{fixtureType.FullName}'. Please ensure that scenario is executed from method with [Scenario] attribute.");

            var arguments = (context.Test as XunitTest)?.TestMethodArguments;
            return new ScenarioDescriptor(methodInfo, arguments);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return member.CustomAttributes
                .Where(attr => attr.AttributeType == typeof(TraitAttribute))
                .Select(attr => attr.ConstructorArguments)
                .Where(args => args.Count == 2 && Equals(args[0].Value, "Category"))
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
