using System;
using System.Globalization;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using LightBDD.Formatting.Parameters;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_step_type_interpretation_tests
    {
        private CoreMetadataProvider _metadataProvider;

        [SetUp]
        public void SetUp()
        {
            _metadataProvider = new TestMetadataProvider(new DefaultNameFormatter(), StepTypeConfiguration.Default);
        }

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
        public void GetStepName_should_properly_convert_predefined_step_type(string inputStepType, string lastStepType, string expectedStepType)
        {
            var descriptor = new StepDescriptor(inputStepType, "some_test_method", (o, a) => Task.CompletedTask);
            var stepName = _metadataProvider.GetStepName(descriptor, lastStepType);
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
            var descriptor = new StepDescriptor(methodName, (o, a) => Task.CompletedTask);
            var stepName = _metadataProvider.GetStepName(descriptor, lastStepType);
            Assert.That(stepName.StepTypeName, Is.EqualTo(expectedStepType));
            Assert.That(stepName.NameFormat, Is.EqualTo(expectedStepNameFormat));
        }
    }
}