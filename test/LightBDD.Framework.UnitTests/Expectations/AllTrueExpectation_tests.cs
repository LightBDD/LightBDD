using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class AllTrueExpectation_tests
    {
        [Test]
        public void It_should_pass_validation()
        {
            var expectation = Expect.To.AllTrue(x => x.GreaterThan(2), x => x.LessThan(5), x => x.Not.Equal(3));
            Assert.True(expectation.Verify(4, ValueFormattingServices.Current));
        }

        [Test]
        public void It_should_pass_negated_validation()
        {
            var expectation = Expect.To.Not.AllTrue(x => x.GreaterThan(2), x => x.LessThan(5), x => x.Not.Equal(3));
            Assert.True(expectation.Verify(3, ValueFormattingServices.Current));
        }

        [Test]
        [TestCase(3, "expected: (greater than '2' and less than '5' and not equal '3'), but got: '3'\n\texpected: not equal '3', but it was")]
        [TestCase(6, "expected: (greater than '2' and less than '5' and not equal '3'), but got: '6'\n\texpected: less than '5', but got: '6'")]
        public void It_should_fail_validation(int value, string expectedMessage)
        {
            var expectation = Expect.To.AllTrue(x => x.GreaterThan(2), x => x.LessThan(5), x => x.Not.Equal(3));
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")));
        }

        [Test]
        [TestCase(4, "expected: not (greater than '2' and less than '5' and not equal '3'), but it was")]
        public void It_should_fail_negated_validation(int value, string expectedMessage)
        {
            var expectation = Expect.To.Not.AllTrue(x => x.GreaterThan(2), x => x.LessThan(5), x => x.Not.Equal(3));
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")));
        }

        [Test]
        public void It_should_format()
        {
            Assert.That(Expect.To.AllTrue(x => x.GreaterThan(2), x => x.LessThan(5), x => x.Not.Equal(3)).ToString(), Is.EqualTo("(greater than '2' and less than '5' and not equal '3')"));
        }
    }
}