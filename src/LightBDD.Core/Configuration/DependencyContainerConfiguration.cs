using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Dependencies.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize DI container used by LightBDD.
    /// </summary>
    public class DependencyContainerConfiguration : FeatureConfiguration
    {
        private readonly IServiceCollection _services = new ServiceCollection()
            .AddTransient<TransientDisposable>();

        /// <summary>
        /// Configures services for the DI container.
        /// </summary>
        /// <param name="onConfigure">Configuration delegate</param>
        /// <returns>Self.</returns>
        public DependencyContainerConfiguration ConfigureServices(Action<IServiceCollection> onConfigure)
        {
            ThrowIfSealed();
            onConfigure?.Invoke(_services);
            return this;
        }

        /// <summary>
        /// Creates new instance of <see cref="IDependencyContainer"/> with applied all configurations.
        /// </summary>
        public IDependencyContainer Build()
        {
            return new DependencyContainer(_services.BuildServiceProvider(true));
        }
    }
}