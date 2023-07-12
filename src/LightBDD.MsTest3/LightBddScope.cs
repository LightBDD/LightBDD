using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.MsTest3.Configuration;
using LightBDD.MsTest3.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3
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
        /// Initializes LightBddScope for provided test assembly.
        /// </summary>
        /// <param name="testContext">Test context</param>
        /// <param name="onConfigure">Optional configuration.</param>
        public static void Initialize(TestContext testContext, Action<LightBddConfiguration> onConfigure = null)
        {
            Initialize(GetTypeAssembly(testContext), onConfigure);
        }

        /// <summary>
        /// Initializes LightBddScope for provided test assembly.
        /// </summary>
        /// <param name="testAssembly">Assembly for which the tests are executed</param>
        /// <param name="onConfigure">Optional configuration.</param>
        public static void Initialize(Assembly testAssembly, Action<LightBddConfiguration> onConfigure = null)
        {
            MsTest3FeatureCoordinator.InstallSelf(Configure(onConfigure), testAssembly);
        }

        private static Assembly GetTypeAssembly(TestContext testContext)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                       .FirstOrDefault(assembly => assembly.GetType(testContext.FullyQualifiedTestClassName, false, true) != null)
                   ?? throw new InvalidOperationException($"Unable to identify assembly for type: {testContext.FullyQualifiedTestClassName}");
        }

        /// <summary>
        /// Finalizes LightBddScope. It should be called after all tests have finished.
        /// </summary>
        public static void Cleanup()
        {
            MsTest3FeatureCoordinator.GetInstance().Dispose();
        }

        private static LightBddConfiguration Configure(Action<LightBddConfiguration> onConfigure)
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            configuration.ProgressNotifierConfiguration()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);

            onConfigure?.Invoke(configuration);
            return configuration;
        }
    }
}