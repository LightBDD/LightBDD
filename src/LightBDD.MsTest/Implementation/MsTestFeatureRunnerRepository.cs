using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.MsTest.Implementation
{
    internal class MsTestFeatureRunnerRepository : FeatureRunnerRepository
    {
        public MsTestFeatureRunnerRepository(LightBddConfiguration configuration) : base(new MsTestIntegrationContext(configuration)) { }
    }
}