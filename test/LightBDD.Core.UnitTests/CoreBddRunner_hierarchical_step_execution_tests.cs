using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_hierarchical_step_execution_tests : StepGroups
    {
        #region Setup/Teardown

        private IFeatureRunner _feature;
        private IBddRunner _runner;
        protected override IBddRunner Runner => _runner;

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
            Assert.DoesNotThrow(() => Runner.Test().TestGroupScenario(Passing_step_group));
            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Passing step group", ExecutionStatus.Passed));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
        }

        [Test]
        public void Runner_should_mark_step_failed_if_substep_fails()
        {
            Assert.Throws<InvalidOperationException>(() => Runner.Test().TestGroupScenario(Failing_step_group));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Failing step group", ExecutionStatus.Failed, $"Step 1.2: {ExceptionReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two throwing exception", ExecutionStatus.Failed, $"Step 1.2: {ExceptionReason}"),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.NotRun)
            );
        }

        [Test]
        public void Runner_should_mark_step_ignored_if_substep_is_ignored()
        {
            Assert.Throws<CustomIgnoreException>(() => Runner.Test().TestGroupScenario(Ignored_step_group));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Ignored step group", ExecutionStatus.Ignored, $"Step 1.2: {IgnoreReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two ignoring scenario", ExecutionStatus.Ignored, $"Step 1.2: {IgnoreReason}"),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.NotRun)
            );
        }

        [Test]
        public void Runner_should_mark_step_bypassed_if_substep_is_bypassed()
        {
            Assert.DoesNotThrow(() => Runner.Test().TestGroupScenario(Bypassed_step_group));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();

            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, "Bypassed step group", ExecutionStatus.Bypassed, $"Step 1.2: {BypassReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two is bypassed", ExecutionStatus.Bypassed, $"Step 1.2: {BypassReason}"),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
        }

        [Test]
        public void Runner_should_properly_associate_steps_to_the_group()
        {
            Assert.DoesNotThrow(() => Runner.Test().TestGroupScenario(Passing_step_group, Composite_group));
            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 2, "Passing step group", ExecutionStatus.Passed),
                new StepResultExpectation(2, 2, "Composite group", ExecutionStatus.Bypassed, $"Step 2.2.2: {BypassReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, "WHEN step two", ExecutionStatus.Passed),
                new StepResultExpectation("1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[1].GetSubSteps().ElementAt(0).GetSubSteps(),
                new StepResultExpectation("2.1.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("2.1.", 2, 3, "WHEN step two with comment", ExecutionStatus.Passed, null, CommentReason),
                new StepResultExpectation("2.1.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
            StepResultExpectation.AssertEqual(steps[1].GetSubSteps().ElementAt(1).GetSubSteps(),
                new StepResultExpectation("2.2.", 1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation("2.2.", 2, 3, "WHEN step two is bypassed", ExecutionStatus.Bypassed, $"Step 2.2.2: {BypassReason}"),
                new StepResultExpectation("2.2.", 3, 3, "THEN step three", ExecutionStatus.Passed)
            );
        }
    }
}