#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightBDD.Core.Dependencies;

/// <summary>
/// Service collection registrator allowing to register the collection of services describing the same <typeparamref name="TService"/> type.
/// </summary>
/// <typeparam name="TService">Service type</typeparam>
public class ServiceCollectionRegistrator<TService> where TService : class
{
    private readonly IServiceCollection _collection;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ServiceCollectionRegistrator(IServiceCollection collection)
    {
        _collection = collection;
    }

    /// <summary>
    /// Adds <typeparamref name="TImplementation"/> service to the collection with <paramref name="lifetime"/> lifetime.
    /// </summary>
    /// <typeparam name="TImplementation">Service implementation</typeparam>
    /// <param name="lifetime">Lifetime</param>
    /// <returns>Self.</returns>
    public ServiceCollectionRegistrator<TService> Add<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TImplementation : TService
    {
        _collection.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), lifetime));
        return this;
    }

    /// <summary>
    /// Adds <paramref name="instance"/> to the collection, where the instance is configured as singleton but it's lifetime is being controlled outside of DI.
    /// </summary>
    /// <param name="instance">Instance to add</param>
    /// <returns>Self.</returns>
    public ServiceCollectionRegistrator<TService> Add(TService instance)
    {
        _collection.Add(ServiceDescriptor.Singleton(instance));
        return this;
    }

    /// <summary>
    /// Adds <typeparamref name="TService"/> to the collection, where it's instance is created with <paramref name="implementationFactory"/> and it's lifetime is specified by <paramref name="lifetime"/> parameter.
    /// </summary>
    /// <param name="implementationFactory">Implementation factory</param>
    /// <param name="lifetime">Lifetime</param>
    /// <returns>Self.</returns>
    public ServiceCollectionRegistrator<TService> Add(Func<IServiceProvider, TService> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        _collection.Add(ServiceDescriptor.Describe(typeof(TService), implementationFactory, lifetime));
        return this;
    }

    /// <summary>
    /// Clears the collection of all <typeparamref name="TService"/> definitions.
    /// </summary>
    /// <returns>Self.</returns>
    public ServiceCollectionRegistrator<TService> Clear()
    {
        _collection.RemoveAll<TService>();
        return this;
    }
}