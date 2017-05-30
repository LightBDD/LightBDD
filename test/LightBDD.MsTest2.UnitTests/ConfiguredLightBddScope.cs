using System.Collections.Concurrent;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2.UnitTests
{
    [TestClass]
    public class ConfiguredLightBddScope
    {
        public static readonly ConcurrentQueue<string> CapturedNotifications = new ConcurrentQueue<string>();

        [AssemblyInitialize]
        public static void Setup(TestContext testContext)
        {
            LightBddScope.Initialize(OnConfigure);
        }

        [AssemblyCleanup]
        public static void Cleanup() { LightBddScope.Cleanup(); }

        private static void OnConfigure(LightBddConfiguration configuration)
        {
            var defaultProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;

            configuration
                .ScenarioProgressNotifierConfiguration()
                .UpdateNotifierProvider<object>(feature => new DelegatingScenarioProgressNotifier(defaultProvider(feature), new DefaultProgressNotifier(x => CapturedNotifications.Enqueue(x))));
        }
    }
}