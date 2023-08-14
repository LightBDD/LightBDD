#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightBDD.Core.Configuration;

public class FeatureCollectionRegistrator<TService> where TService : class
{
    private readonly LightBddConfiguration _cfg;

    public FeatureCollectionRegistrator(LightBddConfiguration cfg)
    {
        _cfg = cfg;
    }

    public FeatureCollectionRegistrator<TService> Add<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TImplementation : TService
    {
        _cfg.Services.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), lifetime));
        return this;
    }

    public FeatureCollectionRegistrator<TService> Add(TService instance)
    {
        _cfg.Services.Add(ServiceDescriptor.Singleton(instance));
        return this;
    }

    public FeatureCollectionRegistrator<TService> Add(Func<IServiceProvider, TService> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        _cfg.Services.Add(ServiceDescriptor.Describe(typeof(TService), implementationFactory, lifetime));
        return this;
    }

    public FeatureCollectionRegistrator<TService> Clear()
    {
        _cfg.Services.RemoveAll<TService>();
        return this;
    }
}