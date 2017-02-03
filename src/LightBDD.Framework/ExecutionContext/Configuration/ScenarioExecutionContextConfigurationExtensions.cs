using LightBDD.Core.Configuration;
using LightBDD.Framework.ExecutionContext.Implementation;

namespace LightBDD.Framework.ExecutionContext.Configuration
{
    /// <summary>
    /// Configuration class allowing to enable <see cref="ScenarioExecutionContext"/> feature in LightBDD.
    /// See also: <seealso cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    public static class ScenarioExecutionContextConfigurationExtensions
    {
        /// <summary>
        /// Enables <see cref="ScenarioExecutionContext"/> feature in provided <paramref name="configuration"/> object.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ExecutionExtensionsConfiguration EnableScenarioExecutionContext(this ExecutionExtensionsConfiguration configuration)
        {
            return configuration.EnableScenarioExtension<ScenarioExecutionContextExtension>();
        }
    }
}
