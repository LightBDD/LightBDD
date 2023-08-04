using System;
using System.Threading.Tasks;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution;

/// <summary>
/// Interface describing runnable scenario
/// </summary>
public interface IRunnableScenarioV2
{
    /// <summary>
    /// Scenario result
    /// </summary>
    IScenarioResult Result { get; }
    /// <summary>
    /// Executes scenario and returns the execution result.<br/>
    /// </summary>
    /// <returns>Execution result, available also via <see cref="Result"/> property</returns>
    /// <exception cref="InvalidOperationException">Thrown when scenario was already executed it is pending execution.</exception>
    Task<IScenarioResult> RunAsync();
}