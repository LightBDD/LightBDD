using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.MsTest
{
    internal class MsTestBddRunnerFactory : BddRunnerFactory
    {
        public MsTestBddRunnerFactory(LightBddConfiguration configuration) : base(new MsTestIntegrationContext(configuration)) { }
    }
}