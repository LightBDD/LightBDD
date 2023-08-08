#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Dependencies.Implementation;

internal class DependencyContainer : IDependencyContainer
{
    private readonly object _holdingScope;
    private readonly IServiceProvider _serviceProvider;

    public DependencyContainer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _holdingScope = serviceProvider;
    }

    private DependencyContainer(AsyncServiceScope scope)
    {
        _serviceProvider = scope.ServiceProvider;
        _holdingScope = scope;
    }

    public object Resolve(Type type)
    {
        if (type == typeof(IDependencyContainer) || type == typeof(IDependencyResolver))
            return this;

        return _serviceProvider.GetService(type) ?? CreateTransient(type);
    }

    private object CreateTransient(Type type)
    {
        try
        {
            return EnlistDisposable(DependencyDescriptor.FindConstructor(type).Invoke(this));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to create transient instance of type '{type}':{Environment.NewLine}{ex.Message}", ex);
        }
    }

    private object EnlistDisposable(object item)
    {
        if (item is IAsyncDisposable or IDisposable)
            return _serviceProvider.GetRequiredService<TransientDisposable>().WithInstance(item)!;
        return item;
    }

    public IDependencyContainer BeginScope()
    {
        return new DependencyContainer(_serviceProvider.CreateAsyncScope());
    }

    public ValueTask DisposeAsync()
    {
        if (_holdingScope is IAsyncDisposable disposable)
            return disposable.DisposeAsync();

        (_holdingScope as IDisposable)?.Dispose();
        return default;
    }
}