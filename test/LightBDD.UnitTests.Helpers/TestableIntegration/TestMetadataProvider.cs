using System;
using System.Collections.Generic;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using NUnit.Framework.Internal;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestMetadataProvider : CoreMetadataProvider
    {
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            return TestExecutionContext.CurrentContext.CurrentTest.Method.MethodInfo;
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CustomCategoryAttribute>(member, a => a.Name);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<CustomFeatureDescriptionAttribute>(featureType.GetTypeInfo(),
                a => a.Description);
        }

        public TestMetadataProvider(INameFormatter nameFormatter)
            : base(nameFormatter, new StepTypeConfiguration(), new DefaultCultureInfoProvider(), new ValueFormattingConfiguration())
        {
        }

        public TestMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration)
            : base(nameFormatter, stepTypeConfiguration, new DefaultCultureInfoProvider(), new ValueFormattingConfiguration())
        {
        }

        public TestMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider)
            : base(nameFormatter, stepTypeConfiguration, cultureInfoProvider, new ValueFormattingConfiguration())
        {
        }
    }
}