using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3.Implementation
{
    internal class MsTest3FeatureCoordinator : FrameworkFeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("LightBDD integration is not initialized. Please ensure that following class is defined in test assembly: \n[TestClass] class LightBddIntegration\n{{\n    [AssemblyInitialize] public static void Setup(TestContext testContext){{ {0}.{1}(); }}\n    [AssemblyCleanup] public static void Cleanup(){{ {0}.{2}(); }}\n}}", nameof(LightBddScope), nameof(LightBddScope.Initialize), nameof(LightBddScope.Cleanup)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException("LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope.");
            return Instance;
        }

        private MsTest3FeatureCoordinator(LightBddConfiguration configuration)
            : base(CreateContext(configuration))
        {
        }

        private static IntegrationContext CreateContext(LightBddConfiguration configuration)
        {
            return new DefaultIntegrationContext(configuration, new MsTest3MetadataProvider(configuration), MapExceptionToStatus);
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new MsTest3FeatureCoordinator(configuration));
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is AssertInconclusiveException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}