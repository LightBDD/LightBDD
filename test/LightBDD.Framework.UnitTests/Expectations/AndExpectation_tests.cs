using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class AndExpectation_tests
    {
        [Test]
        public void It_should_pass_validation()
        {
            var expectation = Expect.To.GreaterThan(3).And(x => x.LessThan(5));
            Assert.True(expectation.Verify(4, ValueFormattingServices.Current));
        }

        [Test]
        public void It_should_pass_complex_validation()
        {
            var expectation = Expect.To.Not.Empty().And(x => x.Not.Null()).And(x => x.Contains('c'));
            Assert.True(expectation.Verify("abc", ValueFormattingServices.Current));
        }

        [Test]
        public void It_should_pass_negated_validation()
        {
            var expectation = Expect.To.Not.LessThan(3).And(x => x.Not.GreaterThan(5));
            Assert.True(expectation.Verify(4, ValueFormattingServices.Current));
        }

        [Test]
        [TestCase(3, "expected: (greater than '3' and less than '5'), but got: '3'\n\texpected: greater than '3', but got: '3'")]
        [TestCase(6, "expected: (greater than '3' and less than '5'), but got: '6'\n\texpected: less than '5', but got: '6'")]
        public void It_should_fail_validation(int value, string expectedMessage)
        {
            var expectation = Expect.To.GreaterThan(3).And(x => x.LessThan(5));
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")));
        }

        [Test]
        [TestCase(2, "expected: (not less than '3' and not greater than '5'), but got: '2'\n\texpected: not less than '3', but it was")]
        [TestCase(6, "expected: (not less than '3' and not greater than '5'), but got: '6'\n\texpected: not greater than '5', but it was")]
        public void It_should_fail_negated_validation(int value, string expectedMessage)
        {
            var expectation = Expect.To.Not.LessThan(3).And(x => x.Not.GreaterThan(5));
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")));
        }

        [Test]
        public void It_should_format()
        {
            Assert.That(Expect.To.GreaterThan(3).And(x => x.Not.LessThan(5)).ToString(), Is.EqualTo("(greater than '3' and not less than '5')"));
        }
    }
}