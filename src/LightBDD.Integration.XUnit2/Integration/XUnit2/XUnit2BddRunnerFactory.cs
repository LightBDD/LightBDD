using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2BddRunnerFactory : BddRunnerFactory
    {
        public XUnit2BddRunnerFactory(LightBddConfiguration configuration) : base(new XUnit2IntegrationContext(configuration))
        {
        }
    }
}