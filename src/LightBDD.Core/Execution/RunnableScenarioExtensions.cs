using System;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Runnable scenario extensions.
    /// </summary>
    public static class RunnableScenarioExtensions
    {
        /// <summary>
        /// Executes scenario synchronously, throwing <see cref="InvalidOperationException"/> if scenario did not finished execution upon return.
        /// </summary>
        /// <param name="scenario">Scenario</param>
        /// <exception cref="InvalidOperationException">Thrown if scenario did not finished upon return or scenario was already executed.</exception>
        /// <exception cref="ScenarioExecutionException">Thrown when any exception occurs during execution.</exception>
        //TODO: remove
        public static void ExecuteSync(this IRunnableScenario scenario)
        {
            var task = scenario.ExecuteAsync();
            if (!task.IsCompleted)
                throw new InvalidOperationException("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
            task.GetAwaiter().GetResult();
        }
    }
}