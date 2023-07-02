using LightBDD.Framework;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Fixie3
{
    /// <summary>
    /// Base class for feature tests with Fixie framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [FeatureFixture]
    public class FeatureFixture
    {
        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Constructor initializing <see cref="Runner"/> for feature class instance.
        /// </summary>
        protected FeatureFixture()
        {
            Runner = FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
        }
    }
}