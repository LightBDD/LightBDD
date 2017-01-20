using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestableBddRunnerFactory : BddRunnerFactory
    {
        public TestableBddRunnerFactory() : this(TestableIntegrationContextBuilder.Default())
        {
        }

        public TestableBddRunnerFactory(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
            : this(TestableIntegrationContextBuilder.Default().WithFeatureProgressNotifier(featureProgressNotifier).WithScenarioProgressNotifierProvider(scenarioProgressNotifier))
        {
        }

        public TestableBddRunnerFactory(TestableIntegrationContextBuilder contextBuilder)
            : base(contextBuilder.Build()) { }

        public static IFeatureBddRunner GetRunner(Type featureType)
        {
            return new TestableBddRunnerFactory().GetRunnerFor(featureType);
        }
    }
}