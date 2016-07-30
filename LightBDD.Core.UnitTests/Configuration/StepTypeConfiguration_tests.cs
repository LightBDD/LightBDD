using System;
using LightBDD.Configuration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class StepTypeConfiguration_tests
    {
        [Test]
        public void Should_initialize_object_with_default_values()
        {
            var configuration = new StepTypeConfiguration();
            Assert.That(configuration.RepeatedStepReplacement, Is.EqualTo("and"));
            Assert.That(configuration.PredefinedStepTypes, Is.EquivalentTo(new[] { "given", "when", "then", "and", "setup" }));
        }

        [Test]
        public void It_should_not_allow_null_predefined_step_array()
        {
            Assert.Throws<ArgumentNullException>(() => new StepTypeConfiguration().UpdatePredefinedStepTypes(null));
        }

        [Test]
        public void It_should_update_configuration()
        {
            var configuration = new StepTypeConfiguration().UpdatePredefinedStepTypes("call").UpdateRepeatedStepReplacement(null);
            Assert.That(configuration.RepeatedStepReplacement, Is.EqualTo(null));
            Assert.That(configuration.PredefinedStepTypes, Is.EquivalentTo(new[] { "call" }));
        }
    }
}