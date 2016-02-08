using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using Xunit;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestMetadataProvider : CoreMetadataProvider
    {
        public override MethodBase CaptureCurrentScenarioMethod()
        {
            return new StackTrace()
                .GetFrames()
                .First(f => f.GetMethod().GetCustomAttributes(true).Any(a => a is FactAttribute))
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

        public TestMetadataProvider(INameFormatter nameFormatter)
            : base(nameFormatter)
        {
        }
    }
}