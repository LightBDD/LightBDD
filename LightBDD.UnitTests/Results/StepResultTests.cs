using LightBDD.Results;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results
{
    [TestFixture]
    public class StepResultTests
    {
        [Test]
        public void ToString_should_return_formatted_step_result()
        {
            var result1 = new StepResult(1, new StepName("abc"), ResultStatus.Passed, "reason");
            var result2 = new StepResult(1, new StepName("abc"), ResultStatus.Passed);

            Assert.That(result1.ToString(), Is.EqualTo("1 abc: Passed (reason)"));
            Assert.That(result2.ToString(), Is.EqualTo("1 abc: Passed"));
        }
    }
}
