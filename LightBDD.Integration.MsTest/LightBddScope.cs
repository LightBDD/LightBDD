using LightBDD.Configuration;
using LightBDD.Integration.MsTest;

namespace LightBDD
{
    public static class LightBddScope
    {
        public static void Initialize(LightBddConfiguration configuration)
        {
            MsTestFeatureCoordinator.InstallSelf(configuration);
        }

        public static void Cleanup()
        {
            MsTestFeatureCoordinator.GetInstance().Dispose();
        }
    }
}