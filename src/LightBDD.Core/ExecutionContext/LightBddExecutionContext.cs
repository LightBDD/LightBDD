#nullable enable
using System;
using System.Threading;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.ExecutionContext;

/// <summary>
/// Global execution context which is present when <see cref="ExecutionPipeline"/> executes tests on the current task flow.
/// </summary>
//TODO: test
public class LightBddExecutionContext
{
    private static readonly AsyncLocal<EngineContext?> Context = new();

    /// <summary>
    /// Returns instance of <see cref="EngineContext"/> used to execute LightBDD tests or <c>null</c> if no tests are executed on the current task flow.
    /// </summary>
    public static EngineContext? GetCurrentIfPresent() => Context.Value;

    /// <summary>
    /// Returns instance of <see cref="EngineContext"/> used to execute LightBDD tests.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no tests are executed on the current task flow</exception>
    public static EngineContext GetCurrent() => GetCurrentIfPresent() ?? throw new InvalidOperationException("No LightBDD tests are executed on this task flow");

    internal static EngineContext Install(EngineContext context)
    {
        Context.Value = context;
        return context;
    }

    internal static void Clear() => Context.Value = null;
}