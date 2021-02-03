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
        void RegisterSingleton(object instance, Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as singleton using it's public constructor to instantiate it on first resolution.
        /// The instance is shared with nested scopes, unless overridden.
        /// </summary>
        void RegisterSingleton<T>(Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as singleton using <paramref name="createFn"/> to instantiate it on first resolution.
        /// The instance is shared with nested scopes, unless overridden.
        /// </summary>
        void RegisterSingleton<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as transient using it's public constructor to instantiate it on every resolve attempt.
        /// The instance is never shared between objects.
        /// </summary>
        void RegisterTransient<T>(Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as transient using <paramref name="createFn"/> to instantiate it on every resolve attempt.
        /// The instance is never shared between objects.
        /// </summary>
        void RegisterTransient<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as scenario scoped using it's public constructor to instantiate it on first resolve attempt in each scenario.
        /// The instance is shared with nested scopes, unless overridden.
        /// </summary>
        void RegisterScenarioScoped<T>(Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as transient using <paramref name="createFn"/> to instantiate it on first resolve attempt in each scenario.
        /// The instance is shared with nested scopes, unless overridden.
        /// </summary>
        void RegisterScenarioScoped<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as locally scoped using it's public constructor to instantiate it on first resolve attempt in each scope like scenario, or each composite step.
        /// The instance is not shared with nested scopes.
        /// </summary>
        void RegisterLocallyScoped<T>(Action<RegistrationOptions> options = null);
        /// <summary>
        /// Registers the given type as transient using <paramref name="createFn"/> to instantiate it on first resolve attempt in each scope like scenario, or each composite step.
        /// The instance is not shared with nested scopes.
        /// </summary>
        void RegisterLocallyScoped<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null);
    }
}