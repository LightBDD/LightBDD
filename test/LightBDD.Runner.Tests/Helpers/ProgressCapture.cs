using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;

namespace LightBDD.Runner.Tests.Helpers;

public class ProgressCapture : IProgressNotifier
{
    private static readonly ConcurrentStack<ITestRunResult> CapturedResults = new();
    public static ITestRunResult TestRunResult => CapturedResults.Single();
    public static IScenarioResult GetScenarioResult(string scenarioId) => GetScenarioResults(scenarioId).Single();
    public static IEnumerable<IScenarioResult> GetScenarioResults(string scenarioId)
    {
        return TestRunResult.Features.SelectMany(f => f.GetScenarios())
            .Where(s => s.Info.Labels.Contains(scenarioId));
    }

    public void Notify(ProgressEvent e)
    {
        if (e is TestRunFinished f)
            CapturedResults.Push(f.Result);
    }
}