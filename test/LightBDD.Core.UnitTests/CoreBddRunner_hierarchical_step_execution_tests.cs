using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_hierarchical_step_execution_tests : Steps
    {
        private IBddRunner _runner;
        private IFeatureRunner _feature;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public void Runner_should_execute_all_steps_within_group()
        {
            Assert.DoesNotThrow(() => _runner.Test().TestGroupScenario(Passing_step_group));
            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Passing step group", ExecutionStatus.Passed));

            StepResultExpectation.AssertEqual(steps[0].SubSteps,
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
        }

        [Test]
        public void Runner_should_mark_step_failed_if_substep_fails()
        {
            Assert.Throws<InvalidOperationException>(() => _runner.Test().TestGroupScenario(Failing_step_group));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Failing step group", ExecutionStatus.Failed, ExceptionReason));

            StepResultExpectation.AssertEqual(steps[0].SubSteps,
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two throwing exception", ExecutionStatus.Failed, ExceptionReason),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.NotRun)
            );
        }

        [Test]
        public void Runner_should_mark_step_ignored_if_substep_is_ignored()
        {
            Assert.Throws<CustomIgnoreException>(() => _runner.Test().TestGroupScenario(Ignored_step_group));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Ignored step group", ExecutionStatus.Ignored, IgnoreReason));

            StepResultExpectation.AssertEqual(steps[0].SubSteps,
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two ignoring scenario", ExecutionStatus.Ignored, IgnoreReason),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.NotRun)
            );
        }

        [Test]
        public void Runner_should_mark_step_bypassed_if_substep_is_bypassed()
        {
            Assert.DoesNotThrow(() => _runner.Test().TestGroupScenario(Bypassed_step_group));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();

            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Bypassed step group", ExecutionStatus.Bypassed, BypassReason));

            StepResultExpectation.AssertEqual(steps[0].SubSteps,
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two is bypassed", ExecutionStatus.Bypassed, BypassReason),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
        }

        [Test]
        public void Runner_should_properly_associate_steps_to_the_group()
        {
            Assert.DoesNotThrow(() => _runner.Test().TestGroupScenario(Passing_step_group, Composite_group));
            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 2, "Passing step group", ExecutionStatus.Passed),
                new StepResultExpectation(2, 2, "Composite group", ExecutionStatus.Bypassed, BypassReason));

            StepResultExpectation.AssertEqual(steps[0].SubSteps,
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[1].SubSteps.ElementAt(0).SubSteps,
                new StepResultExpectation("2.1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("2.1.", 2, 3, "WHEN step two", ExecutionStatus.Passed),
                new StepResultExpectation("2.1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
            StepResultExpectation.AssertEqual(steps[1].SubSteps.ElementAt(1).SubSteps,
                new StepResultExpectation("2.2.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("2.2.", 2, 3, "WHEN step two is bypassed", ExecutionStatus.Bypassed, BypassReason),
                new StepResultExpectation("2.2.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
        }

        StepGroup Composite_group()
        {
            return _runner.Test().CreateCompositeStepGroup(Passing_step_group, Bypassed_step_group);
        }

        StepGroup Passing_step_group()
        {
            return _runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two,
                Then_step_three);
        }

        StepGroup Failing_step_group()
        {
            return _runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_throwing_exception,
                Then_step_three);
        }

        StepGroup Ignored_step_group()
        {
            return _runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_ignoring_scenario,
                Then_step_three);
        }

        StepGroup Bypassed_step_group()
        {
            return _runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);
        }
    }
}