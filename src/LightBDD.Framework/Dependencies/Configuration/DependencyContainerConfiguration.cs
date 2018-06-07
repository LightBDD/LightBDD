using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;

namespace LightBDD.Framework.Dependencies.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize name formatting behavior.
    /// </summary>
    public class DependencyContainerConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns configured <see cref="IDependencyContainer"/>.
        /// </summary>
        public IDependencyContainer DependencyContainer { get; private set; } = new DefaultDependencyContainer();

        public DependencyContainerConfiguration Configure(Action<IDefaultContainerConfigurer> configuration)
        {
            ThrowIfSealed();
            DependencyContainer = new DefaultDependencyContainer(configuration);
            return this;
        }
    }

    public interface IDefaultContainerConfigurer : IContainerConfigurer
    {
        IDefaultContainerConfigurer Register<T>(Action<RegistrationOptions<T>> options = null);
        IDefaultContainerConfigurer Register<T>(Func<IDependencyResolver, Task<T>> resolveFn, Action<RegistrationOptions<T>> options = null);
        IDefaultContainerConfigurer Register<T>(Func<T> resolveFn, Action<RegistrationOptions<T>> options = null);
        IDefaultContainerConfigurer RegisterInstance<T>(T instance, Action<RegistrationOptions<T>> options = null);
    }

    public class RegistrationOptions<T> : IDefaultContainerRegistrationOptions
    {
        public bool IsExternallyOwned { get; private set; }
        public InstantiationOptions InstantiationOptions { get; private set; } = InstantiationOptions.PerDependency;
        public IEnumerable<Type> AsTypes => _asTypes;
        private HashSet<Type> _asTypes = new HashSet<Type>();

        public RegistrationOptions<T> ExternallyOwned()
        {
            IsExternallyOwned = true;
            return this;
        }

        public RegistrationOptions<T> AsSelf()
        {
            _asTypes.Add(typeof(T));
            return this;
        }

        public RegistrationOptions<T> SingleInstance()
        {
            InstantiationOptions = InstantiationOptions.Single;
            return this;
        }

        public RegistrationOptions<T> InstancePerScope()
        {
            InstantiationOptions = InstantiationOptions.PerScope;
            return this;
        }
        public RegistrationOptions<T> InstancePerDependency()
        {
            InstantiationOptions = InstantiationOptions.PerDependency;
            return this;
        }

        public RegistrationOptions<T> As<TBase>()
        {
            if (!typeof(TBase).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
                throw new InvalidOperationException($"Type {typeof(TBase)} has to be assignable from {typeof(T)}");
            _asTypes.Add(typeof(TBase));
            return this;
        }

        internal RegistrationOptions<T> ConfigureWith(Action<RegistrationOptions<T>> cfg)
        {
            cfg?.Invoke(this);
            return this;
        }

        internal RegistrationOptions<T> AsType(Type type)
        {
            _asTypes.Add(type);
            return this;
        }
    }

    public enum InstantiationOptions
    {
        Single,
        PerScope,
        PerDependency
    }

    public interface IDefaultContainerRegistrationOptions
    {
        bool IsExternallyOwned { get; }
        IEnumerable<Type> AsTypes { get; }
        InstantiationOptions InstantiationOptions { get; }
    }
}