using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Dependencies;

namespace LightBDD.Framework.Execution.Dependencies.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize name formatting behavior.
    /// </summary>
    public class DependencyContainerConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns configured <see cref="IDependencyContainer"/>.
        /// </summary>
        public IDependencyContainer DependencyContainer { get; private set; } = new SimpleDependencyContainer(); // TODO: test
    }
}