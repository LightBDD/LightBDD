#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration;

public class FeatureRegistrator<TService> where TService : class
{
    private ServiceDescriptor _featureDescriptor;

    public FeatureRegistrator<TService> Use<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TImplementation : TService
    {
        _featureDescriptor = ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), lifetime);
        return this;
    }

    public FeatureRegistrator<TService> Use(TService instance)
    {
        _featureDescriptor = ServiceDescriptor.Singleton(instance);
        return this;
    }

    public FeatureRegistrator<TService> Use(Func<IServiceProvider, TService> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        _featureDescriptor = ServiceDescriptor.Describe(typeof(TService), implementationFactory, lifetime);
        return this;
    }

    public ServiceDescriptor GetDescriptor() => _featureDescriptor ?? throw new InvalidOperationException($"{nameof(_featureDescriptor)} is not configured. Configure it with one of {nameof(Use)}() methods");
}