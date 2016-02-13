using System;
using System.Collections.Generic;
using System.Globalization;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_step_execution_tests
    {
        private IBddRunner _runner;
        private List<string> _executedSteps;

        [SetUp]
        public void SetUp()
        {
            _runner = new TestableBddRunner(GetType());
            _executedSteps = new List<string>();
        }

        [Test]
        public void Runner_should_execute_all_steps()
        {
            _runner.TestScenario(Given_step_one,
                When_step_two,
                Then_step_three);

            Assert.That(_executedSteps, Is.EqualTo(new[] { "Given_step_one", "When_step_two", "Then_step_three" }));
        }

        [Test]
        public void Runner_should_propagate_step_exception_and_stop_executing_further_steps()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _runner.TestScenario(Given_step_one,
                 When_step_two_throwing_exception,
                 Then_step_three));

            Assert.That(ex.Message, Is.EqualTo("test"));

            Assert.That(_executedSteps, Is.EqualTo(new[] { "Given_step_one", "When_step_two_throwing_exception" }));
        }

        private void Given_step_one() { _executedSteps.Add("Given_step_one"); }
        private void When_step_two() { _executedSteps.Add("When_step_two"); }
        private void When_step_two_throwing_exception()
        {
            _executedSteps.Add("When_step_two_throwing_exception");
            throw new InvalidOperationException("test");
        }
        private void Then_step_three() { _executedSteps.Add("Then_step_three"); }
    }
}
