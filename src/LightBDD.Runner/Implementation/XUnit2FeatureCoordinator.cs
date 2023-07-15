using System;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Runner.Implementation
{
    internal class XUnit2FeatureCoordinator : FrameworkFeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("{0} is not defined in the project. Please ensure that following attribute, or attribute extending it is defined at assembly level: [assembly:{0}]", nameof(LightBddScopeAttribute)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException($"LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope specified by {nameof(LightBddScopeAttribute)} attribute.");
            return Instance;
        }

        private XUnit2FeatureCoordinator(Assembly testAssembly, LightBddConfiguration configuration)
            : base(CreateContext(testAssembly, configuration))
        {
        }

        private static IntegrationContext CreateContext(Assembly testAssembly, LightBddConfiguration configuration)
        {
            return new DefaultIntegrationContext(configuration, new XUnit2MetadataProvider(testAssembly,configuration), MapExceptionToStatus);
        }

        internal static XUnit2FeatureCoordinator InstallSelf(Assembly testAssembly, LightBddConfiguration configuration)
        {
            var coordinator = new XUnit2FeatureCoordinator(testAssembly, configuration);
            Install(coordinator);
            return coordinator;
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}