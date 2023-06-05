using System;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalInitialization;

internal class GlobalActivitySetUp : IGlobalSetUp
{
    private readonly string _name;
    private readonly Func<Task> _setUp;
    private readonly Func<Task> _tearDown;
    private bool _runTearDown;

    public GlobalActivitySetUp(string name, Func<Task> setUp, Func<Task> tearDown)
    {
        _setUp = setUp;
        _tearDown = tearDown;
        _name = name;
        _runTearDown = setUp == null;
    }

    public async Task SetUpAsync(IDependencyResolver _)
    {
        if (_setUp == null)
            return;

        try
        {
            await _setUp();
            _runTearDown = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Set up activity '{_name}' failed: {ex.Message}", ex);
        }
    }

    public async Task TearDownAsync(IDependencyResolver _)
    {
        try
        {
            if (_runTearDown && _tearDown != null)
                await _tearDown();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Tear down activity '{_name}' failed: {ex.Message}", ex);
        }
    }
}