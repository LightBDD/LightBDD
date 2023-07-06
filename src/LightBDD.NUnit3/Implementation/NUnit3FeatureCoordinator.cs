using System;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3FeatureCoordinator : FrameworkFeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("{0} is not defined in the project. Please ensure that following attribute, or attribute extending it is defined at assembly level: [assembly:{0}]", nameof(LightBddScopeAttribute)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException($"LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope specified by {nameof(LightBddScopeAttribute)} attribute.");
            return Instance;
        }

        private NUnit3FeatureCoordinator(LightBddConfiguration configuration, Assembly testAssembly)
            : base(CreateContext(configuration, testAssembly))
        {
        }

        private static IntegrationContext CreateContext(LightBddConfiguration configuration, Assembly testAssembly)
        {
            return new DefaultIntegrationContext(configuration, new NUnit3MetadataProvider(configuration, testAssembly), MapExceptionToStatus);
        }

        internal static void InstallSelf(LightBddConfiguration configuration, Assembly testAssembly)
        {
            Install(new NUnit3FeatureCoordinator(configuration, testAssembly));
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException || ex is InconclusiveException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}