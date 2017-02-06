using System;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Notification.Configuration;
using LightBDD.MsTest.Implementation;

namespace LightBDD.MsTest
{
    /// <summary>
    /// LightBddScope class allowing to initialize and finalize LightBDD in MsTest framework.
    /// 
    /// Example showing how to initialize LightBDD in MsTest framework:
    /// <example>
    /// [TestClass] class LightBddIntegration
    /// {
    ///     [AssemblyInitialize] public static void Setup(TestContext testContext){ LightBddScope.Initialize(); }
    ///     [AssemblyCleanup] public static void Cleanup(){ LightBddScope.Cleanup(); }
    /// }
    /// </example>
    /// </summary>
    public static class LightBddScope
    {
        /// <summary>
        /// Initializes LightBddScope with default configuration.
        /// </summary>
        public static void Initialize()
        {
            Initialize(cfg => { });
        }

        /// <summary>
        /// Initializes LightBddScope with configuration customized with <paramref name="onConfigure"/> action.
        /// </summary>
        /// <param name="onConfigure">Action allowing to customize LightBDD configuration.</param>
        public static void Initialize(Action<LightBddConfiguration> onConfigure)
        {
            MsTestFeatureCoordinator.InstallSelf(Configure(onConfigure));
        }

        /// <summary>
        /// Finalizes LightBddScope. It should be called after all tests have finished.
        /// </summary>
        public static void Cleanup()
        {
            MsTestFeatureCoordinator.GetInstance().Dispose();
        }

        private static LightBddConfiguration Configure(Action<LightBddConfiguration> onConfigure)
        {
            var configuration = new LightBddConfiguration();

            configuration
                .ExecutionExtensionsConfiguration()
                .EnableStepCommenting();

            configuration.Get<FeatureProgressNotifierConfiguration>()
                .UpdateNotifier(MsTestProgressNotifier.CreateFeatureProgressNotifier());

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .UpdateNotifierProvider(MsTestProgressNotifier.CreateScenarioProgressNotifier);

            onConfigure(configuration);
            return configuration;
        }
    }
}