using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Core.UnitTests.Formatting
{
    [TestFixture]
    public class StepNameDefaultDecorator_tests
    {
        [Test]
        [TestCase("aBcdef12", "aBcdef12")]
        [TestCase(null, "")]
        public void Should_format_step_type_name(string stepTypeName, string expected)
        {
            Assert.That(StepNameDecorators.Default.DecorateStepTypeName(stepTypeName), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("abc", true, "abc")]
        [TestCase("<?>", false, "<?>")]
        [TestCase(null, false, "")]
        public void Should_format_step_parameter(string value, bool isEvaluated, string expected)
        {
            var stepParameter = MockRepository.GenerateMock<INameParameterInfo>();
            stepParameter.Stub(p => p.FormattedValue).Return(value);
            stepParameter.Stub(p => p.IsEvaluated).Return(isEvaluated);
            Assert.That(StepNameDecorators.Default.DecorateParameterValue(stepParameter), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("aBcdef12", "aBcdef12")]
        [TestCase(null, "")]
        public void Should_format_step_name_format(string nameFormat, string expected)
        {
            Assert.That(StepNameDecorators.Default.DecorateNameFormat(nameFormat), Is.EqualTo(expected));
        }
    }
}