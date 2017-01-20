using System.Threading.Tasks;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_step_type_interpretation_tests
    {
        [Test]
        [TestCase("given", "", "GIVEN")]
        [TestCase("when", "", "WHEN")]
        [TestCase("when", "given", "WHEN")]
        [TestCase("then", "", "THEN")]
        [TestCase("then", "when", "THEN")]
        [TestCase("setup", "", "SETUP")]

        [TestCase("given", "given", "AND")]
        [TestCase("when", "when", "AND")]
        [TestCase("then", "then", "AND")]
        [TestCase("setup", "setup", "AND")]
        [TestCase("setup", "seTup", "AND")]

        [TestCase("\t\n\r something\t\n\r ", "", "SOMETHING")]
        public void GetStepName_should_properly_convert_predefined_step_type(string inputStepType, string lastStepType, string expectedStepType)
        {
            var metadataProvider = new TestMetadataProvider(new DefaultNameFormatter());

            var descriptor = new StepDescriptor(inputStepType, "some_test_method", (o, a) => Task.CompletedTask);
            var stepName = metadataProvider.GetStepName(descriptor, lastStepType);
            Assert.That(stepName.StepTypeName, Is.EqualTo(expectedStepType));
        }

        [Test]
        [TestCase("given_my_method", "setup", "GIVEN", "my method")]
        [TestCase("given_my_method", "given", "AND", "my method")]

        [TestCase("setup_my_method", "", "SETUP", "my method")]
        [TestCase("setup_my_method", "setup", "AND", "my method")]

        [TestCase("when_my_method", "given", "WHEN", "my method")]
        [TestCase("when_my_method", "when", "AND", "my method")]

        [TestCase("then_my_method", "when", "THEN", "my method")]
        [TestCase("then_my_method", "then", "AND", "my method")]

        [TestCase("else_my_method", "", null, "else my method")]
        [TestCase("else_my_method", "else", null, "else my method")]
        public void GetStepName_should_properly_infer_default_step_type_from_method_name(string methodName, string lastStepType, string expectedStepType, string expectedStepNameFormat)
        {
            var metadataProvider = new TestMetadataProvider(new DefaultNameFormatter());

            var descriptor = new StepDescriptor(methodName, (o, a) => Task.CompletedTask);
            var stepName = metadataProvider.GetStepName(descriptor, lastStepType);
            Assert.That(stepName.StepTypeName, Is.EqualTo(expectedStepType));
            Assert.That(stepName.NameFormat, Is.EqualTo(expectedStepNameFormat));
        }

        [Test]
        public void GetStepName_should_not_extract_step_type_if_nothing_is_left_after_it()
        {
            var metadataProvider = new TestMetadataProvider(new DefaultNameFormatter());

            var descriptor = new StepDescriptor("given", (o, a) => Task.CompletedTask);
            var stepName = metadataProvider.GetStepName(descriptor, null);
            Assert.That(stepName.StepTypeName, Is.EqualTo(null));
            Assert.That(stepName.NameFormat, Is.EqualTo("given"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \r\n\t")]
        public void Should_disable_normalization_if_replacementString_is_empty(string repeatedStepReplacement)
        {
            var metadataProvider = new TestMetadataProvider(new DefaultNameFormatter(), new StepTypeConfiguration().UpdateRepeatedStepReplacement(repeatedStepReplacement));

            var stepTypeName = "given";

            var descriptor = new StepDescriptor(stepTypeName, "some_name", (o, a) => Task.CompletedTask);
            Assert.That(metadataProvider.GetStepName(descriptor, stepTypeName).StepTypeName, Is.EqualTo(stepTypeName.ToUpperInvariant()));
        }

        [Test]
        [TestCase("call_something", "CALL", "something")]
        [TestCase("CaLl_something", "CALL", "something")]
        [TestCase("invoke_something", "INVOKE", "something")]
        [TestCase("then_something", null, "then something")]
        public void Should_allow_to_reconfigure_predefined_step_types(string formattedName, string expectedType, string expectedNameFormat)
        {
            var metadataProvider = new TestMetadataProvider(new DefaultNameFormatter(), new StepTypeConfiguration().UpdatePredefinedStepTypes("call", "invoke"));

            var descriptor = new StepDescriptor(formattedName, (o, a) => Task.CompletedTask);
            var step = metadataProvider.GetStepName(descriptor, null);
            Assert.That(step.StepTypeName, Is.EqualTo(expectedType), "type");
            Assert.That(step.NameFormat, Is.EqualTo(expectedNameFormat), "name");
        }
    }
}