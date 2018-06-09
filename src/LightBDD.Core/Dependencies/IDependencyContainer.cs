using System;
using System.Collections.Generic;

namespace LightBDD.Core.Dependencies
{
    public interface IDependencyContainer : IDependencyResolver, IDisposable
    {
        IDependencyContainer BeginScope(Action<IContainerConfigurer> configuration = null);
    }

    public interface IContainerConfigurer
    {
        void RegisterInstance(object instance, RegistrationOptions options);
    }

    public class RegistrationOptions
    {
        private HashSet<Type> _asTypes = new HashSet<Type>();
        public bool IsExternallyOwned { get; private set; }
        public IEnumerable<Type> AsTypes => _asTypes;

        public RegistrationOptions ExtenrallyOwned()
        {
            IsExternallyOwned = true;
            return this;
        }

        public RegistrationOptions As(params Type[] types)
        {
            foreach (var type in types)
                _asTypes.Add(type);
            return this;
        }

        public RegistrationOptions As<T>() => As(typeof(T));
    }
}
