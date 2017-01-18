using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableBddRunnerFactory : BddRunnerFactory
    {
        public TestableBddRunnerFactory() : this(NoProgressNotifier.Default, fixture => NoProgressNotifier.Default)
        {
        }

        public TestableBddRunnerFactory(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
            : base(new TestableIntegrationContext(featureProgressNotifier, scenarioProgressNotifier))
        {
        }

        public TestableBddRunnerFactory(IIntegrationContext integrationContext)
            : base(integrationContext) { }

        public static IFeatureBddRunner GetRunner(Type featureType)
        {
            return new TestableBddRunnerFactory().GetRunnerFor(featureType);
        }
    }
}