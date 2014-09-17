using System;
using System.Linq;
using NUnit.Framework;

namespace LightBDD.UnitTests
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
    }
}