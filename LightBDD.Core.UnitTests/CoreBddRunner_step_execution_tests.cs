using System;
using System.Collections.Generic;
using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
    public class CoreBddRunner_step_execution_tests
    {
        private readonly IBddRunner _runner;
        private readonly List<string> _executedSteps = new List<string>();

        public CoreBddRunner_step_execution_tests()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Fact]
        public void Runner_should_execute_all_steps()
        {
            _runner.TestScenario(Given_step_one,
                When_step_two,
                Then_step_three);

            Assert.Equal(new[] { "Given_step_one", "When_step_two", "Then_step_three" }, _executedSteps);
        }

        [Fact]
        public void Runner_should_propagate_step_exception_and_stop_executing_further_steps()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _runner.TestScenario(Given_step_one,
                 When_step_two_throwing_exception,
                 Then_step_three));

            Assert.Equal("test", ex.Message);

            Assert.Equal(new[] { "Given_step_one", "When_step_two_throwing_exception" }, _executedSteps);
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
