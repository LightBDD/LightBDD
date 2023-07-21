using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestAssemblyRunner : XunitTestAssemblyRunner
{
    public LightBddTestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
    {
    }

    protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
    {
        if (testCollection.CollectionDefinition?.ToRuntimeType() == typeof(LightBddTestCollection))
            return new LightBddTestCollectionRunner(TestAssembly).RunAsync(messageBus, testCollection, testCases, cancellationTokenSource);
        return base.RunTestCollectionAsync(messageBus, testCollection, testCases, cancellationTokenSource);
    }
}