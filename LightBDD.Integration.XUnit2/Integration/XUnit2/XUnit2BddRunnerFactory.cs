using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.XUnit2
{
    public class XUnit2BddRunnerFactory : BddRunnerFactory
    {

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new XUnit2IntegrationContext(progressNotifier);
        }
    }
}