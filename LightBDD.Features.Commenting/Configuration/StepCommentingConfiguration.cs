using LightBDD.Extensions.ContextualAsyncExecution.Configuration;
using LightBDD.Implementation;

namespace LightBDD.Configuration
{
    public static class StepCommentingConfiguration
    {
        public static ExecutionExtensionsConfiguration EnableStepCommenting(this ExecutionExtensionsConfiguration cfg)
        {
            return cfg
                .EnableScenarioExecutionContext()
                .EnableStepExtension<StepCommentingExtension>();
        }
    }
}
