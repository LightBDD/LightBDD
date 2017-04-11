using System;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Interface extending <see cref="IFeatureFixtureRunner"/>, offering method to enrich runner by providing <see cref="IIntegrationContext"/>.
    /// </summary>
    public interface IEnrichableFeatureFixtureRunner : IFeatureFixtureRunner
    {
        /// <summary>
        /// Creates enriched runner based on <see cref="IFeatureFixtureRunner"/> and <see cref="IIntegrationContext"/>.
        /// </summary>
        /// <typeparam name="TEnrichedRunner">Type of enriched runner.</typeparam>
        /// <param name="runnerFactory">Runner factory.</param>
        /// <returns></returns>
        TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IIntegrationContext, TEnrichedRunner> runnerFactory);
    }
}