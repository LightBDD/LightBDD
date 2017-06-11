using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Reporting.Configuration;

namespace LightBDD.NUnit2.Implementation
{
    internal class NUnit2FeatureCoordinator : FeatureCoordinator
    {
        public new static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("{0} is not defined in the project. Please ensure that following attribute, or attribute extending it is defined at assembly level: [assembly:{0}]", nameof(LightBddScopeAttribute)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException($"LightBDD scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope specified by {nameof(LightBddScopeAttribute)} attribute.");
            return Instance;
        }

        private NUnit2FeatureCoordinator(LightBddConfiguration configuration) : base(
            new NUnit2FeatureRunnerRepository(configuration),
            new FeatureReportGenerator(configuration.ReportWritersConfiguration().ToArray()),
            configuration)
        {
        }

        internal static void InstallSelf(LightBddConfiguration configuration)
        {
            Install(new NUnit2FeatureCoordinator(configuration));
        }
    }
}