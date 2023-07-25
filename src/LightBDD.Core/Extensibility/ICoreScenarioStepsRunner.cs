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
    /// Configures scenario to be executed with context provided by <paramref name="contextProvider"/>.
    /// </summary>
    /// <param name="contextProvider">Context provider function.</param>
    /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
    ICoreScenarioStepsRunner WithContext(Func<object> contextProvider, bool takeOwnership);

    /// <summary>
    /// Configures scenario to be executed with context provided by <paramref name="contextProvider"/>.
    /// </summary>
    /// <param name="contextProvider">Context provider function.</param>
    /// <param name="scopeConfigurator"></param>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
    ICoreScenarioStepsRunner WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator>? scopeConfigurator = null);

    /// <summary>
    /// Executes built scenario.
    /// </summary>
    /// <exception cref="ScenarioExecutionException">Thrown upon scenario failure or upon scenario execution flow interruption.</exception>
    Task RunAsync();

    /// <summary>
    /// Returns configuration
    /// </summary>
    LightBddConfiguration Configuration { get; }
}