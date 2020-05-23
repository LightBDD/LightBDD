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
            // fact
            if (!testMethod.Method.GetCustomAttributes(typeof(DataAttribute)).Any())
                return new XunitTestCase[] { new ScenarioTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod) };

            return base.Discover(discoveryOptions, testMethod, factAttribute);
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForSkippedDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
            IAttributeInfo theoryAttribute, object[] dataRow, string skipReason)
        {
            return new[] { new SkippedDataRowTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, skipReason, dataRow) };
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForSkip(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
            IAttributeInfo theoryAttribute, string skipReason)
        {
            return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, object[] dataRow)
        {
            return new XunitTestCase[]
            {
                new ScenarioTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,  dataRow)
            };
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        {
            return new XunitTestCase[]
            {
                new ScenarioMultiTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod)
            };
        }
    }
}