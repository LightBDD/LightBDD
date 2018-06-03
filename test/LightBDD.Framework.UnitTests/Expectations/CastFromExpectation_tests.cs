using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class CastFromExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<object>(
                    "matches 'abc'",
                    x => x.Match("abc").CastFrom(Expect.Type<object>()))
                .WithMatchingValues("abc")
                .WithNotMatchingValue(null, "expected: matches 'abc', but got: '<null>'")
                .WithNotMatchingValue("Abc", "expected: matches 'abc', but got: 'Abc'");
        }

        [Test]
        public void Casting_from_wrong_type_should_fail()
        {
            var result = Expect.To.Match("abc").CastFrom(Expect.Type<object>()).Verify(5, ValueFormattingServices.Current);
            Assert.False(result);
            Assert.That(result.Message, Is.EqualTo("value of type 'Int32' cannot be cast to 'String'"));
        }
    }
}