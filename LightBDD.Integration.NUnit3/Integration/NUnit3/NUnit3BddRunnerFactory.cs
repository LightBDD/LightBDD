using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.NUnit3
{
    public class NUnit3BddRunnerFactory : BddRunnerFactory
    {
        public NUnit3BddRunnerFactory(LightBddConfiguration configuration) : base(configuration)
        {
        }

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new NUnit3IntegrationContext(Configuration, progressNotifier);
        }
    }
}