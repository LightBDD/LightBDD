using LightBDD.Core.Configuration;
using LightBDD.ExecutionContext.Implementation;

namespace LightBDD.ExecutionContext.Configuration
{
    public static class ScenarioExecutionContextConfiguration
    {
        public static ExecutionExtensionsConfiguration EnableScenarioExecutionContext(this ExecutionExtensionsConfiguration cfg)
        {
            return cfg.EnableScenarioExtension<ScenarioExecutionContextExtension>();
        }
    }
}
