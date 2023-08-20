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

    /// <summary>
    /// Adds progress notifier of <typeparamref name="TImplementation"/> type to the collection.
    /// </summary>
    /// <typeparam name="TImplementation">Progress notifier type</typeparam>
    /// <returns>Self.</returns>
    public ProgressNotifierRegistrator Add<TImplementation>() where TImplementation : IProgressNotifier
    {
        _registrator.Add<TImplementation>();
        return this;
    }

    /// <summary>
    /// Adds <paramref name="instance"/> to the collection, where the instance is configured as singleton but it's lifetime is being controlled outside of DI.
    /// </summary>
    /// <param name="instance">Instance to add</param>
    /// <returns>Self.</returns>
    public ProgressNotifierRegistrator Add(IProgressNotifier instance)
    {
        _registrator.Add(instance);
        return this;
    }

    /// <summary>
    /// Adds <see cref="IProgressNotifier"/> to the collection, where it's instance is created with <paramref name="implementationFactory"/>.
    /// </summary>
    /// <param name="implementationFactory">Implementation factory</param>
    /// <returns>Self.</returns>
    public ProgressNotifierRegistrator Add(Func<IServiceProvider, IProgressNotifier> implementationFactory)
    {
        _registrator.Add(implementationFactory);
        return this;
    }

    /// <summary>
    /// Clears the collection of all <see cref="IProgressNotifier"/> definitions.
    /// </summary>
    /// <returns>Self.</returns>
    public ProgressNotifierRegistrator Clear()
    {
        _registrator.Clear();
        return this;
    }
}