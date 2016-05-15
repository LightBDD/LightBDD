using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.NUnit3
{
    public class NUnit3BddRunnerFactory : BddRunnerFactory
    {
        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new NUnit3IntegrationContext(progressNotifier);
        }
    }
}