namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// ICoreBddRunner interface allowing to define and execute scenarios in programmatic manner.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
    /// </summary>
    public interface ICoreBddRunner
    {
        /// <summary>
        /// Creates new scenario to execute.
        /// </summary>
        /// <returns>Scenario runner instance.</returns>
        IScenarioRunner NewScenario();
    }
}