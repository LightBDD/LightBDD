namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// IFeatureFixtureRunner interface allowing to define and execute scenarios in programmatic manner.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
    /// </summary>
    public interface IFeatureFixtureRunner
    {
        /// <summary>
        /// Creates new scenario to execute.
        /// </summary>
        /// <returns>Scenario runner instance.</returns>
        IScenarioRunner NewScenario();
    }
}