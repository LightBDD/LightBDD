using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestCase : TestMethodTestCase
{
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public LightBddTestCase()
    {
    }
    public LightBddTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod)
        : base(discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod)
    {
    }
}