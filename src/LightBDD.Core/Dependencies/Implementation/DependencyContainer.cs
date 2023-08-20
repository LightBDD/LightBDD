#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Dependencies.Implementation;

internal class DependencyContainer : IDependencyContainer
{
    public readonly DependencyResolverProvider ResolverProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDependencyResolver _resolver;

    public DependencyContainer(IServiceCollection collection)
    {
        ResolverProvider = new(this);
        collection.AddTransient<IDependencyResolver, DependencyResolver>();
        collection.AddSingleton<IDependencyResolverProvider>(ResolverProvider);
        _serviceProvider = collection.BuildServiceProvider(true);
        _resolver = _serviceProvider.GetRequiredService<IDependencyResolver>();
    }

    public object Resolve(Type type) => _resolver.Resolve(type);
    public TDependency Resolve<TDependency>() => _resolver.Resolve<TDependency>();
    public IDependencyContainer BeginScope() => new ScopedDependencyContainer(this, this, _serviceProvider.CreateAsyncScope());

    public ValueTask DisposeAsync()
    {
        if (_serviceProvider is IAsyncDisposable disposable)
            return disposable.DisposeAsync();

        (_serviceProvider as IDisposable)?.Dispose();
        return default;
    }
}