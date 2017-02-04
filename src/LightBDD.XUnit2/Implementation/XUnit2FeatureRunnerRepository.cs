using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.XUnit2.Implementation
{
    internal class XUnit2FeatureRunnerRepository : FeatureRunnerRepository
    {
        public XUnit2FeatureRunnerRepository(LightBddConfiguration configuration) : base(new XUnit2IntegrationContext(configuration))
        {
        }
    }
}