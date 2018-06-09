using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Dependencies.Implementation;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize name formatting behavior.
    /// </summary>
    public class DependencyContainerConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns configured <see cref="IDependencyContainer"/>.
        /// </summary>
        public IDependencyContainer DependencyContainer { get; private set; } = new BasicDependencyContainer();

        public DependencyContainerConfiguration UseContainer(IDependencyContainer container)
        {
            ThrowIfSealed();
            DependencyContainer = container;
            return this;
        }

        public DependencyContainerConfiguration UseDefaultContainer(Action<IContainerConfigurer> configurer = null)
        {
            return UseContainer(new BasicDependencyContainer(configurer));
        }
    }
}