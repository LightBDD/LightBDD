#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
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

    public IRunnableScenarioBuilder CreateBuilder(IFeatureInfo? feature = null) => _factory.CreateFor(feature ?? Fake.Object<TestResults.TestFeatureInfo>());

    public IRunnableScenarioV2 CreateScenario(Func<ICoreScenarioStepsRunner, Task> entryMethod)
    {
        return CreateBuilder().WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner)).Build();
    }

    public IRunnableScenarioV2 CreateScenario<TFixture>(Func<ICoreScenarioStepsRunner, Task> entryMethod)
    {
        var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
        featureInfo.FeatureType = typeof(TFixture);
        return CreateBuilder(featureInfo).WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner)).Build();
    }

    public IRunnableScenarioV2 CreateScenario<TFixture>(Func<TFixture, ICoreScenarioStepsRunner, Task> entryMethod)
    {
        var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
        featureInfo.FeatureType = typeof(TFixture);
        return CreateBuilder(featureInfo).WithScenarioEntryMethod((fixture, runner) => entryMethod.Invoke((TFixture)fixture, runner)).Build();
    }

    public static TestableScenarioFactory Create(Action<LightBddConfiguration>? onConfigure = null)
    {
        LightBddConfiguration cfg = new();
        TestLightBddConfiguration.SetTestDefaults(cfg);
        onConfigure?.Invoke(cfg);

        return new(new RunnableScenarioFactory(new EngineContext(typeof(TestableScenarioFactory).Assembly, cfg)));
    }
}