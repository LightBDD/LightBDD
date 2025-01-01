using System;
using LightBDD.Framework;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class StepExecution_tests
    {
        [Test]
        public void Bypass_should_fail_when_called_outside_of_step()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => StepExecution.Current.Bypass("reason"));
            Assert.That(exception!.Message, Does.StartWith("Current task is not executing any scenarios."));
        }

        [Test]
        public void IgnoreScenario_should_fail_when_called_outside_of_step()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => StepExecution.Current.IgnoreScenario("reason"));
            Assert.That(exception!.Message, Does.StartWith("Current task is not executing any scenarios."));
        }
    }
}
