using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.XUnit2
{
    public class XUnitBddRunnerFactory : BddRunnerFactory
    {
        public static XUnitBddRunnerFactory Instance { get; } = new XUnitBddRunnerFactory();

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new XUnitIntegrationContext(progressNotifier);
        }
    }
}