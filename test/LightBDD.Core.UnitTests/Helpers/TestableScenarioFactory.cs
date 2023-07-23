#nullable enable
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
    public static readonly TestableScenarioFactory Default = new();
    private readonly RunnableScenarioFactory _factory;

    private TestableScenarioFactory()
    {
        _factory = new RunnableScenarioFactory(TestableIntegrationContextBuilder.Default()
            //TODO: review
            .WithConfiguration(x=>x.ExceptionHandlingConfiguration().UpdateExceptionDetailsFormatter(e=>$"{e.GetType().Namespace}.{e.GetType().Name}: {e.Message}"))
            .Build());
    }

    public IRunnableScenarioBuilder CreateBuilder(IFeatureInfo? feature = null) => _factory.CreateFor(feature ?? Fake.Object<TestResults.TestFeatureInfo>());

    public IRunnableScenarioV2 CreateScenario(Func<ICoreScenarioBuilderV2,Task> entryMethod)
    {
        return CreateBuilder().WithScenarioEntryMethod((_, runner) => entryMethod.Invoke(runner)).Build();
    }
}