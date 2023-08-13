#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// Interface for building and running scenario steps
/// </summary>
public interface ICoreScenarioStepsRunner
{
    /// <summary>
    /// Configures steps to be executed with scenario.
    /// </summary>
    /// <param name="steps">Steps to execute.</param>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="steps"/> are <c>null</c>.</exception>
    ICoreScenarioStepsRunner AddSteps(IEnumerable<StepDescriptor> steps);

    /// <summary>
    /// Configures execution context for steps to execute.
    /// </summary>
    /// <param name="contextProvider">Context provider.</param>
    /// <returns>Self.</returns>
    ICoreScenarioStepsRunner WithContext(Resolvable<object?> contextProvider);

    /// <summary>
    /// Executes built scenario.
    /// </summary>
    /// <exception cref="ScenarioExecutionException">Thrown upon scenario failure or upon scenario execution flow interruption.</exception>
    Task RunAsync();

    /// <summary>
    /// Returns configuration
    /// </summary>
    //TODO: review if can be removed
    LightBddConfiguration Configuration { get; }
}