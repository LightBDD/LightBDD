using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.NUnit2.Implementation
{
    internal class NUnit2FeatureRunnerRepository : FeatureRunnerRepository
    {
        public NUnit2FeatureRunnerRepository(LightBddConfiguration configuration) : base(new NUnit2IntegrationContext(configuration))
        {
        }
    }
}