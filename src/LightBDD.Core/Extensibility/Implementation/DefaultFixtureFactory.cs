using System;
using System.Reflection;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Extensibility.Implementation;

internal class DefaultFixtureFactory : IFixtureFactory
{
    private readonly IDependencyResolverProvider _provider;

    public DefaultFixtureFactory(IDependencyResolverProvider provider)
    {
        _provider = provider;
    }

    public object Create(Type fixtureType)
    {
        try
        {
            return _provider.GetCurrent().Resolve(fixtureType);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw new InvalidOperationException($"Initialization of {fixtureType.Name} fixture failed: {ex.InnerException.Message}", ex.InnerException);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Initialization of {fixtureType.Name} fixture failed: {ex.Message}", ex);
        }
    }
}