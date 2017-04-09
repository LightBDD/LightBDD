using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;

namespace LightBDD.Framework.ExecutionContext
{
    public class LightBddContext
    {
        public static LightBddConfiguration Configuration => FeatureCoordinatorWrapper.GetInstance()?.Configuration ?? new LightBddConfiguration().Seal();


        private class FeatureCoordinatorWrapper : FeatureCoordinator
        {
            private FeatureCoordinatorWrapper() : base(null, null, null) { }
            public static FeatureCoordinator GetInstance() => Instance;
        }
    }
}
