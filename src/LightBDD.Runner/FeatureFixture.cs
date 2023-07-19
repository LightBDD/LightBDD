using System;
using LightBDD.Framework;
using LightBDD.XUnit2.Implementation;
using Xunit.Abstractions;

//TODO: update namespace
namespace LightBDD.XUnit2
{
    /// <summary>
    /// Base class for feature tests with XUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    public class FeatureFixture : ITestOutputProvider
    {
        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when TestOutput is null (scenario was executed from test without [Scenario] attribute or was explicitly initialized with null).</exception>
        public ITestOutputHelper TestOutput { get; }

        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner => BddRunnerAdapter.Instance;

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance and configures <see cref="TestOutput"/> with default test output.
        /// </summary>
        protected FeatureFixture() : this(TestOutputHelpers.Current)
        {
        }

        /// <summary>
        /// Constructor initializing <see cref="Runner"/> for feature class instance as well as configures <see cref="TestOutput"/> with <paramref name="output"/>.
        /// </summary>
        protected FeatureFixture(ITestOutputHelper output)
        {
            TestOutput = output;
        }
    }
}