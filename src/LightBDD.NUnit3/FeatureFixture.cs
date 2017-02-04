using LightBDD.Core.Extensibility;
using NUnit.Framework;

namespace LightBDD.NUnit3
{
    /// <summary>
    /// Base class for feature tests with NUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [FeatureFixture]
    public class FeatureFixture
    {
        private readonly IFeatureBddRunner _featureBddRunner;
        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance.
        /// </summary>
        protected FeatureFixture()
        {
            _featureBddRunner = FeatureRunnerProvider.GetRunnerFor(GetType());
            Runner = _featureBddRunner.GetRunner(this);
        }

        /// <summary>
        /// Feature fixture tear down method disposing <see cref="Runner"/> after all tests are executed.
        /// </summary>
        [OneTimeTearDown]
        public void FeatureFixtureTearDown()
        {
            _featureBddRunner.Dispose();
        }
    }
}