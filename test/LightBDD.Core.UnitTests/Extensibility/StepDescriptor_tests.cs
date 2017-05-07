using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class StepDescriptor_tests
    {
        private static readonly Func<object, object[], Task<StepResultDescriptor>> SomeStepInvocation = (o, a) => Task.FromResult(StepResultDescriptor.None);

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

            var descriptor = new StepDescriptor(predefinedStepType, rawName, SomeStepInvocation, parameters);

            Assert.That(descriptor.PredefinedStepType, Is.EqualTo(predefinedStepType));
            Assert.That(descriptor.RawName, Is.EqualTo(rawName));
            Assert.That(descriptor.Parameters, Is.SameAs(parameters));
            Assert.That(descriptor.StepInvocation, Is.SameAs(SomeStepInvocation));
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
    }
}