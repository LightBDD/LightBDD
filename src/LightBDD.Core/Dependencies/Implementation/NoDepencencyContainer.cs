using System;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal class NoDepencencyContainer : IDependencyContainer
    {
        public static IDependencyContainer Instance { get; } = new NoDepencencyContainer();

        private NoDepencencyContainer() { }
        public object Resolve(Type type) => throw new NotImplementedException();

        public void Dispose() { }

        public IDependencyContainer BeginScope(Action<ContainerConfigurator>? configuration = null) => throw new NotImplementedException();
    }
}