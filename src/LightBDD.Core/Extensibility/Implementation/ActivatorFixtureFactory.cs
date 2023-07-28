using System;
using System.Reflection;

namespace LightBDD.Core.Extensibility.Implementation;

internal class ActivatorFixtureFactory : IFixtureFactory
{
    public static readonly ActivatorFixtureFactory Instance = new();

    private ActivatorFixtureFactory() { }

    public object Create(Type fixtureType)
    {
        try
        {
            return Activator.CreateInstance(fixtureType) ?? throw new InvalidOperationException($"Initialization of {fixtureType.Name} fixture failed: Instance is null");
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