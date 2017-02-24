using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using NUnit.Framework;

namespace LightBDD.NUnit2
{
    /// <summary>
    /// Base class for feature tests with NUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [FeatureFixture]
    public class FeatureFixture
    {
        private readonly IFeatureRunner _featureRunner;
        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance.
        /// </summary>
        protected FeatureFixture()
        {
            _featureRunner = FeatureRunnerProvider.GetRunnerFor(GetType());
            Runner = _featureRunner.GetBddRunner(this);
        }

        /// <summary>
        /// Feature fixture tear down method disposing <see cref="Runner"/> after all tests are executed.
        /// </summary>
        [TestFixtureTearDown]
        public void FeatureFixtureTearDown()
        {
            _featureRunner.Dispose();
        }
    }
}