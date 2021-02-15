using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Default LightBDD Container Configurator
    /// </summary>
    public interface IDefaultContainerConfigurator
    {
        /// <summary>
        /// Registers the given instance as singleton.
        /// The instance is shared with nested scopes, unless overridden.
        /// </summary>
        void RegisterInstance(object instance, Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type using it's public constructor to instantiate it in given instance scope.
        /// </summary>
        void RegisterType<T>(InstanceScope scope, Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type using <paramref name="createFn"/> to instantiate it in given instance scope.
        /// </summary>
        void RegisterType<T>(InstanceScope scope, Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null);
    }
}