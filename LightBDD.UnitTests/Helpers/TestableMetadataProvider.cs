using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override IEnumerable<string> GetImplementationSpecificFeatureCategories(Type testClass)
        {
            return ExtractAttributePropertyValues<CategoryAttribute>(testClass, a => a.Name);
        }
    }
}