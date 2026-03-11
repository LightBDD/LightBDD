using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;
using LightBDD.XUnit3.Implementation.Customization;
using System;

namespace LightBDD.XUnit3.Implementation
{
    internal class XUnit3FeatureCoordinator : FrameworkFeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException($"{nameof(LightBddScopeAttribute)} is not defined in the project. Please ensure that a class extending {nameof(LightBddScopeAttribute)} is registered at assembly level: [assembly:TestPipelineStartup(typeof(YourScope))]");
            if (Instance.IsDisposed)
                throw new InvalidOperationException($"LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope specified by {nameof(LightBddScopeAttribute)} attribute.");
            return Instance;
        }

        private XUnit3FeatureCoordinator(LightBddConfiguration configuration)
            : base(CreateContext(configuration))
        {
        }

        private static IntegrationContext CreateContext(LightBddConfiguration configuration)
        {
            return new DefaultIntegrationContext(configuration, new XUnit3MetadataProvider(configuration), MapExceptionToStatus);
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new XUnit3FeatureCoordinator(configuration));
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}
