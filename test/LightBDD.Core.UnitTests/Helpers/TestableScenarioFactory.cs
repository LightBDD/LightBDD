﻿#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;

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

    public IRunnableScenarioV2 CreateScenario(Func<ICoreScenarioBuilderV2, Task> entryMethod)
    {
        return CreateBuilder().WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner)).Build();
    }

    public IRunnableScenarioV2 CreateScenario<TFixture>(Func<ICoreScenarioBuilderV2, Task> entryMethod)
    {
        var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
        featureInfo.FeatureType = typeof(TFixture);
        return CreateBuilder(featureInfo).WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner)).Build();
    }

    public static TestableScenarioFactory Create(Action<LightBddConfiguration>? onConfigure = null)
    {
        void Configure(LightBddConfiguration x)
        {
            //TODO: review
            x.ExceptionHandlingConfiguration().UpdateExceptionDetailsFormatter(e => $"{e.GetType().Namespace}.{e.GetType().Name}: {e.Message}");
            onConfigure?.Invoke(x);
        }

        return new(new RunnableScenarioFactory(TestableIntegrationContextBuilder.Default()
            .WithConfiguration(Configure)
            .Build()));
    }
}