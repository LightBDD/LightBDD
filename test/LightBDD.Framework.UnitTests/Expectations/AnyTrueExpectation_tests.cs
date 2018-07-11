using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class AnyTrueExpectation_tests
    {
        [Test]
        public void It_should_pass_validation()
        {
            var expectation = Expect.To.BeAnyTrue(x => x.BeLessThan(2), x => x.BeGreaterThan(5), x => x.Equal(4));
            Assert.True(expectation.Verify(1, ValueFormattingServices.Current));
            Assert.True(expectation.Verify(6, ValueFormattingServices.Current));
            Assert.True(expectation.Verify(4, ValueFormattingServices.Current));
        }

        [Test]
        public void It_should_pass_negated_validation()
        {
            var expectation = Expect.To.Not.BeAnyTrue(x => x.BeLessThan(2), x => x.BeGreaterThan(5), x => x.Equal(4));
            Assert.True(expectation.Verify(3, ValueFormattingServices.Current));
        }

        [Test]
        [TestCase(3, "expected: (less than '2' or greater than '5' or equals '4'), but got: '3'\n\texpected: less than '2', but got: '3'\n\texpected: greater than '5', but got: '3'\n\texpected: equals '4', but got: '3'")]
        public void It_should_fail_validation(int value, string expectedMessage)
        {
            var expectation = Expect.To.BeAnyTrue(x => x.BeLessThan(2), x => x.BeGreaterThan(5), x => x.Equal(4));
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")));
        }

        [Test]
        [TestCase(4, "expected: not (less than '2' or greater than '5' or equals '4'), but it was")]
        [TestCase(1, "expected: not (less than '2' or greater than '5' or equals '4'), but it was")]
        [TestCase(6, "expected: not (less than '2' or greater than '5' or equals '4'), but it was")]
        public void It_should_fail_negated_validation(int value, string expectedMessage)
        {
            var expectation = Expect.To.Not.BeAnyTrue(x => x.BeLessThan(2), x => x.BeGreaterThan(5), x => x.Equal(4));
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")));
        }

        [Test]
        public void It_should_format()
        {
            Assert.That(Expect.To.BeAnyTrue(x => x.BeLessThan(2), x => x.BeGreaterThan(5), x => x.Equal(4)).ToString(), Is.EqualTo("(less than '2' or greater than '5' or equals '4')"));
        }
    }
}