using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    internal class TestableMetadataProvider : TestMetadataProvider
    {
        protected override string GetImplementationSpecificFeatureDescription(Type testClass)
        {
            return testClass.GetCustomAttributes(typeof(DescriptionAttribute), true)
                            .OfType<DescriptionAttribute>()
                            .Select(a => a.Description)
                            .SingleOrDefault();
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CategoryAttribute>(member, a => a.Name);
        }
    }
}