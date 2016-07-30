using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.MsTest
{
    public class MsTestBddRunnerFactory : BddRunnerFactory
    {
        public MsTestBddRunnerFactory(LightBddConfiguration configuration) : base(configuration) { }

        protected override IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier)
        {
            return new MsTestIntegrationContext(Configuration, progressNotifier);
        }
    }
}