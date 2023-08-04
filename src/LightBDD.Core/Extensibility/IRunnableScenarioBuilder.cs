#nullable enable
using System;
using System.Collections.Generic;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// Builder interface for <see cref="IRunnableScenarioV2"/>
/// </summary>
public interface IRunnableScenarioBuilder
{
    /// <summary>
    /// Configures scenario with runtime ID.
    /// </summary>
    /// <param name="runtimeId">Runtime ID</param>
    /// <returns>Self.</returns>
    IRunnableScenarioBuilder WithRuntimeId(string runtimeId);

    /// <summary>
    /// Configures scenario with name.
    /// </summary>
    /// <param name="name">Name to set.</param>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentException">Throws when <paramref name="name"/> is <c>null</c> or empty.</exception>
    IRunnableScenarioBuilder WithName(INameInfo name);

    /// <summary>
    /// Configures scenario with labels.
    /// </summary>
    /// <param name="labels">Labels to set.</param>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="labels"/> are <c>null</c>.</exception>
    IRunnableScenarioBuilder WithLabels(IReadOnlyList<string> labels);

    /// <summary>
    /// Configures scenario with categories.
    /// </summary>
    /// <param name="categories">Categories to set.</param>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="categories"/> are <c>null</c>.</exception>
    IRunnableScenarioBuilder WithCategories(IReadOnlyList<string> categories);

    /// <summary>
    /// Configures scenario to be executed with additional decorators provided by <paramref name="scenarioDecorators"/>.
    /// </summary>
    /// <param name="scenarioDecorators">Decorators to use.</param>
    /// <returns>Self.</returns>
    IRunnableScenarioBuilder WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators);

    /// <summary>
    /// Specifies scenario entry point
    /// </summary>
    /// <param name="entryMethod">Entry point</param>
    /// <returns>Self/</returns>
    IRunnableScenarioBuilder WithScenarioEntryMethod(ScenarioEntryMethod entryMethod);

    /// <summary>
    /// Builds runnable scenario.
    /// </summary>
    /// <returns>Runnable scenario</returns>
    IRunnableScenarioV2 Build();
}