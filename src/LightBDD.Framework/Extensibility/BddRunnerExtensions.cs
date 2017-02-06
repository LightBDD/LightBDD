using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// IBddRunner extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class BddRunnerExtensions
    {
        /// <summary>
        /// Method allowing to retrieve the <see cref="IFeatureFixtureRunner"/> instance allowing to define and execute scenarios.
        /// This method is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
        /// </summary>
        /// <typeparam name="TContext">Bdd runner context type.</typeparam>
        /// <param name="runner">Bdd runner.</param>
        /// <returns>Instance of <see cref="IFeatureFixtureRunner"/>.</returns>
        public static IFeatureFixtureRunner Integrate<TContext>(this IBddRunner<TContext> runner)
        {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            if (!(runner is IFeatureFixtureRunner))
                throw new InvalidOperationException($"The type '{runner.GetType().Name}' has to implement '{nameof(IFeatureFixtureRunner)}' interface to support integration.");

            return (IFeatureFixtureRunner)runner;
        }

        /// <summary>
        /// Method allowing to retrieve the <see cref="IBddRunner"/> instance from <see cref="IFeatureRunner"/>.
        /// The <see cref="IBddRunner"/> is a main runner interface that should be used for executing LightBDD scenarios, while the interfaces coming from LightBDD.Core namespace should be used only in integration projects.
        /// </summary>
        /// <param name="featureRunner">Instance of <see cref="IFeatureRunner"/>.</param>
        /// <param name="fixture">Feature fixture instance.</param>
        /// <returns></returns>
        public static IBddRunner GetBddRunner(this IFeatureRunner featureRunner, object fixture)
        {
            if (featureRunner == null)
                throw new ArgumentNullException(nameof(featureRunner));
            return new BddRunner(featureRunner.ForFixture(fixture));
        }
    }
}