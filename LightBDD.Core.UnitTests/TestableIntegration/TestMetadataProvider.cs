using System;
using System.Collections.Generic;
using System.Reflection;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using NUnit.Framework.Internal;

namespace LightBDD.Core.UnitTests.TestableIntegration
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
            return ExtractAttributePropertyValue<CustomFeatureDescriptionAttribute>(featureType.GetTypeInfo(), a => a.Description);
        }

        public TestMetadataProvider(INameFormatter nameFormatter) : base(nameFormatter, new StepTypeConfiguration(), new DefaultCultureInfoProvider()) { }
        public TestMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration) : base(nameFormatter, stepTypeConfiguration, new DefaultCultureInfoProvider()) { }
    }
}