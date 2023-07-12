using System;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Fixie3.Implementation
{
    internal class FixieFeatureCoordinator : FrameworkFeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException($"LightBDD integration is not initialized. Please ensure that following classes are defined in test assembly: \nclass ConfiguredLightBddScope : {nameof(LightBddScope)} {{ }}\nclass WithLightBddConventions : {nameof(LightBddDiscoveryConvention)} {{ }}");
            if (Instance.IsDisposed)
                throw new InvalidOperationException("LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside  of assembly execution scope.");
            return Instance;
        }

        private FixieFeatureCoordinator(LightBddConfiguration configuration, Assembly testAssembly)
            : base(CreateContext(configuration, testAssembly))
        {
        }

        private static IntegrationContext CreateContext(LightBddConfiguration configuration, Assembly testAssembly)
        {
            return new DefaultIntegrationContext(configuration, new FixieMetadataProvider(configuration, testAssembly), MapExceptionToStatus);
        }

        internal static void InstallSelf(LightBddConfiguration configuration, Assembly testAssembly)
        {
            Install(new FixieFeatureCoordinator(configuration, testAssembly));
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}