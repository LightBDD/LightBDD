using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.MsTest2.Implementation
{
    internal class MsTest2FeatureRunnerRepository : FeatureRunnerRepository
    {
        public MsTest2FeatureRunnerRepository(LightBddConfiguration configuration) : base(new MsTest2IntegrationContext(configuration)) { }
    }
}