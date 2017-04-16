using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// The feature runner interface allowing to execute feature tests and collect execution results.
    /// The instances of this interface can be provided by <see cref="FeatureRunnerRepository"/>.
    /// <para>It is expected that for given feature test class, one instance of <see cref="IFeatureRunner"/> is created, while each class instance will have dedicated instance of <see cref="IFeatureFixtureRunner"/>.</para>
    /// <para>Note for testing framework integration projects: The feature runner should be disposed after all scenarios execution.</para>
    /// </summary>
    public interface IFeatureRunner : IDisposable
    {
        /// <summary>
        /// Returns <see cref="IFeatureFixtureRunner"/> instance allowing to execute scenario tests on <paramref name="fixture"/> object.
        /// </summary>
        /// <param name="fixture">Fixture object containing scenario tests to execute.</param>
        /// <returns><see cref="IFeatureFixtureRunner"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="fixture"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws when <paramref name="fixture"/> type does not match feature type that <see cref="IFeatureRunner"/> has been created for by <see cref="FeatureRunnerRepository"/>.</exception>
        IFeatureFixtureRunner ForFixture(object fixture);
        /// <summary>
        /// Returns current results of feature tests.
        /// </summary>
        /// <returns></returns>
        IFeatureResult GetFeatureResult();
    }
}