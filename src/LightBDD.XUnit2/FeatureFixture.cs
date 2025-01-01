using System;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.XUnit2.Implementation;
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
        private readonly ITestOutputHelper _testOutput;

        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when TestOutput is null (scenario was executed from test without [Scenario] attribute or was explicitly initialized with null).</exception>
        public ITestOutputHelper TestOutput => _testOutput ?? throw new InvalidOperationException(nameof(TestOutput) + " is not provided. Ensure that scenario is executed from method with [Scenario] attribute, or " + nameof(ITestOutputHelper) + " instance is provided to " + nameof(FeatureFixture) + " constructor.");

        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance and configures <see cref="TestOutput"/> with default test output.
        /// </summary>
        protected FeatureFixture()
        {
            _testOutput = TestContextProvider.Current?.OutputHelper;
            Runner = FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
        }

        /// <summary>
        /// Constructor initializing <see cref="Runner"/> for feature class instance as well as configures <see cref="TestOutput"/> with <paramref name="output"/>.
        /// </summary>
        [Obsolete($"Use parameterless constructor instead and retrieve {nameof(ITestOutputHelper)} via {nameof(TestOutput)} property.", true)]
        protected FeatureFixture(ITestOutputHelper output)
        {
            _testOutput = output;
            Runner = FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
        }
    }
}