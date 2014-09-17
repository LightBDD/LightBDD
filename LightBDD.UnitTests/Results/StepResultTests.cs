using LightBDD.Results;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results
{
    [TestFixture]
    public class StepResultTests
    {
        [Test]
        public void Should_compare_step_results()
        {
            var result1 = new StepResult(1, "abc", ResultStatus.Passed, "reason");
            var result2 = new StepResult(1, "abc", ResultStatus.Passed, "reason");
            var result3 = new StepResult(1, "abcd", ResultStatus.Passed, "reason");
            var result4 = new StepResult(2, "abc", ResultStatus.Passed, "reason");
            var result5 = new StepResult(1, "abc", ResultStatus.Failed, "reason");
            var result6 = new StepResult(1, "abc", ResultStatus.Passed);

            AssertEqualityAndHash(result1, result2, true);
            AssertEqualityAndHash(result1, result3, false);
            AssertEqualityAndHash(result1, result4, false);
            AssertEqualityAndHash(result1, result5, false);
            AssertEqualityAndHash(result1, result6, false);
        }

        [Test]
        public void ToString_should_return_formatted_step_result()
        {
            var result1 = new StepResult(1, "abc", ResultStatus.Passed, "reason");
            var result2 = new StepResult(1, "abc", ResultStatus.Passed);

            Assert.That(result1.ToString(), Is.EqualTo("1 abc: Passed (reason)"));
            Assert.That(result2.ToString(), Is.EqualTo("1 abc: Passed"));
        }

        private void AssertEqualityAndHash(StepResult result1, StepResult result2, bool shouldMatch)
        {
            Assert.That(Equals(result1, result2), Is.EqualTo(shouldMatch));
            Assert.That(result1.GetHashCode() == result2.GetHashCode(), Is.EqualTo(shouldMatch));
        }
    }
}
