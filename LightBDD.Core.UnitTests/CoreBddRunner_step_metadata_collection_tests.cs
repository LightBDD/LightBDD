using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
    public class CoreBddRunner_step_metadata_collection_tests : Steps
    {
        private readonly IBddRunner _runner;

        public CoreBddRunner_step_metadata_collection_tests()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Fact]
        public void It_should_capture_all_steps()
        {
            _runner.TestScenario(
                Given_step_one,
                When_step_two,
                Then_step_three);

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, "Given step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, "When step two", ExecutionStatus.Passed),
                    new StepResultExpectation(3, "Then step three", ExecutionStatus.Passed)
                );
        }

        [Fact]
        public void It_should_capture_failed_steps()
        {
            try
            {
                _runner.TestScenario(
                    Given_step_one,
                    When_step_two_throwing_exception,
                    Then_step_three);
            }
            catch { }

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, "Given step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, "When step two throwing exception", ExecutionStatus.Failed, ExceptionReason),
                    new StepResultExpectation(3, "Then step three", ExecutionStatus.NotRun)
                );
        }

        [Fact]
        public void It_should_capture_bypassed_steps()
        {
            _runner.TestScenario(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, "Given step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, "When step two is bypassed", ExecutionStatus.Bypassed, BypassReason),
                    new StepResultExpectation(3, "Then step three", ExecutionStatus.Passed)
                );
        }
    }
}