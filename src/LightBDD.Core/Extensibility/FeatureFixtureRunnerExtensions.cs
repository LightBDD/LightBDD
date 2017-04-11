using System;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// <see cref="IFeatureFixtureRunner"/> extensions.
    /// </summary>
    public static class FeatureFixtureRunnerExtensions
    {
        /// <summary>
        /// Converts to <see cref="IEnrichableFeatureFixtureRunner"/>.
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <returns><see cref="IEnrichableFeatureFixtureRunner"/> instance.</returns>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="runner"/> does not implement <see cref="IEnrichableFeatureFixtureRunner"/> interface.</exception>
        public static IEnrichableFeatureFixtureRunner AsEnrichable(this IFeatureFixtureRunner runner)
        {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));
            if (!(runner is IEnrichableFeatureFixtureRunner))
                throw new NotSupportedException($"The type '{runner.GetType().Name}' has to implement '{nameof(IEnrichableFeatureFixtureRunner)}' interface to support runner enrichment.");
            return (IEnrichableFeatureFixtureRunner)runner;
        }
    }
}