using System;

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

        /// <summary>
        /// Creates enriched runner based on <see cref="IFeatureFixtureRunner"/> and <see cref="IIntegrationContext"/>.
        /// </summary>
        /// <typeparam name="TEnrichedRunner">Type of enriched runner.</typeparam>
        /// <param name="runnerFactory">Runner factory.</param>
        /// <returns></returns>
        TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IIntegrationContext, TEnrichedRunner> runnerFactory);
    }
}