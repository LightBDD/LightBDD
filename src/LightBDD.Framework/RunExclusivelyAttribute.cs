using System;
using LightBDD.Core.Execution.Constraints;

namespace LightBDD.Framework;

/// <summary>
/// When applied on scenario method, it informs LightBDD to run scenario exclusively with no other scenarios being executed at the same time.<br/>
/// Scenarios with this constraint are always executed after unconstrained scenarios.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RunExclusivelyAttribute : Attribute, IScenarioRequireExclusiveRunAttribute
{
}