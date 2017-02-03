using LightBDD.Commenting.Implementation;
using LightBDD.Core.Configuration;
using LightBDD.ExecutionContext;
using LightBDD.ExecutionContext.Configuration;

namespace LightBDD.Commenting.Configuration
{
    /// <summary>
    /// Configuration class allowing to enable commenting feature in LightBDD.
    /// See also: <seealso cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    public static class CommentingConfigurationExtensions
    {
        /// <summary>
        /// Enables configuration feature in provided <paramref name="configuration"/> object.
        /// As this feature depends on <see cref="ScenarioExecutionContext"/>, it enables it as well with <see cref="ScenarioExecutionContextConfigurationExtensions.EnableScenarioExecutionContext"/>().
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ExecutionExtensionsConfiguration EnableStepCommenting(this ExecutionExtensionsConfiguration configuration)
        {
            return configuration
                .EnableScenarioExecutionContext()
                .EnableStepExtension<StepCommentingExtension>();
        }
    }
}
