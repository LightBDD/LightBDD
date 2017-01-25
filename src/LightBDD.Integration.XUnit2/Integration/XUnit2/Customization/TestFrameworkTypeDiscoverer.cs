using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Integration.XUnit2.Customization
{
    internal class TestFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
    {
        public Type GetTestFrameworkType(IAttributeInfo attribute)
        {
            return typeof(TestFramework);
        }
    }
}