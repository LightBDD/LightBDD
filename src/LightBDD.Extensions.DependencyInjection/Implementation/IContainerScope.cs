using System;
using LightBDD.Core.Dependencies;

namespace LightBDD.Extensions.DependencyInjection.Implementation
{
    interface IContainerScope : IDisposable
    {
        object Resolve(Type type);
        IContainerScope BeginScope(LifetimeScope scope);
    }
}