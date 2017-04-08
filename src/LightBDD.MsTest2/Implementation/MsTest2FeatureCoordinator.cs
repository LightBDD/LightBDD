using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Reporting.Configuration;

namespace LightBDD.MsTest2.Implementation
{
    internal class MsTest2FeatureCoordinator : FeatureCoordinator
    {
        public static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("LightBDD integration is not initialized. Please ensure that following class is defined in test assembly: \n[TestClass] class LightBddIntegration\n{{\n    [AssemblyInitialize] public static void Setup(TestContext testContext){{ {0}.{1}(); }}\n    [AssemblyCleanup] public static void Cleanup(){{ {0}.{2}(); }}\n}}", nameof(LightBddScope), nameof(LightBddScope.Initialize), nameof(LightBddScope.Cleanup)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException("LightBdd scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope.");
            return Instance;
        }

        public MsTest2FeatureCoordinator(LightBddConfiguration configuration) : base(
            new MsTest2FeatureRunnerRepository(configuration),
            new FeatureReportGenerator(configuration.ReportWritersConfiguration().ToArray()), configuration)
        {
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new MsTest2FeatureCoordinator(configuration));
        }
    }
}