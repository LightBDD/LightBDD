using LightBDD.Core.Execution;
using LightBDD.Framework;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class StepExecution_tests
    {
        [Test]
        public void Bypass_should_throw_StepBypassException()
        {
            var bypassReason = "reason";

            var exception = Assert.Throws<BypassException>(() => StepExecution.Current.Bypass(bypassReason));
            Assert.That(exception.Message, Is.EqualTo(bypassReason));
        }
    }
}
