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

    public async Task DisposeAsync(ExecutionStatusCollector collector)
    {
        if (Fixture == null)
            return;
        if (Fixture is IScenarioTearDown tearDown)
            await collector.Capture(() => TearDown(tearDown));
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