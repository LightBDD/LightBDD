using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using Xunit.Abstractions;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Base class for feature tests with XUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [FeatureFixture]
    public class FeatureFixture : ITestOutputProvider
    {
        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        public ITestOutputHelper TestOutput { get; }
        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance as well as configures <see cref="TestOutput"/> with <paramref name="output"/>.
        /// </summary>
        protected FeatureFixture(ITestOutputHelper output)
        {
            TestOutput = output;
            Runner = FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
        }
    }
}