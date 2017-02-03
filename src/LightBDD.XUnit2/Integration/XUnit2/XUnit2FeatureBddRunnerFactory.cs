using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2FeatureBddRunnerFactory : FeatureBddRunnerFactory
    {
        public XUnit2FeatureBddRunnerFactory(LightBddConfiguration configuration) : base(new XUnit2IntegrationContext(configuration))
        {
        }
    }
}