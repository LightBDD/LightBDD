#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightBDD.Core.Dependencies.Implementation;

internal class DependencyContainer : IDependencyContainer
{
    private readonly DependencyResolverProvider _resolverProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDependencyResolver _resolver;

    public DependencyContainer(IServiceCollection collection)
    {
        collection = new ServiceCollection().Add(collection);
        _resolverProvider = new(this);

        collection.AddTransient<IDependencyResolver, DependencyResolver>();
        collection.AddSingleton<IDependencyResolverProvider>(_resolverProvider);
        _serviceProvider = collection.BuildServiceProvider(true);
        _resolver = _serviceProvider.GetRequiredService<IDependencyResolver>();
    }

    public object Resolve(Type type)
    {
        return _resolver.Resolve(type);
    }

    public IDependencyContainer BeginScope()
    {
        return new ScopedDependencyContainer(this, this, _serviceProvider.CreateAsyncScope());
    }

    public ValueTask DisposeAsync()
    {
        if (_serviceProvider is IAsyncDisposable disposable)
            return disposable.DisposeAsync();

        (_serviceProvider as IDisposable)?.Dispose();
        return default;
    }

    private class ScopedDependencyContainer : IDependencyContainer
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
            _root._resolverProvider.SetCurrent(this);
            _resolver = scope.ServiceProvider.GetRequiredService<IDependencyResolver>();
        }

        public async ValueTask DisposeAsync()
        {
            _root._resolverProvider.SetCurrent(_parent);
            await _scope.DisposeAsync();
        }

        public object Resolve(Type type) => _resolver.Resolve(type);

        public IDependencyContainer BeginScope()
        {
            return new ScopedDependencyContainer(_root, this, _scope.ServiceProvider.CreateAsyncScope());
        }
    }
}