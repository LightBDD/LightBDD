using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestableFeatureRunnerRepository : FeatureRunnerRepository
    {
        public TestableFeatureRunnerRepository() : this(TestableIntegrationContextBuilder.Default())
        {
        }

        public TestableFeatureRunnerRepository(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
            : this(TestableIntegrationContextBuilder.Default().WithFeatureProgressNotifier(featureProgressNotifier).WithScenarioProgressNotifierProvider(scenarioProgressNotifier))
        {
        }

        public TestableFeatureRunnerRepository(TestableIntegrationContextBuilder contextBuilder)
            : base(contextBuilder.Build()) { }

        public static IFeatureBddRunner GetRunner(Type featureType)
        {
            return new TestableFeatureRunnerRepository().GetRunnerFor(featureType);
        }
    }
}