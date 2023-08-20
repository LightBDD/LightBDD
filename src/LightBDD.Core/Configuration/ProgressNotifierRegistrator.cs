#nullable enable
using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration;

/// <summary>
/// Registrator of <see cref="IProgressNotifier"/> implementations used to notify the execution progress of LightBDD tests.<br/>
/// Because progress notification covers the entire execution stage, all registered instances are singletons.<br/>
/// In case the implementation requires access to scoped dependencies, it should use <see cref="IDependencyResolverProvider"/>.
/// </summary>
public class ProgressNotifierRegistrator
{
    private readonly ServiceCollectionRegistrator<IProgressNotifier> _registrator;

    internal ProgressNotifierRegistrator(IServiceCollection collection)
    {
        _registrator = new(collection);
    }

    public ProgressNotifierRegistrator Add<TImplementation>() where TImplementation : IProgressNotifier
    {
        _registrator.Add<TImplementation>();
        return this;
    }

    public ProgressNotifierRegistrator Add(IProgressNotifier instance)
    {
        _registrator.Add(instance);
        return this;
    }

    public ProgressNotifierRegistrator Add(Func<IServiceProvider, IProgressNotifier> implementationFactory)
    {
        _registrator.Add(implementationFactory);
        return this;
    }

    public ProgressNotifierRegistrator Clear()
    {
        _registrator.Clear();
        return this;
    }
}