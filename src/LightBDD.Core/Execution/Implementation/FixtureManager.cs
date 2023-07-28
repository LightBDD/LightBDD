#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Execution.Implementation;

internal class FixtureManager
{
    private readonly IFixtureFactory _fixtureFactory;
    public object? Fixture { get; private set; }

    public FixtureManager(IFixtureFactory fixtureFactory)
    {
        _fixtureFactory = fixtureFactory;
    }

    public async Task InitializeAsync(Type fixtureType)
    {
        Fixture = _fixtureFactory.Create(fixtureType);
        if(Fixture is IScenarioSetUp setup)
            await SetUp(setup);
    }

    private async Task SetUp(IScenarioSetUp setup)
    {
        try
        {
            await setup.OnScenarioSetUp();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"OnScenarioSetUp() failed: {ex.Message}", ex);
        }
    }

    public async Task DisposeAsync(ExceptionCollector collector)
    {
        if (Fixture == null)
            return;
        if (Fixture is IScenarioTearDown tearDown)
            await collector.Capture(() => TearDown(tearDown));
        if (Fixture is IAsyncDisposable asyncDisposable)
            await collector.Capture(() => DisposeAsync(asyncDisposable));
        if (Fixture is IDisposable disposable)
            collector.Capture(() => Dispose(disposable));
    }

    private void Dispose(IDisposable disposable)
    {
        try
        {
            disposable.Dispose();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Fixture Dispose() failed: {ex.Message}", ex);
        }
    }

    private async Task DisposeAsync(IAsyncDisposable asyncDisposable)
    {
        try
        {
            await asyncDisposable.DisposeAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Fixture DisposeAsync() failed: {ex.Message}", ex);
        }
    }

    private static async Task TearDown(IScenarioTearDown tearDown)
    {
        try
        {
            await tearDown.OnScenarioTearDown();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Fixture OnScenarioTearDown() failed: {ex.Message}", ex);
        }
    }
}