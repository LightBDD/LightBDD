using LightBDD.Commenting.Implementation;
using LightBDD.Core.Configuration;
using LightBDD.ExecutionContext.Configuration;

namespace LightBDD.Commenting.Configuration
{
    public static class CommentingConfiguration
    {
        public static ExecutionExtensionsConfiguration EnableStepCommenting(this ExecutionExtensionsConfiguration cfg)
        {
            return cfg
                .EnableScenarioExecutionContext()
                .EnableStepExtension<StepCommentingExtension>();
        }
    }
}
