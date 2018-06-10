using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext;
using LightBDD.Framework.ExecutionContext.Configuration;

namespace LightBDD.Framework.Commenting.Configuration
{
    /// <summary>
    /// Configuration class allowing to enable commenting feature in LightBDD.
    /// See also: <seealso cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    public static class CommentingConfigurationExtensions
    {
        /// <summary>
        /// Enables ability to comment steps.
        /// This feature enables <see cref="FrameworkConfigurationExtensions.EnableCurrentScenarioTracking"/>() as it depends on it.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ExecutionExtensionsConfiguration EnableStepCommenting(this ExecutionExtensionsConfiguration configuration)
        {
            return configuration.EnableCurrentScenarioTracking();
        }
    }
}
