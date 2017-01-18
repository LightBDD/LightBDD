using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Extensions.ContextualAsyncExecution.Implementation;

namespace LightBDD.Extensions.ContextualAsyncExecution.Configuration
{
    public static class ScenarioExecutionContextConfiguration
    {
        public static ExecutionExtensionsConfiguration EnableScenarioExecutionContext(this ExecutionExtensionsConfiguration cfg)
        {
            return cfg.EnableScenarioExtension<ScenarioExecutionContextExtension>();
        }
    }
}
