using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_step_metadata_collection_tests : Steps
    {
        private IBddRunner _runner;
        private IFeatureBddRunner _feature;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableBddRunnerFactory.GetRunner(GetType());
            _runner = _feature.GetRunner(this);
        }

        [Test]
        public void It_should_capture_all_steps()
        {
            _runner.Test().TestScenario(
                Given_step_one,
                When_step_two,
                Then_step_three);

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 3, "WHEN step two", ExecutionStatus.Passed),
                    new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_capture_failed_steps()
        {
            try
            {
                _runner.Test().TestScenario(
                    Given_step_one,
                    When_step_two_throwing_exception,
                    Then_step_three);
            }
            catch { }

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 3, "WHEN step two throwing exception", ExecutionStatus.Failed, ExceptionReason),
                    new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.NotRun)
                );
        }

        [Test]
        public void It_should_capture_bypassed_steps()
        {
            _runner.Test().TestScenario(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 3, "WHEN step two is bypassed", ExecutionStatus.Bypassed, BypassReason),
                    new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_infer_and_capture_default_step_types()
        {
            _runner.Test().TestScenario(
                Setup_before_steps,
                Setup_before_steps,
                Given_step_one,
                Given_step_one,
                When_step_two,
                When_step_two,
                Then_step_three,
                Then_step_four,
                Some_step
                );

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 9, "SETUP before steps", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 9, "AND before steps", ExecutionStatus.Passed),
                    new StepResultExpectation(3, 9, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(4, 9, "AND step one", ExecutionStatus.Passed),
                    new StepResultExpectation(5, 9, "WHEN step two", ExecutionStatus.Passed),
                    new StepResultExpectation(6, 9, "AND step two", ExecutionStatus.Passed),
                    new StepResultExpectation(7, 9, "THEN step three", ExecutionStatus.Passed),
                    new StepResultExpectation(8, 9, "AND step four", ExecutionStatus.Passed),
                    new StepResultExpectation(9, 9, "Some step", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_capture_predefined_step_types()
        {
            _runner.Test().TestScenario(
                TestStep.CreateWithTypeAsync("setup", Some_step),
                TestStep.CreateWithTypeAsync("setup", Some_step),
                TestStep.CreateWithTypeAsync("given", Some_step),
                TestStep.CreateWithTypeAsync("given", Some_step),
                TestStep.CreateWithTypeAsync("when", Some_step),
                TestStep.CreateWithTypeAsync("when", Some_step),
                TestStep.CreateWithTypeAsync("then", Some_step),
                TestStep.CreateWithTypeAsync("then", Some_step),
                TestStep.CreateWithTypeAsync("something else", Some_step),
                TestStep.CreateWithTypeAsync("something else", Some_step)
                );

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 10, "SETUP Some step", ExecutionStatus.Passed),
                new StepResultExpectation(2, 10, "AND Some step", ExecutionStatus.Passed),
                new StepResultExpectation(3, 10, "GIVEN Some step", ExecutionStatus.Passed),
                new StepResultExpectation(4, 10, "AND Some step", ExecutionStatus.Passed),
                new StepResultExpectation(5, 10, "WHEN Some step", ExecutionStatus.Passed),
                new StepResultExpectation(6, 10, "AND Some step", ExecutionStatus.Passed),
                new StepResultExpectation(7, 10, "THEN Some step", ExecutionStatus.Passed),
                new StepResultExpectation(8, 10, "AND Some step", ExecutionStatus.Passed),
                new StepResultExpectation(9, 10, "SOMETHING ELSE Some step", ExecutionStatus.Passed),
                new StepResultExpectation(10, 10, "AND Some step", ExecutionStatus.Passed)
                );
        }
    }
}