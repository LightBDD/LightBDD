using System.Collections.Generic;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Extensibility.Execution.Implementation;

/// <summary>
/// Global decorators provider
/// </summary>
//TODO: internal and optimise for empty collections and singletons
public class GlobalDecoratorsProvider
{
    private readonly IDependencyResolverProvider _provider;

    /// <summary>
    /// Default constructor
    /// </summary>
    public GlobalDecoratorsProvider(IDependencyResolverProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Provides globally registered step decorators
    /// </summary>
    public IEnumerable<IStepDecorator> ProvideStepDecorators()
    {
        foreach (var decorator in _provider.GetCurrent().Resolve<IEnumerable<IStepDecorator>>())
        {
            yield return decorator;
        }
    }

    /// <summary>
    /// Provides globally registered scenario decorators
    /// </summary>
    public IEnumerable<IScenarioDecorator> ProvideScenarioDecorators()
    {
        foreach (var decorator in _provider.GetCurrent().Resolve<IEnumerable<IScenarioDecorator>>())
        {
            yield return decorator;
        }
    }
}