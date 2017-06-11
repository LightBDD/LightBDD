using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;

namespace LightBDD.Core.Extensibility
{
    public class LightBddConfigurationAware
    {
        protected LightBddConfigurationAware() { }
        protected LightBddConfiguration Configuration => FeatureCoordinator.GetInstance().Configuration;
    }
}
