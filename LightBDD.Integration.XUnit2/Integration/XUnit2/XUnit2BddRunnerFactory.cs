using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.XUnit2
{
    public class XUnit2BddRunnerFactory : BddRunnerFactory
    {
        public static XUnit2BddRunnerFactory Instance { get; } = new XUnit2BddRunnerFactory();

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new XUnit2IntegrationContext(progressNotifier);
        }
    }
}