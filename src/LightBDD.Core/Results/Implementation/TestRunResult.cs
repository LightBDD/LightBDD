using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Implementation;

internal class TestRunResult : ITestRunResult
{
    private readonly IReadOnlyList<IFeatureResult> _features;
    public ITestRunInfo Info { get; }
    public ExecutionStatus OverallStatus { get; }
    public ExecutionTime ExecutionTime { get; }
    public IReadOnlyList<IFeatureResult> Features =>_features;

    public TestRunResult(ITestRunInfo info, ExecutionTime executionTime, IEnumerable<IFeatureResult> features)
    {
        _features = features
            .OrderBy(r => r.Info.Name.ToString(), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Info = info;
        ExecutionTime = executionTime;
        OverallStatus = CollectOverallStatus();
    }

    private ExecutionStatus CollectOverallStatus()
    {
        var executionStatus = _features
            .SelectMany(f => f.GetScenarios())
            .Select(s => s.Status)
            .DefaultIfEmpty(ExecutionStatus.NotRun)
            .Max();

        if (executionStatus != ExecutionStatus.Failed)
            executionStatus = ExecutionStatus.Passed;
        return executionStatus;
    }
}