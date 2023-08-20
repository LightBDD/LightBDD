using LightBDD.Framework.Implementation;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Extensibility;

/// <summary>
/// BddRunnerContext dedicated to be used by framework internals and extensions to provide <see cref="IBddRunner"/> for currently executed scenario.
/// </summary>
public class BddRunnerContext
{
    /// <summary>
    /// Returns <see cref="IBddRunner"/> interface for currently executed scenario.
    /// The runner implementation uses deferred initialization, where currently executed scenario details will get obtained upon first call to <see cref="IScenarioBuilder{NoContext}.Integrate()"/>,
    /// at which stage an exception will be thrown if no scenario is being run by the current task.
    /// </summary>
    public static IBddRunner GetCurrent () => new BddRunnerV2();
}