using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;

namespace LightBDD.TUnit
{
    /// <summary>
    /// Base class for feature tests with TUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    public class FeatureFixture
    {
        private static readonly object Lock = new();
        private IBddRunner _runner;

        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner
        {
            get
            {
                lock (Lock)
                {
                    return _runner ??= FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
                }
            }
        }
        

        /// <summary>
        /// Feature fixture tear down method disposing <see cref="Runner"/> after all tests are executed.
        /// </summary>
        [After(Class)]
        public static void FeatureFixtureTearDown(ClassHookContext classHookContext)
        {
            FeatureRunnerProvider.GetRunnerFor(classHookContext.ClassType).Dispose();
        }
    }
}