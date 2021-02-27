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

        [Obsolete]
        public TestableFeatureRunnerRepository(IFeatureProgressNotifier featureProgressNotifier,
            Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
            : this(TestableIntegrationContextBuilder.Default().WithFeatureProgressNotifier(featureProgressNotifier)
                .WithScenarioProgressNotifierProvider(scenarioProgressNotifier))
        {
        }

        public TestableFeatureRunnerRepository(IProgressNotifier progressNotifier)
            : this(TestableIntegrationContextBuilder.Default().WithProgressNotifier(progressNotifier))
        {
        }

        public TestableFeatureRunnerRepository(TestableIntegrationContextBuilder contextBuilder)
            : this(contextBuilder.Build()) { }

        private TestableFeatureRunnerRepository(IntegrationContext context) : base(context)
        {
            Context = context;
        }

        public IntegrationContext Context { get; }

        public static IFeatureRunner GetRunner(Type featureType)
        {
            return new TestableFeatureRunnerRepository().GetRunnerFor(featureType);
        }
    }
}