using System.Collections.Generic;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Extensibility.Execution.Implementation;

//TODO: internal and optimise for empty collections and singletons
public class GlobalDecoratorsProvider
{
    private readonly IDependencyResolverProvider _provider;

    public GlobalDecoratorsProvider(IDependencyResolverProvider provider)
    {
        _provider = provider;
    }

    public IEnumerable<IStepDecorator> ProvideStepDecorators()
    {
        foreach (var decorator in _provider.GetCurrent().Resolve<IEnumerable<IStepDecorator>>())
        {
            yield return decorator;
        }
    }

    public IEnumerable<IScenarioDecorator> ProvideScenarioDecorators()
    {
        foreach (var decorator in _provider.GetCurrent().Resolve<IEnumerable<IScenarioDecorator>>())
        {
            yield return decorator;
        }
    }
}