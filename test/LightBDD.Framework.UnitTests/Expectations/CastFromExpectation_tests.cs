using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class CastFromExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<object>(
                    "like 'abc'",
                    x => x.BeLike("abc").CastFrom(Expect.Type<object>()))
                .WithMatchingValues("abc")
                .WithNotMatchingValue(null, "expected: like 'abc', but got: '<null>'")
                .WithNotMatchingValue("Abc", "expected: like 'abc', but got: 'Abc'");

            yield return new ExpectationScenario<object>(
                    "equals '5'",
                    x => x.Equal(5).CastFrom(Expect.Type<object>()))
                .WithMatchingValues(5, 5L, (byte)5, 5f, 5d, 5m, (int?)5)
                .WithNotMatchingValue(4d, "expected: equals '5', but got: '4'");
        }

        [Test]
        public void Casting_from_wrong_type_should_fail()
        {
            var result = Expect.To.BeLike("abc").CastFrom(Expect.Type<object>()).Verify(5, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message, Is.EqualTo("value '5' of type 'Int32' cannot be cast to 'String'"));
        }

        [Test]
        public void Casting_from_null_to_value_type_should_fail()
        {
            var result = Expect.To.Equal(5).CastFrom(Expect.Type<object>()).Verify(null, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message, Is.EqualTo("value '<null>' of type '<null>' cannot be cast to 'Int32'"));
        }

        [Test]
        public void Casting_of_numerics_with_overflow_should_fail()
        {
            var result = Expect.To.Equal(5).CastFrom(Expect.Type<object>()).Verify(long.MaxValue, ValueFormattingServices.Current);
            Assert.False(result);
            result.Message.ShouldBe("value '9223372036854775807' of type 'Int64' cannot be cast to 'Int32': Value was either too large or too small for an Int32.");
        }

        [Test]
        public void Casting_of_numerics_with_precision_loss_should_fail()
        {
            var result = Expect.To.Equal(5).CastFrom(Expect.Type<object>()).Verify(5.14, ValueFormattingServices.Current);
            Assert.False(result);
            result.Message.ShouldBe("value '5.14' of type 'Double' cannot be cast to 'Int32' without precision loss");
        }
    }
}