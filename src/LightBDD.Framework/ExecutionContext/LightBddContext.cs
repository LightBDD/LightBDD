using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;

namespace LightBDD.Framework.ExecutionContext
{
    /// <summary>
    /// LightBDD Context class offering LightBDD test specific data during test execution.
    /// </summary>
    //TODO: rethink if this context is better or reworking IFeatureFixtureRunner is more feasible. Consider unit testing of extended scenario runner
    public class LightBddContext
    {
        /// <summary>
        /// Returns <see cref="LightBddConfiguration"/> instance containing current configuration for tests.
        /// If LightBDD is not configured yet, the default configuration would be returned.
        /// </summary>
        public static LightBddConfiguration Configuration => FeatureCoordinatorWrapper.GetInstance()?.Configuration ?? new LightBddConfiguration().Seal();

        private class FeatureCoordinatorWrapper : FeatureCoordinator
        {
            private FeatureCoordinatorWrapper() : base(null, null, null) { }
            public static FeatureCoordinator GetInstance() => Instance;
        }
    }
}
