using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class ScenarioTestCaseRunner : XunitTestCaseRunner
    {
        public ScenarioTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, object[] testMethodArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
             : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource)
        {
        }

        protected override Task<RunSummary> RunTestAsync()
        {
            var runner = new ScenarioTestRunner(CreateTest(TestCase, DisplayName), MessageBus, TestClass,
                ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes,
                new ExceptionAggregator(Aggregator), CancellationTokenSource);

            return runner.RunScenarioAsync();
        }
    }
}