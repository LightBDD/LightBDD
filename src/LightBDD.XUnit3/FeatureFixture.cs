using System;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.XUnit3.Implementation;
using Xunit;

namespace LightBDD.XUnit3
{
    /// <summary>
    /// Base class for feature tests with xUnit v3 framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [FeatureFixture]
    public class FeatureFixture : ITestOutputProvider
    {
        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when TestOutput is null (scenario was executed from test without [Scenario] attribute).</exception>
        public ITestOutputHelper TestOutput => TestContext.Current?.TestOutputHelper ?? throw new InvalidOperationException(nameof(TestOutput) + " is not provided. Ensure that scenario is executed from method with [Scenario] attribute.");

        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance.
        /// </summary>
        protected FeatureFixture()
        {
            Runner = FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
        }
    }
}
