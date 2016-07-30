using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.XUnit2
{
    public class XUnit2BddRunnerFactory : BddRunnerFactory
    {
        public XUnit2BddRunnerFactory(LightBddConfiguration configuration) : base(configuration)
        {
        }

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new XUnit2IntegrationContext(Configuration, progressNotifier);
        }
    }
}