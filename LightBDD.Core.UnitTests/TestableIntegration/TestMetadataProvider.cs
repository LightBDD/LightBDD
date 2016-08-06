using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestMetadataProvider : CoreMetadataProvider
    {
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            return new StackTrace()
                .GetFrames()
                .First(f => f.GetMethod().GetCustomAttributes(true).Any(a => a is TestAttribute))
                .GetMethod();
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CustomCategoryAttribute>(member, a => a.Name);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<CustomFeatureDescriptionAttribute>(featureType, a => a.Description);
        }

        public TestMetadataProvider(INameFormatter nameFormatter) : base(nameFormatter, new StepTypeConfiguration(), new DefaultCultureInfoProvider()) { }
        public TestMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration) : base(nameFormatter, stepTypeConfiguration, new DefaultCultureInfoProvider()) { }
    }
}