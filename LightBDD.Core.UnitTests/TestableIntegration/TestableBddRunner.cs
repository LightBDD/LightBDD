using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableBddRunnerFactory : BddRunnerFactory
    {
        public static IBddRunner GetRunner(Type featureType, Func<IProgressNotifier> progressNotifierProvider)
        {
            return new TestableBddRunnerFactory().GetRunnerFor(featureType, progressNotifierProvider).AsBddRunner();
        }

        public static IBddRunner GetRunner(Type featureType, IProgressNotifier progressNotifier)
        {
            return GetRunner(featureType, () => progressNotifier);
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