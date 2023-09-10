using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Scheduling;

/// <summary>
/// Interface allowing to schedule scenario execution.
/// </summary>
public interface IScenarioExecutionScheduler
{
    /// <summary>
    /// Schedules <paramref name="scenarioFn"></paramref> to be executed and returns tasks allowing to obtain the execution result.
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    /// <param name="scenarioFn">Scenario function</param>
    /// <returns>Task</returns>
    Task<T> Schedule<T>(Func<Task<T>> scenarioFn);
}