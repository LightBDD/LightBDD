using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.MsTest
{
    internal class MsTestFeatureBddRunnerFactory : FeatureBddRunnerFactory
    {
        public MsTestFeatureBddRunnerFactory(LightBddConfiguration configuration) : base(new MsTestIntegrationContext(configuration)) { }
    }
}