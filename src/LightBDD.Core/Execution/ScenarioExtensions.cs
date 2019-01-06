using System;

namespace LightBDD.Core.Execution
{
    public static class ScenarioExtensions
    {
        public static void ExecuteSync(this IRunnableScenario scenario)
        {
            var task = scenario.ExecuteAsync();
            if (!task.IsCompleted)
                throw new InvalidOperationException("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
            task.GetAwaiter().GetResult();
        }
    }
}