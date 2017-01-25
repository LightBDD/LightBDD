using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Integration.XUnit2.Customization
{
    internal class ScenarioTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public ScenarioTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var defaultMethodDisplay = discoveryOptions.MethodDisplayOrDefault();
            return factAttribute.GetNamedArgument<string>("Skip") != null
                ? new[] { new XunitTestCase(_diagnosticMessageSink, defaultMethodDisplay, testMethod) }
                : new XunitTestCase[] { new ScenarioTestCase(_diagnosticMessageSink, defaultMethodDisplay, testMethod) };
        }
    }
}