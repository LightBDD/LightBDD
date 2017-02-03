using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Reporting;
using LightBDD.Reporting;
using LightBDD.Reporting.Configuration;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3FeatureCoordinator : FeatureCoordinator
    {
        public static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("{0} is not defined in the project. Please ensure that following attribute, or attribute extending it is defined at assembly level: [assembly:{0}]", nameof(LightBddScopeAttribute)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException(string.Format("LightBdd scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope specified by {0} attribute.", nameof(LightBddScopeAttribute)));
            return Instance;
        }

        public NUnit3FeatureCoordinator(FeatureBddRunnerFactory runnerFactory, IFeatureAggregator featureAggregator) : base(runnerFactory, featureAggregator)
        {
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new NUnit3FeatureCoordinator(new NUnit3FeatureBddRunnerFactory(configuration), new FeatureReportGenerator(configuration.ReportWritersConfiguration().ToArray())));
        }
    }
}