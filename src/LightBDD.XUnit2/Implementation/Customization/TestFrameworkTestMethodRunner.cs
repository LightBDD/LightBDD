using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class TestFrameworkTestMethodRunner : XunitTestMethodRunner
    {
        private readonly ITestClass _testClass;

        public TestFrameworkTestMethodRunner(ITestClass testClass, ITestMethod testMethod, IReflectionTypeInfo @class,
            IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink,
            IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource,
            object[] constructorArguments)
            : base(testMethod, @class, method, testCases, diagnosticMessageSink,
                messageBus, aggregator, cancellationTokenSource, constructorArguments)
        {
            _testClass = testClass;
        }

        protected override Task<RunSummary> RunTestCasesAsync()
        {
            return TaskExecutor.RunAsync(
                CancellationTokenSource.Token,
                TestCases.Select(c => (Func<Task<RunSummary>>)(() => RunTestCaseAsync(c))).ToArray(),
                _testClass);
        }
    }
}