using System;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalInitialization;

internal class GlobalActivitySetUp : IGlobalSetUp
{
    private readonly string _name;
    private readonly Func<Task> _setUp;
    private readonly Func<Task> _cleanUp;
    private bool _runCleanup;

    public GlobalActivitySetUp(string name, Func<Task> setUp, Func<Task> cleanUp)
    {
        _setUp = setUp;
        _cleanUp = cleanUp;
        _name = name;
        _runCleanup = setUp == null;
    }

    public async Task SetUpAsync(IDependencyResolver _)
    {
        if (_setUp == null)
            return;

        try
        {
            await _setUp();
            _runCleanup = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Set up activity '{_name}' failed: {ex.Message}", ex);
        }
    }

    public async Task CleanUpAsync(IDependencyResolver _)
    {
        try
        {
            if (_runCleanup && _cleanUp != null)
                await _cleanUp();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Clean up of activity '{_name}' failed: {ex.Message}", ex);
        }
    }
}