using LightBDD.Integration.MsTest;

namespace LightBDD
{
    public static class LightBddScope
    {
        public static void Initialize()
        {
            MsTestFeatureCoordinator.InstallSelf();
        }

        public static void Cleanup()
        {
            MsTestFeatureCoordinator.GetInstance().Dispose();
        }
    }
}