using System;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableBddRunnerFactory : BddRunnerFactory
    {
        private TestableBddRunnerFactory(IIntegrationContext integrationContext) : base(integrationContext)
        {
        }

        public static IBddRunner GetRunner(Type featureType, IProgressNotifier progressNotifier)
        {
            return new TestableBddRunnerFactory(new TestableIntegrationContext(progressNotifier)).GetRunnerFor(featureType).AsBddRunner();
        }

        public static IBddRunner GetRunner(Type featureType)
        {
            return GetRunner(featureType, new NoProgressNotifier());
        }
    }
}