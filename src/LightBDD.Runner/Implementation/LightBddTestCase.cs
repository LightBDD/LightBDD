using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation;

internal class LightBddTestCase : XunitTestCase
{
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public LightBddTestCase()
    {
    }
    public LightBddTestCase(IMessageSink diagnosticMessageSink, ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod)
        : base(diagnosticMessageSink,discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod)
    {
    }
}