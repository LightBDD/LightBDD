using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.MsTest2.Configuration;
using LightBDD.MsTest2.Implementation;

namespace LightBDD.MsTest2
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
            MsTest2FeatureCoordinator.InstallSelf(Configure(onConfigure));
        }

        /// <summary>
        /// Finalizes LightBddScope. It should be called after all tests have finished.
        /// </summary>
        public static void Cleanup()
        {
            MsTest2FeatureCoordinator.GetInstance().Dispose();
        }

        private static LightBddConfiguration Configure(Action<LightBddConfiguration> onConfigure)
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            configuration.ProgressNotifierConfiguration()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);

            onConfigure(configuration);
            return configuration;
        }
    }
}