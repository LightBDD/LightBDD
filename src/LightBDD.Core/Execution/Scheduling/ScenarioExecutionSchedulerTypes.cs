using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Scheduling.Implementation;

namespace LightBDD.Core.Execution.Scheduling;

/// <summary>
/// Class providing types representing core implementations of <see cref="IScenarioExecutionScheduler"/>, which can be used on <see cref="IScenarioExecutionSchedulerAttribute"/>.
/// </summary>
public static class ScenarioExecutionSchedulerTypes
{
    /// <summary>
    /// Schedules scenario tasks to be executed on a single dedicated thread, and allowing usage of <see cref="ThreadLocal{T}"/> to share data between scenario, decorators and steps.<br/>
    /// While the assigned thread may be used to execute other scenarios too, it is guaranteed that at a given point of running the specific scenario, no other scenarios will be executed on that thread. <br/>
    /// Using this scheduler does not prevent other scenarios to be executed in parallel using this scheduler type (in which case, every scenario will have own dedicated thread) or other schedulers.<br/>
    /// Please note that all tasks spawned within the scenario will be executed on the same thread, unless <see cref="Task.ConfigureAwait"/> is called with <c>false</c> parameter or task is explicitly scheduled with <see cref="Task.Run(System.Action)"/> or similar methods.
    /// </summary>
    public static readonly Type DedicatedThread = typeof(DedicatedThreadScenarioExecutionScheduler);
    /// <summary>
    /// Schedules scenario tasks to be executed on a standard thread pool, leveraging multiple cores during scenario execution. It is an equivalent of using <see cref="Task.Run{TResult}(System.Func{System.Threading.Tasks.Task{TResult}?})"/>
    /// </summary>
    public static readonly Type SharedThreadPool = typeof(ThreadPoolScenarioExecutionScheduler);
}