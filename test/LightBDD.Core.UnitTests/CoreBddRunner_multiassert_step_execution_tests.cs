using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_multiassert_step_execution_tests
    {
        #region Setup/Teardown

        private IFeatureRunner _feature;
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        public void Passing_step() { }
        public void Failing_step() { throw new InvalidOperationException("failing"); }
        public void Ignoring_step() { StepExecution.Current.IgnoreScenario("ignoring"); }

        [MultiAssert]
        public TestCompositeStep Multiassert_ignoring_steps()
        {
            return TestCompositeStep.Create(Ignoring_step, Ignoring_step, Passing_step);
        }
        [MultiAssert]
        public TestCompositeStep Multiassert_failing_composite()
        {
            return TestCompositeStep.Create(Failing_step, Failing_step, Passing_step);
        }

        public TestCompositeStep Passing_composite()
        {
            return TestCompositeStep.Create(Passing_step, Passing_step);
        }

        [Test]
        public void Runner_should_throw_only_first_ignore_exception_to_be_properly_handled_by_underlying_testing_framework()
        {
            Assert.Throws<CustomIgnoreException>(() => _runner.Test().TestGroupScenario(Multiassert_ignoring_steps));
        }

        [Test]
        [MultiAssert]
        public void Runner_should_propagate_exceptions_from_multiassert_scenario_with_AggregateException()
        {
            var ex = Assert.Throws<AggregateException>(() => _runner.Test().TestGroupScenario(Multiassert_failing_composite, Multiassert_failing_composite));

            Assert.That(ex.InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(AggregateException), typeof(AggregateException) }));
            var innerAggregates = ex.InnerExceptions.Cast<AggregateException>().ToArray();
            Assert.That(innerAggregates[0].InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(InvalidOperationException), typeof(InvalidOperationException) }));
            Assert.That(innerAggregates[1].InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(InvalidOperationException), typeof(InvalidOperationException) }));
        }

        [Test]
        public void Runner_should_throw_original_exception_if_there_is_no_need_to_aggregate_multiple_ones()
        {
            var ex = Assert.Throws<AggregateException>(() => _runner.Test().TestGroupScenario(Multiassert_failing_composite));

            Assert.That(ex.InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(InvalidOperationException), typeof(InvalidOperationException) }));
        }

        [Test]
        [MultiAssert]
        public void Runner_should_aggregate_only_failure_exceptions()
        {
            var ex = Assert.Throws<AggregateException>(() => _runner.Test().TestGroupScenario(Multiassert_failing_composite, Multiassert_ignoring_steps, Multiassert_failing_composite));
            Assert.That(
                ex.Flatten().InnerExceptions.Select(x => x.GetType()).ToArray(),
                Is.EqualTo(new[]
                {
                    typeof(InvalidOperationException), typeof(InvalidOperationException),
                    typeof(InvalidOperationException), typeof(InvalidOperationException)
                }));
        }

        [Test]
        [MultiAssert]
        public void Runner_should_properly_capture_status_from_multiassert_steps()
        {
            Assert.Throws<AggregateException>(() => _runner.Test().TestGroupScenario(Multiassert_ignoring_steps, Multiassert_failing_composite, Passing_composite));

            var scenario = _feature.GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));

            var steps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Multiassert ignoring steps", ExecutionStatus.Ignored, $"Step 1.1: ignoring{Environment.NewLine}Step 1.2: ignoring"),
                new StepResultExpectation(2, 3, "Multiassert failing composite", ExecutionStatus.Failed, $"Step 2.1: failing{Environment.NewLine}Step 2.2: failing"),
                new StepResultExpectation(3, 3, "Passing composite", ExecutionStatus.Passed)
                );

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "Ignoring step", ExecutionStatus.Ignored, "Step 1.1: ignoring"),
                new StepResultExpectation("1.", 2, 3, "Ignoring step", ExecutionStatus.Ignored, "Step 1.2: ignoring"),
                new StepResultExpectation("1.", 3, 3, "Passing step", ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[1].GetSubSteps(),
                new StepResultExpectation("2.", 1, 3, "Failing step", ExecutionStatus.Failed, "Step 2.1: failing"),
                new StepResultExpectation("2.", 2, 3, "Failing step", ExecutionStatus.Failed, "Step 2.2: failing"),
                new StepResultExpectation("2.", 3, 3, "Passing step", ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[2].GetSubSteps(),
                new StepResultExpectation("3.", 1, 2, "Passing step", ExecutionStatus.Passed),
                new StepResultExpectation("3.", 2, 2, "Passing step", ExecutionStatus.Passed)
            );
        }
    }
}