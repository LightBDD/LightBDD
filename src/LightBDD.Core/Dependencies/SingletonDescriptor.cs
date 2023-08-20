#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Dependencies;

/// <summary>
/// Singleton descriptor allowing to select implementation for the <typeparamref name="TService"/> service.<br/>
/// </summary>
/// <typeparam name="TService">Service type</typeparam>
public class SingletonDescriptor<TService> where TService : class
{
    private ServiceDescriptor? _featureDescriptor;

    /// <summary>
    /// Uses <typeparamref name="TImplementation"/> as the implementation for the <typeparamref name="TService"/> feature, using DI to resolve and control the singleton instance lifetime.
    /// </summary>
    /// <typeparam name="TImplementation">Implementation type to use</typeparam>
    /// <returns>Self.</returns>
    public SingletonDescriptor<TService> Use<TImplementation>() where TImplementation : TService
    {
        _featureDescriptor = ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
        return this;
    }

    /// <summary>
    /// Uses <paramref name="instance"/> as the implementation for the <typeparamref name="TService"/> feature, where the instance lifetime is controlled outside of DI.
    /// </summary>
    /// <param name="instance">Instance to use</param>
    /// <returns>Self.</returns>
    public SingletonDescriptor<TService> Use(TService instance)
    {
        _featureDescriptor = ServiceDescriptor.Singleton(instance);
        return this;
    }

    /// <summary>
    /// Uses <paramref name="implementationFactory"/> to create the implementation for the <typeparamref name="TService"/> feature, using DI to control the singleton instance lifetime.
    /// </summary>
    /// <param name="implementationFactory">Implementation factory</param>
    /// <returns>Self.</returns>
    public SingletonDescriptor<TService> Use(Func<IServiceProvider, TService> implementationFactory)
    {
        _featureDescriptor = ServiceDescriptor.Describe(typeof(TService), implementationFactory, ServiceLifetime.Singleton);
        return this;
    }

    /// <summary>
    /// Returns <see cref="ServiceDescriptor"/> representing the registration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no implementation was configured with Use() methods./></exception>
    public ServiceDescriptor GetDescriptor() => _featureDescriptor ?? throw new InvalidOperationException($"{nameof(_featureDescriptor)} is not configured. Configure it with one of {nameof(Use)}() methods");
}