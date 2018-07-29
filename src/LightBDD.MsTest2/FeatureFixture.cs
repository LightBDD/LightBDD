using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2
{
    /// <summary>
    /// Base class for feature tests with MSTest framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    [FeatureFixture]
    public class FeatureFixture : ITestContextProvider
    {
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

        /// <summary>
        /// Gives access to currently executed test context, including ability to write output.
        /// The property is set by the MsTest Framework.
        /// </summary>
        public TestContext TestContext { get; set; }
    }
}