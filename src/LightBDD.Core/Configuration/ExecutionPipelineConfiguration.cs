using System;

namespace LightBDD.Core.Configuration;

/// <summary>
/// Execution pipeline configuration
/// </summary>
//TODO: expand implementation and cover with tests (separate ticket)
public class ExecutionPipelineConfiguration : FeatureConfiguration
{
    /// <summary>
    /// Max concurrent scenarios to execute.
    /// </summary>
    public int MaxConcurrentScenarios { get; private set; } = GetDefaultMaxConcurrentScenarios();

    /// <summary>
    /// Sets max concurrent scenarios to be executed at the same time.<br/>
    /// Using <c>0</c> resets to default which is <see cref="Environment.ProcessorCount"/>.
    /// </summary>
    /// <returns>Self.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxConcurrentScenarios"/> is negative</exception>
    public ExecutionPipelineConfiguration SetMaxConcurrentScenarios(int maxConcurrentScenarios)
    {
        ThrowIfSealed();
        MaxConcurrentScenarios = maxConcurrentScenarios switch
        {
            < 0 => throw new ArgumentOutOfRangeException(nameof(maxConcurrentScenarios), maxConcurrentScenarios, "Value cannot be negative"),
            0 => GetDefaultMaxConcurrentScenarios(),
            _ => maxConcurrentScenarios
        };
        return this;
    }

    private static int GetDefaultMaxConcurrentScenarios() => Math.Max(1, Environment.ProcessorCount);
}