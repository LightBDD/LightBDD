using System;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.Runner.Implementation;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.Runner
{
    /// <summary>
    /// Base class for feature tests with XUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [Collection(LightBddTestCollection.Name)]
    public class FeatureFixture : ITestOutputProvider
    {
        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when TestOutput is null (scenario was executed from test without [Scenario] attribute or was explicitly initialized with null).</exception>
        public ITestOutputHelper TestOutput { get; } = TestOutputHelpers.Current;

        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; } = BddRunnerContext.GetCurrent();
    }
}