using System;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableBddRunnerFactory : BddRunnerFactory
    {
        public TestableBddRunnerFactory() : base(new LightBddConfiguration())
        {
        }

        public static IBddRunner GetRunner(Type featureType, IProgressNotifier progressNotifier)
        {
            return new TestableBddRunnerFactory().GetRunnerFor(featureType, () => progressNotifier).AsBddRunner();
        }

        public static IBddRunner GetRunner(Type featureType)
        {
            return GetRunner(featureType, new NoProgressNotifier());
        }

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new TestableIntegrationContext(progressNotifier);
        }
    }
}