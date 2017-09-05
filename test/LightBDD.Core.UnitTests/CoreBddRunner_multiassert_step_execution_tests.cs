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
        [MultiAssert]
        public void Failing_multiassert_step() { throw new Exception("multiassert"); }
        public void Ignoring_step() { StepExecution.Current.IgnoreScenario("normal"); }
        [MultiAssert]
        public void Ignoring_multiassert_step() { StepExecution.Current.IgnoreScenario("multiassert"); }

        [MultiAssert]
        public TestCompositeStep Multiassert_ignoring_steps()
        {
            return TestCompositeStep.Create(Ignoring_multiassert_step, Ignoring_step, Ignoring_multiassert_step);
        }

        public TestCompositeStep Multiassert_failing_composite()
        {
            return TestCompositeStep.Create(Failing_multiassert_step, Failing_multiassert_step, Passing_step);
        }

        public TestCompositeStep Passing_composite()
        {
            return TestCompositeStep.Create(Passing_step, Passing_step);
        }

        public TestCompositeStep Multiassert_complex_composite()
        {
            return TestCompositeStep.CreateFromComposites(Multiassert_ignoring_steps, Multiassert_failing_composite, Passing_composite);
        }

        [Test]
        public void Runner_should_properly_capture_exceptions_from_multiassert_steps()
        {
            //TODO: make it more precise
            var ex = Assert.Throws<AggregateException>(() =>
                _runner.Test().TestGroupScenario(Multiassert_ignoring_steps, Multiassert_failing_composite, Passing_composite));

            Assert.That(ex.InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(Exception), typeof(Exception) }));
        }

        [Test]
        public void Runner_should_properly_capture_status_from_multiassert_steps()
        {
            Assert.Throws<AggregateException>(() => _runner.Test().TestGroupScenario(Multiassert_ignoring_steps, Multiassert_failing_composite, Passing_composite));

            var scenario = _feature.GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));

            var steps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Multiassert ignoring steps", ExecutionStatus.Ignored, "Step 1.1: multiassert\r\nStep 1.2: normal"),
                new StepResultExpectation(2, 3, "Multiassert failing composite", ExecutionStatus.Failed, "Step 2.1: multiassert\r\nStep 2.2: multiassert"),
                new StepResultExpectation(3, 3, "Passing composite", ExecutionStatus.NotRun)
                );

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, "Ignoring multiassert step", ExecutionStatus.Ignored, "Step 1.1: multiassert"),
                new StepResultExpectation("1.", 2, 3, "Ignoring step", ExecutionStatus.Ignored, "Step 1.2: normal"),
                new StepResultExpectation("1.", 3, 3, "Ignoring multiassert step", ExecutionStatus.NotRun)
            );

            StepResultExpectation.AssertEqual(steps[1].GetSubSteps(),
                new StepResultExpectation("2.", 1, 3, "Failing multiassert step", ExecutionStatus.Failed, "Step 2.1: multiassert"),
                new StepResultExpectation("2.", 2, 3, "Failing multiassert step", ExecutionStatus.Failed, "Step 2.2: multiassert"),
                new StepResultExpectation("2.", 3, 3, "Passing step", ExecutionStatus.Passed)
            );

            Assert.That(steps[2].GetSubSteps(), Is.Empty);
        }
    }
}