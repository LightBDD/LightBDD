using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit
{
    /// <summary>
    /// ScenarioTestRunner reimplements the TestRunner&lt;&gt; RunAsync() method, allowing to skip test programatically with IgnoreException.
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

                if (!string.IsNullOrEmpty(SkipReason))
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
                        if (!MessageBus.QueueMessage(testResult))
                            CancellationTokenSource.Cancel();
                }

                Aggregator.Clear();
                BeforeTestFinished();

                if (Aggregator.HasExceptions)
                    if (!MessageBus.QueueMessage(new TestCleanupFailure(Test, Aggregator.ToException())))
                        CancellationTokenSource.Cancel();
            }

            if (!MessageBus.QueueMessage(new TestFinished(Test, runSummary.Time, output)))
                CancellationTokenSource.Cancel();

            return runSummary;
        }
    }
}