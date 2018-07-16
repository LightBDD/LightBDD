using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class ScenarioTestCaseDiscoverer : TheoryDiscoverer
    {
        public ScenarioTestCaseDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public override IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            // skipped
            if (factAttribute.GetNamedArgument<string>("Skip") != null)
                return new[] { new XunitTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod) };

            // fact
            if (!testMethod.Method.GetCustomAttributes(typeof(DataAttribute)).Any())
                return new XunitTestCase[] { new ScenarioTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod) };

            return base.Discover(discoveryOptions, testMethod, factAttribute);
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,IAttributeInfo theoryAttribute, object[] dataRow)
        {
            return new XunitTestCase[]
            {
                new ScenarioTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod,dataRow)
            };
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,IAttributeInfo theoryAttribute)
        {
            return new XunitTestCase[]
            {
                new ScenarioMultiTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod)
            };
        }
    }
}