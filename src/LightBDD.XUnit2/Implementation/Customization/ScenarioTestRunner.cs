using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    /// <summary>
    /// ScenarioTestRunner re-implements the TestRunner&lt;&gt; RunAsync() method, allowing to skip test programmatically with IgnoreException.
    /// It also captures scenario method details for metadata provider.
    /// </summary>
    internal class ScenarioTestRunner : XunitTestRunner
    {
        public ScenarioTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        public async Task<RunSummary> RunScenarioAsync()
        {
            var runSummary = new RunSummary { Total = 1 };
            var output = string.Empty;

            if (!MessageBus.QueueMessage(new TestStarting(Test)))
                CancellationTokenSource.Cancel();
            else
            {
                AfterTestStarting();
                if (AssemblySettings.Current.SetUpException != null)
                {
                    runSummary.Failed++;
                    if (!MessageBus.QueueMessage(new TestFailed(Test, 0,"LightBddScope SetUp failed", AssemblySettings.Current.SetUpException)))
                        CancellationTokenSource.Cancel();
                }
                else if (!string.IsNullOrEmpty(SkipReason) && AssemblySettings.Current.UseXUnitSkipBehavior)
                {
                    runSummary.Skipped++;

                    if (!MessageBus.QueueMessage(new TestSkipped(Test, SkipReason)))
                        CancellationTokenSource.Cancel();
                }
                else
                {
                    var aggregator = new ExceptionAggregator(Aggregator);

                    if (!aggregator.HasExceptions)
                    {
                        var tuple = await aggregator.RunAsync(() => InvokeTestAsync(aggregator));
                        runSummary.Time = tuple.Item1;
                        output = tuple.Item2;
                    }

                    var exception = aggregator.ToException();
                    TestResultMessage testResult;

                    if (exception == null)
                        testResult = new TestPassed(Test, runSummary.Time, output);
                    else if (exception is IgnoreException)
                    {
                        testResult = new TestSkipped(Test, exception.Message);
                        runSummary.Skipped++;
                    }
                    else
                    {
                        testResult = new TestFailed(Test, runSummary.Time, output, exception);
                        runSummary.Failed++;
                    }

                    if (!CancellationTokenSource.IsCancellationRequested)
                    {
                        if (!MessageBus.QueueMessage(testResult))
                            CancellationTokenSource.Cancel();
                    }
                }

                Aggregator.Clear();
                BeforeTestFinished();

                if (Aggregator.HasExceptions)
                {
                    if (!MessageBus.QueueMessage(new TestCleanupFailure(Test, Aggregator.ToException())))
                        CancellationTokenSource.Cancel();
                }
            }

            if (!MessageBus.QueueMessage(new TestFinished(Test, runSummary.Time, output)))
                CancellationTokenSource.Cancel();

            return runSummary;
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var testOutputHelper = ConstructorArguments.OfType<TestOutputHelper>().FirstOrDefault() ?? new TestOutputHelper();

            testOutputHelper.Initialize(MessageBus, Test);
            try
            {
                TestContextProvider.Initialize(TestMethod, TestMethodArguments, testOutputHelper, SkipReason);
                var totalTime = await InvokeTestMethodAsync(aggregator);
                return Tuple.Create(totalTime, testOutputHelper.Output);
            }
            finally
            {
                testOutputHelper.Uninitialize();
                TestContextProvider.Clear();
            }
        }
    }
}