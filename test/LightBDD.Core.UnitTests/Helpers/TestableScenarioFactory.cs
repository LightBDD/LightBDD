#nullable enable
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
        _factory = new RunnableScenarioFactory(TestableIntegrationContextBuilder.Default().Build());
    }

    public IRunnableScenarioBuilder Create(IFeatureInfo? feature = null) => _factory.CreateFor(feature ?? Fake.Object<TestResults.TestFeatureInfo>());
}