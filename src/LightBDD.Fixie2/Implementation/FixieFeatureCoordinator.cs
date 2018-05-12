using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Reporting.Configuration;

namespace LightBDD.Fixie2.Implementation
{
    internal class FixieFeatureCoordinator : FeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException($"LightBDD integration is not initialized. Please ensure that following classes are defined in test assembly: \nclass ConfiguredLightBddScope : {nameof(LightBddScope)} {{ }}\nclass WithLightBddConventions : {nameof(LightBddDiscoveryConvention)} {{ }}");
            if (Instance.IsDisposed)
                throw new InvalidOperationException("LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside  of assembly execution scope.");
            return Instance;
        }

        private FixieFeatureCoordinator(LightBddConfiguration configuration) : base(
            new FixieFeatureRunnerRepository(configuration),
            new FeatureReportGenerator(configuration.ReportWritersConfiguration().ToArray()),
            configuration)
        {
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new FixieFeatureCoordinator(configuration));
        }
    }
}