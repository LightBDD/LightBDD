using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3FeatureBddRunnerFactory : FeatureBddRunnerFactory
    {
        public NUnit3FeatureBddRunnerFactory(LightBddConfiguration configuration) : base(new NUnit3IntegrationContext(configuration))
        {
        }
    }
}