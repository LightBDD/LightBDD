#nullable enable
using System;
using System.Threading;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// Scenario builder context providing access to <see cref="ICoreScenarioBuilder"/> within the LightBDD scenarios to define and run scenario steps.
/// </summary>
public class ScenarioBuilderContext
{
    private static readonly AsyncLocal<ICoreScenarioBuilder?> Builder = new();

    /// <summary>
    /// Returns <see cref="ICoreScenarioBuilder"/> to define and run scenario steps. The builder instance is only available within the currently running scenario method and it should be used only once to run scenario steps.
    /// </summary>
    public static ICoreScenarioBuilder Current => Builder.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");

    internal static void SetCurrent(ICoreScenarioBuilder? builder) => Builder.Value = builder;
}