using System;
using System.Threading;
using LightBDD.Core.Execution.Scheduling;

namespace LightBDD.Framework;

/// <summary>
/// When applied on scenario method, it informs LightBDD to schedule scenario tasks to be executed on a single dedicated thread, and allowing usage of <see cref="ThreadLocal{T}"/> to share data between scenario, decorators and steps.<br/>
/// See <see cref="ScenarioExecutionSchedulerTypes.DedicatedThread"/> for more details.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RunOnDedicatedThreadAttribute : Attribute, IScenarioExecutionSchedulerAttribute
{
    /// <inheritdoc />
    public Type SchedulerType => ScenarioExecutionSchedulerTypes.DedicatedThread;
}