using System;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Implementation
{
    internal static class ScenarioExtensions
    {
        public static void AwaitSyncScenario(this Task task)
        {
            if (!task.IsCompleted)
                throw new InvalidOperationException("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
            task.GetAwaiter().GetResult();
        }
    }
}