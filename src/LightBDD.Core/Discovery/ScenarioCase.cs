#nullable enable
using System;
using System.Reflection;

namespace LightBDD.Core.Discovery;

/// <summary>
/// Scenario Case
/// </summary>
public class ScenarioCase
{
    /// <summary>
    /// Feature Fixture type
    /// </summary>
    public TypeInfo FeatureFixtureType { get; private set; } = null!;
    /// <summary>
    /// Scenario method
    /// </summary>
    public MethodInfo ScenarioMethod { get; private set; } = null!;
    /// <summary>
    /// Scenario arguments which may be empty if <see cref="ScenarioMethod"/> is parameter-less or <see cref="RequireArgumentResolutionAtRuntime"/> is <c>true</c>.
    /// </summary>
    public object[] ScenarioArguments { get; private set; } = Array.Empty<object>();
    /// <summary>
    /// Specifies if scenario arguments need to be provided at runtime.
    /// </summary>
    public bool RequireArgumentResolutionAtRuntime { get; private set; }
    /// <summary>
    /// Specifies scenario unique Id
    /// </summary>
    public string RuntimeId { get; private set; }

    /// <summary>
    /// Creates parameter-less scenario case
    /// </summary>
    /// <param name="fixtureType">Fixture type</param>
    /// <param name="scenarioMethod">Scenario method</param>
    public static ScenarioCase CreateParameterless(TypeInfo fixtureType, MethodInfo scenarioMethod) => new() { FeatureFixtureType = fixtureType, ScenarioMethod = scenarioMethod };

    /// <summary>
    /// Creates parameterized scenario case
    /// </summary>
    /// <param name="fixtureType">Fixture type</param>
    /// <param name="scenarioMethod">Scenario method</param>
    /// <param name="scenarioArguments">Scenario method arguments</param>
    public static ScenarioCase CreateParameterized(TypeInfo fixtureType, MethodInfo scenarioMethod, object[] scenarioArguments) => new() { FeatureFixtureType = fixtureType, ScenarioMethod = scenarioMethod, ScenarioArguments = scenarioArguments };

    /// <summary>
    /// Creates parameterized scenario case with arguments provided at runtime.
    /// </summary>
    /// <param name="fixtureType">Fixture type</param>
    /// <param name="scenarioMethod">Scenario method</param>
    public static ScenarioCase CreateParameterizedAtRuntime(TypeInfo fixtureType, MethodInfo scenarioMethod) => new() { FeatureFixtureType = fixtureType, ScenarioMethod = scenarioMethod, RequireArgumentResolutionAtRuntime = true };

    /// <summary>
    /// Sets external ID
    /// </summary>
    public ScenarioCase WithRuntimeId(string runtimeId)
    {
        RuntimeId = runtimeId;
        return this;
    }
}