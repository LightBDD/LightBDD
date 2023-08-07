using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Dependencies.Implementation;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize DI container used by LightBDD.
    /// </summary>
    public class DependencyContainerConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns configured <see cref="IDependencyContainer"/>.
        /// </summary>
        public IDependencyContainer DependencyContainer { get; private set; } = new DefaultDependencyContainer(LifetimeScope.Global);

        /// <summary>
        /// Sets <paramref name="container"/> as a container to be used by LightBDD scenarios and steps.
        /// </summary>
        /// <param name="container">Container to use.</param>
        /// <returns>Self.</returns>
        public DependencyContainerConfiguration UseContainer(IDependencyContainer container)
        {
            ThrowIfSealed();
            DependencyContainer = container;
            return this;
        }

        /// <summary>
        /// Configures the LightBDD to use it's default implementation of DI container.
        /// If specified, the <paramref name="configurator"/> function is used to configure the container.<br/>
        ///
        /// The default DI container features are:<br/>
        /// * automatic resolution of types (classes and structures) with 1 public constructor,<br/>
        /// * customizable resolution of types using factory method,<br/>
        /// * constructor dependency injections,<br/>
        /// * single, scenario, local and transient <see cref="InstanceScope"/> registrations,<br/>
        /// * disposal of dependencies implementing <see cref="IDisposable"/> interface,<br/>
        /// * container <see cref="LifetimeScope"/> to manage instances within global, scenario, nested local scopes.
        /// </summary>
        /// <param name="configurator">Configuration function.</param>
        /// <returns>Self.</returns>
        public DependencyContainerConfiguration UseDefault(Action<IDefaultContainerConfigurator> configurator = null)
        {
            return UseContainer(new DefaultDependencyContainer(LifetimeScope.Global, configurator));
        }
    }
}