using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestCollectionRunner
{
    private readonly ITestAssembly _testAssembly;

    public LightBddTestCollectionRunner(ITestAssembly testAssembly)
    {
        _testAssembly = testAssembly;
    }

    public async Task<RunSummary> RunAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
    {
        //TODO: handle exceptions
        var scope = GetLightBddScope();

        var results = await new ExecutionPipelineAdapter(messageBus, _testAssembly, scope.Configure).Execute(testCases);
        return new RunSummary
        {
            Failed = results.Features.CountScenariosWithStatus(ExecutionStatus.Failed),
            Skipped = results.Features.CountScenariosWithStatus(ExecutionStatus.Ignored),
            Total = results.Features.CountScenarios(),
            Time = (decimal)results.ExecutionTime.Duration.TotalSeconds
        };
    }

    private LightBddScopeAttribute GetLightBddScope()
    {
        var assembly = _testAssembly.Assembly.ToRuntimeAssembly();

        var attributes = assembly.GetCustomAttributes<LightBddScopeAttribute>().ToArray();
        if (attributes.Length > 1)
            throw new InvalidOperationException($"Only one attribute of {typeof(LightBddScopeAttribute)} type can be defined in assembly {assembly.FullName}");
        return attributes.FirstOrDefault() ?? new LightBddScopeAttribute();
    }
}