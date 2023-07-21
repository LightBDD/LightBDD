using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using LightBDD.Core.Discovery;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestCaseDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _diagnosticMessageSink;
    private readonly ScenarioDiscoverer _discoverer = new();

    public LightBddTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
    }

    public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
    {
        var typeInfo = ((ReflectionTypeInfo)testMethod.TestClass.Class).Type.GetTypeInfo();

        foreach (var scenario in _discoverer.DiscoverFor(typeInfo, testMethod.Method.ToRuntimeMethod(), CancellationToken.None))
        {
            yield return new XunitTestCase(
                _diagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                scenario.ScenarioArguments.ToNullIfEmpty());
        }
    }
}