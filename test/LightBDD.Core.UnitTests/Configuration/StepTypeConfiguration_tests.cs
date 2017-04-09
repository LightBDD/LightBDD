using System;
using LightBDD.Core.Configuration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class StepTypeConfiguration_tests
    {
        [Test]
        public void It_should_initialize_object_with_default_values()
        {
            var configuration = new StepTypeConfiguration();
            Assert.That(configuration.RepeatedStepReplacement, Is.EqualTo("and"));
            Assert.That(configuration.PredefinedStepTypes, Is.EquivalentTo(new[] { "given", "when", "then", "and", "setup" }));

            Assert.That(configuration.UseLambdaNameAsStepType("_"), Is.False);
            Assert.That(configuration.UseLambdaNameAsStepType("x"), Is.False);
            Assert.That(configuration.UseLambdaNameAsStepType("xx"), Is.True);
            Assert.That(configuration.UseLambdaNameAsStepType("__"), Is.True);
        }

        [Test]
        public void It_should_not_allow_updating_with_null_predefined_step_array()
        {
            Assert.Throws<ArgumentNullException>(() => new StepTypeConfiguration().UpdatePredefinedStepTypes(null));
        }

        [Test]
        public void It_should_not_allow_updating_with_null_UseLambdaNameAsStepType()
        {
            Assert.Throws<ArgumentNullException>(() => new StepTypeConfiguration().UpdateUseLambdaNameAsStepType(null));
        }

        [Test]
        public void It_should_update_configuration()
        {
            var configuration = new StepTypeConfiguration()
                .UpdatePredefinedStepTypes("call")
                .UpdateRepeatedStepReplacement(null)
                .UpdateUseLambdaNameAsStepType(name => name == "test");

            Assert.That(configuration.RepeatedStepReplacement, Is.EqualTo(null));
            Assert.That(configuration.PredefinedStepTypes, Is.EquivalentTo(new[] { "call" }));
            Assert.That(configuration.UseLambdaNameAsStepType("test"), Is.True);
            Assert.That(configuration.UseLambdaNameAsStepType("test2"), Is.False);
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<StepTypeConfiguration>();

            var repeatedStepReplacement = cfg.RepeatedStepReplacement;
            var predefinedStepTypes = cfg.PredefinedStepTypes;
            var useLambdaNameAsStepType = cfg.UseLambdaNameAsStepType;

            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.UpdatePredefinedStepTypes("abc"));
            Assert.Throws<InvalidOperationException>(() => cfg.UpdateRepeatedStepReplacement("something"));
            Assert.Throws<InvalidOperationException>(() => cfg.UpdateUseLambdaNameAsStepType(name => false));

            Assert.That(cfg.RepeatedStepReplacement, Is.EqualTo(repeatedStepReplacement));
            Assert.That(cfg.PredefinedStepTypes, Is.EqualTo(predefinedStepTypes));
            Assert.That(cfg.UseLambdaNameAsStepType, Is.EqualTo(useLambdaNameAsStepType));
        }
    }
}