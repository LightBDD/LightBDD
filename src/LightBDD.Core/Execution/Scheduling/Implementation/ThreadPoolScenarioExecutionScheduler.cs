using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Scheduling.Implementation;

internal class ThreadPoolScenarioExecutionScheduler : IScenarioExecutionScheduler
{
    public Task<T> Schedule<T>(Func<Task<T>> scenarioFn) => Task.Run(scenarioFn);
}