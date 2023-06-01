using System;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalInitialization;

internal class GlobalResourceSetUp<TDependency> : IGlobalSetUp where TDependency : IGlobalResourceSetUp
{
    private bool _executed;

    public async Task SetUpAsync(IDependencyResolver resolver)
    {
        try
        {
            await resolver.Resolve<TDependency>().SetUpAsync();
            _executed = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Set up of resource '{typeof(TDependency).Name}' failed: {ex.Message}", ex);
        }
    }

    public async Task CleanUpAsync(IDependencyResolver resolver)
    {
        try
        {
            if (_executed)
                await resolver.Resolve<TDependency>().CleanUpAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Clean up of resource '{typeof(TDependency).Name}' failed: {ex.Message}", ex);
        }
    }
}