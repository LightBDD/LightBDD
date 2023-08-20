#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Dependencies.Implementation;

internal class DependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object Resolve(Type type)
    {
        var item = _serviceProvider.GetService(type);
        if (item != null)
            return item;

        try
        {
            item = TransientDependencyFactory.Create(type, this);
            return item is IAsyncDisposable or IDisposable
                ? _serviceProvider.GetRequiredService<TransientDisposable>().WithInstance(item)!
                : item;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to create transient instance of type '{type}':{Environment.NewLine}{ex.Message}", ex);
        }
    }
}