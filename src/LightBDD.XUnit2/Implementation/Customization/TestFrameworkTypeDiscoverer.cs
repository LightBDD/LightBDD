using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class TestFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
    {
        public Type GetTestFrameworkType(IAttributeInfo attribute)
        {
            return typeof(TestFramework);
        }
    }
}