using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Dependencies.Configuration;

namespace LightBDD.Framework.Dependencies
{
    public class DefaultDependencyContainer : IDependencyContainer
    {
        private readonly DefaultDependencyContainer _parent;

        public DefaultDependencyContainer(Action<IDefaultContainerConfigurer> configuration = null) : this(null, configuration)
        {
        }

        private DefaultDependencyContainer(
            DefaultDependencyContainer parent,
            Action<IDefaultContainerConfigurer> configuration = null)
        {
            _parent = parent;
            var cfg = new DefaultContainerConfigurer();
            configuration?.Invoke(cfg);
        }

        public async Task<object> ResolveAsync(Type type)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDependencyContainer BeginScope(Action<IDefaultContainerConfigurer> configuration = null)
        {
            return new DefaultDependencyContainer(this, configuration);
        }

        IDependencyContainer IDependencyContainer.BeginScope(Action<IContainerConfigurer> configuration)
        {
            return new DefaultDependencyContainer(this, configuration);
        }

    }

    internal class DefaultContainerConfigurer : IDefaultContainerConfigurer
    {
        private readonly Stack<Registration> _registrations = new Stack<Registration>();

        public IEnumerable<Registration> Registrations => _registrations;

        public void RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var opt = new RegistrationOptions<object>().AsType(instance.GetType()).SingleInstance();

            if (!options.TakeOwnership)
                opt.ExternallyOwned();

            _registrations.Push(new Registration(
                r => Task.FromResult(instance),
                opt));
        }

        public IDefaultContainerConfigurer Register<T>(Action<RegistrationOptions<T>> options)
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (!(typeInfo.IsClass || typeInfo.IsValueType) || typeInfo.IsAbstract)
                throw new InvalidOperationException($"Type {typeof(T)} cannot be instantiated: Only non-abstract classes and value types are supported");

            var ctor = typeInfo
                .DeclaredConstructors
                .Where(c => c.IsPublic)
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (ctor == null)
                throw new InvalidOperationException($"No public constructor found for type: {typeof(T)}");

            _registrations.Push(new Registration(
                new ConstructorActivator(ctor).ActivateAsync,
                new RegistrationOptions<T>().ConfigureWith(options)));
            return this;
        }

        public IDefaultContainerConfigurer Register<T>(Func<IDependencyResolver, Task<T>> resolveFn, Action<RegistrationOptions<T>> options)
        {
            _registrations.Push(new Registration(
                async r => await resolveFn(r),
                new RegistrationOptions<T>().ConfigureWith(options)));
            return this;
        }

        public IDefaultContainerConfigurer Register<T>(Func<T> resolveFn, Action<RegistrationOptions<T>> options)
        {
            _registrations.Push(new Registration(
                r => Task.FromResult((object)resolveFn()),
                new RegistrationOptions<T>().ConfigureWith(options)));
            return this;
        }

        public IDefaultContainerConfigurer RegisterInstance<T>(T instance, Action<RegistrationOptions<T>> options)
        {
            var opt = new RegistrationOptions<T>()
                .SingleInstance()
                .ConfigureWith(options);

            if (opt.InstantiationOptions != InstantiationOptions.Single)
                throw new InvalidOperationException($"{nameof(RegisterInstance)} can be only registered as singleton.");

            _registrations.Push(new Registration(
                r => Task.FromResult((object)instance),
                opt));
            return this;
        }
    }

    internal class ConstructorActivator
    {
        private readonly ConstructorInfo _ctor;
        private Type[] _params;

        public ConstructorActivator(ConstructorInfo ctor)
        {
            _ctor = ctor;
            _params = ctor.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        public async Task<object> ActivateAsync(IDependencyResolver resolver)
        {
            return _ctor.Invoke(await Task.WhenAll(_params.Select(resolver.ResolveAsync)));
        }
    }

    internal class Registration
    {
        public Func<IDependencyResolver, Task<object>> ResolveFn { get; }
        public IDefaultContainerRegistrationOptions Options { get; }

        public Registration(Func<IDependencyResolver, Task<object>> resolveFn, IDefaultContainerRegistrationOptions options)
        {
            ResolveFn = resolveFn;
            Options = options;
        }
    }
}
