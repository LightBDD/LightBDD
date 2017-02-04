using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3FeatureRunnerRepository : FeatureRunnerRepository
    {
        public NUnit3FeatureRunnerRepository(LightBddConfiguration configuration) : base(new NUnit3IntegrationContext(configuration))
        {
        }
    }
}