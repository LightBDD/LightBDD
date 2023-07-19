using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// <see cref="ICoreScenarioBuilder"/> integration interface allowing to construct scenario for execution.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
    /// </summary>
    public interface ICoreScenarioBuilder
    {
        /// <summary>
        /// Configures steps to be executed with scenario.
        /// </summary>
        /// <param name="steps">Steps to execute.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="steps"/> are <c>null</c>.</exception>
        ICoreScenarioBuilder AddSteps(IEnumerable<StepDescriptor> steps);
        /// <summary>
        /// Configures scenario details with values inferred by <see cref="CoreMetadataProvider"/>.
        /// </summary>
        /// <returns>Self.</returns>
        ICoreScenarioBuilder WithCapturedScenarioDetails();
        /// <summary>
        /// Configures scenario details with values inferred by <see cref="CoreMetadataProvider"/>, but only if scenario name has not been provided yet.
        /// </summary>
        /// <returns>Self.</returns>
        ICoreScenarioBuilder WithCapturedScenarioDetailsIfNotSpecified();
        /// <summary>
        /// Configures scenario with labels.
        /// </summary>
        /// <param name="labels">Labels to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="labels"/> are <c>null</c>.</exception>
        ICoreScenarioBuilder WithLabels(string[] labels);
        /// <summary>
        /// Configures scenario with categories.
        /// </summary>
        /// <param name="categories">Categories to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="categories"/> are <c>null</c>.</exception>
        ICoreScenarioBuilder WithCategories(string[] categories);
        /// <summary>
        /// Configures scenario with name.
        /// </summary>
        /// <param name="name">Name to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentException">Throws when <paramref name="name"/> is <c>null</c> or empty.</exception>
        ICoreScenarioBuilder WithName(string name);

        /// <summary>
        /// Configures scenario to be executed with context provided by <paramref name="contextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
        ICoreScenarioBuilder WithContext(Func<object> contextProvider, bool takeOwnership);

        /// <summary>
        /// Configures scenario to be executed with context provided by <paramref name="contextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <param name="scopeConfigurator"></param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
        ICoreScenarioBuilder WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator = null);
        /// <summary>
        /// Configures scenario to be executed with additional decorators provided by <paramref name="scenarioDecorators"/>.
        /// </summary>
        /// <param name="scenarioDecorators">Decorators to use.</param>
        /// <returns>Self.</returns>
        ICoreScenarioBuilder WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators);

        /// <summary>
        /// Builds scenario.
        /// </summary>
        /// <returns>Scenario.</returns>
        /// <exception cref="InvalidOperationException">Throws when name or steps are not defined.</exception>
        IRunnableScenario Build();

        /// <summary>
        /// Returns current <see cref="LightBddConfiguration"/> instance.
        /// </summary>
        LightBddConfiguration Configuration { get; }

        /// <summary>
        /// Configures scenario to have provided name.
        /// </summary>
        ICoreScenarioBuilder WithScenarioDetails(IScenarioInfo scenarioInfo);
        /// <summary>
        /// Configures scenario to have unique ID.
        /// </summary>
        ICoreScenarioBuilder WithRuntimeId(string runtimeId);
    }
}