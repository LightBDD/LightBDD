using System;
using LightBDD.Core.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.SummaryGeneration;
using LightBDD.SummaryGeneration.Formatters;

namespace LightBDD.Integration.MsTest
{
    internal class MsTestFeatureCoordinator : FeatureCoordinator
    {
        public static FeatureCoordinator GetInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(string.Format("LightBDD integration is not initialized. Please ensure that following class is defined in test assembly: \n[TestClass] class LightBddIntegration\n{{\n    [AssemblyInitialize] public static void Setup(TestContext testContext){{ {0}.{1}(); }}\n    [AssemblyCleanup] public static void Cleanup(){{ {0}.{2}(); }}\n}}", nameof(LightBddScope), nameof(LightBddScope.Initialize), nameof(LightBddScope.Cleanup)));
            if (Instance.IsDisposed)
                throw new InvalidOperationException("LightBdd scenario test execution is already finished. Please ensure that no tests are executed outside of assembly execution scope.");
            return Instance;
        }

        public MsTestFeatureCoordinator(BddRunnerFactory runnerFactory, IFeatureAggregator featureAggregator) : base(runnerFactory, featureAggregator)
        {
        }

        internal static void InstallSelf()
        {
            Install(new MsTestFeatureCoordinator(new MsTestBddRunnerFactory(), new FeatureSummaryGenerator(new SummaryFileWriter(new XmlResultFormatter(), "~/Reports/summary.xml"))));
        }
    }
}