using LightBDD.Core.ExecutionContext;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Extensibility;

/// <summary>
/// BddRunnerContext dedicated to be used by framework internals and extensions to provide <see cref="IBddRunner"/> for currently executed scenario.
/// </summary>
public class BddRunnerContext
{
    /// <summary>
    /// Returns <see cref="IBddRunner"/> interface for currently executed scenario or throws an exception if no scenario is being run by the current task.
    /// </summary>
    public static IBddRunner GetCurrent () => new BddRunnerV2(ScenarioExecutionContext.CurrentScenarioStepsRunner);
}