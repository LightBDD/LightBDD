#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Dependencies.Implementation;

internal class ScopedDependencyContainer : IDependencyContainer
{
    private readonly AsyncServiceScope _scope;
    private readonly DependencyContainer _root;
    private readonly IDependencyContainer _parent;
    private readonly IDependencyResolver _resolver;

    public ScopedDependencyContainer(DependencyContainer root, IDependencyContainer parent, AsyncServiceScope scope)
    {
        _root = root;
        _parent = parent;
        _scope = scope;
        _root.ResolverProvider.SetCurrent(this);
        _resolver = scope.ServiceProvider.GetRequiredService<IDependencyResolver>();
    }

    public async ValueTask DisposeAsync()
    {
        _root.ResolverProvider.SetCurrent(_parent);
        await _scope.DisposeAsync();
    }

    public object Resolve(Type type) => _resolver.Resolve(type);
    public TDependency Resolve<TDependency>() => _resolver.Resolve<TDependency>();
    public IDependencyContainer BeginScope() => new ScopedDependencyContainer(_root, this, _scope.ServiceProvider.CreateAsyncScope());
}