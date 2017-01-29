using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestableFeatureBddRunnerFactory : FeatureBddRunnerFactory
    {
        public TestableFeatureBddRunnerFactory() : this(TestableIntegrationContextBuilder.Default())
        {
        }

        public TestableFeatureBddRunnerFactory(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
            : this(TestableIntegrationContextBuilder.Default().WithFeatureProgressNotifier(featureProgressNotifier).WithScenarioProgressNotifierProvider(scenarioProgressNotifier))
        {
        }

        public TestableFeatureBddRunnerFactory(TestableIntegrationContextBuilder contextBuilder)
            : base(contextBuilder.Build()) { }

        public static IFeatureBddRunner GetRunner(Type featureType)
        {
            return new TestableFeatureBddRunnerFactory().GetRunnerFor(featureType);
        }
    }
}