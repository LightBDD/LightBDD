using System;

namespace LightBDD.Core.Execution.Dependencies
{
    public interface IDependencyContainer : IDependencyResolver, IDisposable
    {
        IDependencyContainer BeginScope();
    }
}
