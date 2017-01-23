using System;
using LightBDD.Core.Configuration;
using LightBDD.Integration.MsTest;
using LightBDD.Notification.Configuration;

namespace LightBDD
{
    public static class LightBddScope
    {
        public static void Initialize()
        {
            Initialize(cfg => { });
        }

        public static void Initialize(Action<LightBddConfiguration> onConfigure)
        {
            MsTestFeatureCoordinator.InstallSelf(Configure(onConfigure));
        }

        public static void Cleanup()
        {
            MsTestFeatureCoordinator.GetInstance().Dispose();
        }

        private static LightBddConfiguration Configure(Action<LightBddConfiguration> onConfigure)
        {
            var configuration = new LightBddConfiguration();

            configuration.Get<FeatureProgressNotifierConfiguration>()
                .UpdateNotifier(MsTestProgressNotifier.CreateFeatureProgressNotifier());

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .UpdateNotifierProvider(MsTestProgressNotifier.CreateScenarioProgressNotifier);

            onConfigure(configuration);
            return configuration;
        }
    }
}