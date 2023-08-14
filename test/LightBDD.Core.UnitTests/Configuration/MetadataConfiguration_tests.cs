using System;
using LightBDD.Core.Configuration;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Configuration;

[TestFixture]
public class MetadataConfiguration_tests
{
    [Test]
    public void It_should_initialize_object_with_default_values()
    {
        var configuration = new MetadataConfiguration();
        configuration.EngineAssemblies.ShouldBe(new[] { typeof(MetadataConfiguration).Assembly });
    }

    [Test]
    public void It_should_register_assemblies_but_only_once_and_return_latest_registered_first()
    {
        var configuration = new MetadataConfiguration()
            .RegisterEngineAssembly(GetType().Assembly)
            .RegisterEngineAssembly(typeof(TestScenarioBuilder).Assembly)
            .RegisterEngineAssembly(GetType().Assembly);

        configuration.EngineAssemblies.ShouldBe(new[]
        {
            typeof(TestScenarioBuilder).Assembly,
            GetType().Assembly,
            typeof(MetadataConfiguration).Assembly
        });
    }

    [Test]
    public void Configuration_should_be_sealable()
    {
        var lightBddConfig = new LightBddConfiguration();
        var cfg = lightBddConfig.ConfigureFeature<MetadataConfiguration>();

        var engineAssemblies = cfg.EngineAssemblies;

        lightBddConfig.Seal();

        Assert.Throws<InvalidOperationException>(() => cfg.RegisterEngineAssembly(GetType().Assembly));
        cfg.EngineAssemblies.ShouldBe(engineAssemblies);
    }
}