using LightBDD.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3BddRunnerFactory : BddRunnerFactory
    {
        public NUnit3BddRunnerFactory(LightBddConfiguration configuration) : base(new NUnit3IntegrationContext(configuration))
        {
        }
    }
}