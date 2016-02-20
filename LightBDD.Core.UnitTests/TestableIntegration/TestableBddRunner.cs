using System;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableBddRunner : CoreBddRunner
    {
        public TestableBddRunner(Type featureType, IProgressNotifier progressNotifier)
            : base(featureType, new TestableIntegrationContext(progressNotifier))
        {
        }

        public TestableBddRunner(Type featureType) : this(featureType, new NoProgressNotifier())
        {
        }
    }
}