using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation
{
    internal class LightBddFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
    {
        public Type GetTestFrameworkType(IAttributeInfo attribute)
        {
            return typeof(LightBddTestFramework);
        }
    }
}