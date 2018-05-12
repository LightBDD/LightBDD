using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Formatting.Configuration;

namespace LightBDD.Fixie2.Implementation
{
    [DebuggerStepThrough]
    internal class FixieMetadataProvider : CoreMetadataProvider
    {
        public FixieMetadataProvider(LightBddConfiguration configuration)
            : base(configuration.NameFormatterConfiguration().Formatter,
                configuration.StepTypeConfiguration(),
                configuration.CultureInfoProviderConfiguration().CultureInfoProvider,
                configuration.ValueFormattingConfiguration()) { }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var context = TestContextProvider.Current;
            if (context?.TestMethod == null)
                throw new InvalidOperationException("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute.");
            return new ScenarioDescriptor(context.TestMethod, context.TestMethodArguments);
        }

        public override MethodBase CaptureCurrentScenarioMethod()
        {
            return CaptureCurrentScenario().MethodInfo;
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