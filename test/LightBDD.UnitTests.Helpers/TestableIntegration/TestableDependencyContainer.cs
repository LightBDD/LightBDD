using System;
using System.Collections.Generic;
using LightBDD.Core.Dependencies;

namespace LightBDD.UnitTests.Helpers.TestableIntegration;

public class TestableDependencyContainer : IDependencyContainerV2
{
    private readonly Dictionary<Type, object> _items = new();

    public void Register(Type type, object dependency) => _items[type] = dependency;
    public object Resolve(Type type) => _items[type];

    public void Dispose() { }

    public IDependencyContainer BeginScope(Action<ContainerConfigurator> configuration = null) => this;

    public IDependencyContainerV2 BeginScope(LifetimeScope scope, Action<ContainerConfigurator> configuration = null) => this;
}