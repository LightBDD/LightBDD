using System;

namespace LightBDD.Core.Execution.Scheduling;

/// <summary>
/// Scenario execution scheduler attribute allowing to customize scheduler used for annotated scenario.
/// </summary>
public interface IScenarioExecutionSchedulerAttribute
{
    /// <summary>
    /// Returns type of <see cref="IScenarioExecutionScheduler"/> used to schedule execution of the scenario.
    /// </summary>
    Type SchedulerType { get; }
}