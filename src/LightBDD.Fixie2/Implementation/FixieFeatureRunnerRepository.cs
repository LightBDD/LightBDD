using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Fixie2.Implementation
{
    internal class FixieFeatureRunnerRepository : FeatureRunnerRepository
    {
        public FixieFeatureRunnerRepository(LightBddConfiguration configuration) : base(new FixieIntegrationContext(configuration))
        {
        }
    }
}