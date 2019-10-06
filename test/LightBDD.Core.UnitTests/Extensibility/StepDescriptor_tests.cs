using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class StepDescriptor_tests
    {
        private static readonly StepFunc SomeStepInvocation = (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance);

        [Test]
        public void It_should_allow_creating_step_descriptor()
        {
            var predefinedStepType = "stepType";
            var rawName = "rawName";
            var parameters = new[]
            {
                ParameterDescriptor.FromConstant(ParameterInfoHelper.IntParameterInfo, 55),
                ParameterDescriptor.FromConstant(ParameterInfoHelper.IntParameterInfo, 32)
            };

            var descriptor = new StepDescriptor(rawName, SomeStepInvocation, parameters) { PredefinedStepType = predefinedStepType };

            Assert.That(descriptor.PredefinedStepType, Is.EqualTo(predefinedStepType));
            Assert.That(descriptor.RawName, Is.EqualTo(rawName));
            Assert.That(descriptor.Parameters, Is.SameAs(parameters));
            Assert.That(descriptor.StepInvocation, Is.SameAs(SomeStepInvocation));
            Assert.That(descriptor.IsValid, Is.True);
            Assert.That(descriptor.CreationException, Is.Null);
        }

        [Test]
        public void It_should_allow_creating_step_descriptor_with_no_predefined_step()
        {
            var descriptor = new StepDescriptor("rawName", SomeStepInvocation);
            Assert.That(descriptor.PredefinedStepType, Is.Null);
        }

        [Test]
        [TestCase(null)]
        [TestCase(" \t\r\n")]
        public void It_should_require_meaningful_rawName(string incorrectName)
        {
            var ex = Assert.Throws<ArgumentException>(() => new StepDescriptor(incorrectName, SomeStepInvocation));
            Assert.That(ex.Message, Does.StartWith("Null or just white space is not allowed"));
            Assert.That(ex.ParamName, Is.EqualTo("rawName"));
        }

        [Test]
        public void It_should_require_not_null_stepInvocation()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StepDescriptor("abc", null));
            Assert.That(ex.ParamName, Is.EqualTo("stepInvocation"));
        }

        [Test]
        public void It_should_create_descriptor_from_exception()
        {
            var ex = new Exception("foo");
            var descriptor = StepDescriptor.CreateInvalid(ex);
            Assert.That(descriptor.IsValid, Is.False);
            Assert.That(descriptor.CreationException, Is.SameAs(ex));
            Assert.That(descriptor.RawName, Is.EqualTo("--INVALID STEP--"));
            Assert.That(descriptor.Parameters, Is.Empty);

            var actual = Assert.ThrowsAsync<Exception>(() => descriptor.StepInvocation(null, null));
            Assert.That(actual, Is.SameAs(ex));
        }
    }
}