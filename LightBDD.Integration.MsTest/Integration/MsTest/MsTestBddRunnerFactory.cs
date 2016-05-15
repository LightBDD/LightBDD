using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.MsTest
{
    public class MsTestBddRunnerFactory : BddRunnerFactory
    {
        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new MsTestIntegrationContext(progressNotifier);
        }
    }
}