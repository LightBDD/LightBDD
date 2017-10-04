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
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            var expectedConfiguration = new LightBddConfiguration();

            expectedConfiguration.ExecutionExtensionsConfiguration()
                .EnableStepCommenting();

            Assert.That(configuration.ExecutionExtensionsConfiguration()
                    .StepDecorators
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expectedConfiguration.ExecutionExtensionsConfiguration()
                    .StepDecorators
                    .Select(x => x.GetType())
                    .ToArray()));
        }

        [Test]
        public void WithFrameworkDefaults_should_enable_default_scenario_decorators()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            var expectedConfiguration = new LightBddConfiguration();

            expectedConfiguration.ExecutionExtensionsConfiguration()
                .EnableScenarioExecutionContext();

            Assert.That(configuration.ExecutionExtensionsConfiguration()
                    .ScenarioDecorators
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expectedConfiguration.ExecutionExtensionsConfiguration()
                    .ScenarioDecorators
                    .Select(x => x.GetType())
                    .ToArray()));
        }

        [Test]
        public void WithFrameworkDefaults_should_enable_default_value_formatters()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            var expectedConfiguration = new LightBddConfiguration();

            expectedConfiguration.ValueFormattingConfiguration()
                .RegisterGeneral(new DictionaryFormatter())
                .RegisterGeneral(new CollectionFormatter());

            Assert.That(configuration.ValueFormattingConfiguration()
                    .GeneralFormatters
                    .Select(x => x.GetType())
                    .ToArray(),
                Is.EquivalentTo(expectedConfiguration.ValueFormattingConfiguration()
                    .GeneralFormatters
                    .Select(x => x.GetType())
                    .ToArray()));
        }
    }
}
