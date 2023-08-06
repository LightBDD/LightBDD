#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.UnitTests.Helpers;

namespace LightBDD.Core.UnitTests.Helpers;

internal class TestableScenarioFactory
{
    public static readonly TestableScenarioFactory Default = Create();
    private readonly RunnableScenarioFactory _factory;

    private TestableScenarioFactory(RunnableScenarioFactory factory)
    {
        _factory = factory;
    }

    public static TestableScenarioFactory Create(Action<LightBddConfiguration>? onConfigure = null)
    {
        LightBddConfiguration cfg = new();
        TestLightBddConfiguration.SetTestDefaults(cfg);
        onConfigure?.Invoke(cfg);

        return new(new RunnableScenarioFactory(new EngineContext(cfg)));
    }

    public IRunnableScenarioBuilder CreateBuilder(IFeatureInfo? feature = null) => _factory.CreateFor(feature ?? Fake.Object<TestResults.TestFeatureInfo>());

    public Task<IScenarioResult> RunScenario(Func<ICoreScenarioStepsRunner, Task> entryMethod)
        => CreateBuilder()
            .WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner))
            .Build()
            .RunAsync();

    public Task<IScenarioResult> RunScenario<TFixture>(Func<ICoreScenarioStepsRunner, Task> entryMethod)
    {
        var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
        featureInfo.FeatureType = typeof(TFixture);
        return CreateBuilder(featureInfo)
            .WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner))
            .Build()
            .RunAsync();
    }

    public Task<IScenarioResult> RunScenario<TFixture>(Func<TFixture, ICoreScenarioStepsRunner, Task> entryMethod)
    {
        var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
        featureInfo.FeatureType = typeof(TFixture);
        return CreateBuilder(featureInfo)
            .WithScenarioEntryMethod((fixture, runner) => entryMethod.Invoke((TFixture)fixture, runner))
            .Build()
            .RunAsync();
    }
}