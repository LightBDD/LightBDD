using System;
using LightBDD.Core.Execution;

namespace LightBDD.Framework;

/// <summary>
/// Attribute that can be applied to scenario method to change it's execution priority.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ScenarioPriorityAttribute : Attribute, IExecutionPriorityAttribute
{
    /// <summary>
    /// Constructor allowing to change execution priority
    /// </summary>
    /// <param name="priority"></param>
    public ScenarioPriorityAttribute(ScenarioPriority priority)
    {
        Priority = (int)priority;
    }
    /// <inheritdoc />
    public int Priority { get; }
}