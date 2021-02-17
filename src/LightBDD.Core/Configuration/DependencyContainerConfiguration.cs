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
        [Obsolete("Please migrate to " + nameof(IDependencyContainerV2) + " implementations")]
        public DependencyContainerConfiguration UseContainer(IDependencyContainer container) =>
            UseContainer(new WrappingDependencyContainer(container));

        /// <summary>
        /// Sets <paramref name="container"/> as a container to be used by LightBDD scenarios and steps.
        /// </summary>
        /// <param name="container">Container to use.</param>
        /// <returns>Self.</returns>
        public DependencyContainerConfiguration UseContainer(IDependencyContainerV2 container)
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
        /// * it allows to resolve types (classes and structures) with 1 public constructor,<br/>
        /// * it supports constructor dependency injections,<br/>
        /// * it supports singleton registrations with <see cref="ContainerConfigurator"/>,<br/>
        /// * it supports disposal of dependencies upon disposal, if dependency implements <see cref="IDisposable"/> interface,<br/>
        /// * it supports container scopes.
        /// </summary>
        /// <param name="configurator">Configuration function.</param>
        /// <returns>Self.</returns>
        [Obsolete("Please migrate to " + nameof(UseDefault) + "() method instead")]
        public DependencyContainerConfiguration UseDefaultContainer(Action<ContainerConfigurator>? configurator = null)
        {
            return UseContainer(new DefaultDependencyContainer(LifetimeScope.Global, configurator));
        }

        /// <summary>
        /// Configures the LightBDD to use it's default implementation of DI container.
        /// If specified, the <paramref name="configurator"/> function is used to configure the container.<br/>
        ///
        /// The default DI container features are:<br/>
        /// * it allows to resolve types (classes and structures) with 1 public constructor,<br/>
        /// * it supports constructor dependency injections,<br/>
        /// * it supports singleton registrations with <see cref="ContainerConfigurator"/>,<br/>
        /// * it supports disposal of dependencies upon disposal, if dependency implements <see cref="IDisposable"/> interface,<br/>
        /// * it supports container scopes.
        /// </summary>
        /// <param name="configurator">Configuration function.</param>
        /// <returns>Self.</returns>
        public DependencyContainerConfiguration UseDefault(Action<IDefaultContainerConfigurator> configurator = null)
        {
            return UseContainer(new DefaultDependencyContainer(LifetimeScope.Global, configurator));
        }
    }
}