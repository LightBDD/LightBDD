using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;
using LightBDD.XUnit2.Implementation.Customization;
using System;

namespace LightBDD.XUnit2.Implementation
{
    internal class XUnit2FeatureCoordinator : FrameworkFeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("{0} is not defined in the project. Please ensure that following attribute, or attribute extending it is defined at assembly level: [assembly:{0}]", nameof(LightBddScopeAttribute)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException(string.Format("LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope specified by {0} attribute.", nameof(LightBddScopeAttribute)));
            return Instance;
        }

        private XUnit2FeatureCoordinator(LightBddConfiguration configuration)
            : base(CreateContext(configuration))
        {
        }

        private static IntegrationContext CreateContext(LightBddConfiguration configuration)
        {
            return new DefaultIntegrationContext(configuration, new XUnit2MetadataProvider(configuration), MapExceptionToStatus);
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new XUnit2FeatureCoordinator(configuration));
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}