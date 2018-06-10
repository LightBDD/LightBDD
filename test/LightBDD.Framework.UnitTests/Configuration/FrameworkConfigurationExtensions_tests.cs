using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext.Configuration;
using LightBDD.Framework.Formatting.Configuration;
using LightBDD.Framework.Formatting.Values;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Configuration
{
    [TestFixture]
    public class FrameworkConfigurationExtensions_tests
    {
        [Test]
        public void WithFrameworkDefaults_should_enable_default_step_decorators()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults().ExecutionExtensionsConfiguration();

            var expected = new LightBddConfiguration().ExecutionExtensionsConfiguration()
                .EnableStepCommenting();

            Assert.That(configuration
                    .StepDecorators
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expected
                    .StepDecorators
                    .Select(x => x.GetType())
                    .ToArray()));
        }

        [Test]
        public void WithFrameworkDefaults_should_enable_default_scenario_decorators()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults().ExecutionExtensionsConfiguration();

            var expected = new LightBddConfiguration().ExecutionExtensionsConfiguration()
                .EnableScenarioExecutionContext()
                .EnableCurrentScenarioTracking();

            Assert.That(configuration
                    .ScenarioDecorators
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expected
                    .ScenarioDecorators
                    .Select(x => x.GetType())
                    .ToArray()));
        }

        [Test]
        public void WithFrameworkDefaults_should_register_default_general_value_formatters()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults().ValueFormattingConfiguration();

            var expected = new LightBddConfiguration().ValueFormattingConfiguration()
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
            var configuration = new LightBddConfiguration().ValueFormattingConfiguration()
                .RegisterFrameworkDefaultGeneralFormatters();

            var expected = new LightBddConfiguration().ValueFormattingConfiguration()
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
    }
}