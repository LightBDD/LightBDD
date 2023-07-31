using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Implementation;

//TODO: remove after migration
internal static class StepsRunnerExtensions
{
    public static void RunSync(this ICoreScenarioStepsRunner runner)
    {
        var task = runner.RunAsync();
        if (!task.IsCompleted)
            throw new InvalidOperationException("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
        task.GetAwaiter().GetResult();
    }
}