using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Formatting.Values;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Configuration
{
    [TestFixture]
    public class FrameworkConfigurationExtensions_tests
    {
        [Test]
        public void WithFrameworkDefaults_should_enable_default_step_decorators()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            Assert.That(configuration
                    .Services.Where(x => x.ServiceType == typeof(IStepDecorator))
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.Empty);
        }

        [Test]
        public void WithFrameworkDefaults_should_enable_default_scenario_decorators()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            Assert.That(configuration
                    .Services.Where(x => x.ServiceType == typeof(IScenarioDecorator))
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.Empty);
        }

        [Test]
        public void WithFrameworkDefaults_should_register_default_general_value_formatters()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults().ForValueFormatting();

            var expected = new LightBddConfiguration().ForValueFormatting()
                .RegisterGeneral(new DictionaryFormatter())
                .RegisterGeneral(new CollectionFormatter());

            Assert.That(configuration
                    .GeneralFormatters
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expected
                    .GeneralFormatters
                    .Select(x => x.GetType())
                    .ToArray()));
        }

        [Test]
        public void RegisterFrameworkDefaultGeneralFormatters_should_register_default_general_value_formatters()
        {
            var configuration = new LightBddConfiguration().ForValueFormatting()
                .RegisterFrameworkDefaultGeneralFormatters();

            var expected = new LightBddConfiguration().ForValueFormatting()
                .RegisterGeneral(new DictionaryFormatter())
                .RegisterGeneral(new CollectionFormatter());

            Assert.That(configuration
                    .GeneralFormatters
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expected
                    .GeneralFormatters
                    .Select(x => x.GetType())
                    .ToArray()));
        }

        [Test]
        public void WithFrameworkDefaults_should_register_Framework_assembly_on_MetadataConfiguration()
        {
            new LightBddConfiguration().WithFrameworkDefaults()
                .ForMetadata().EngineAssemblies.ShouldBe(new[]
                {
                    typeof(IBddRunner).Assembly,
                    typeof(CoreMetadataProvider).Assembly
                });
        }
    }
}