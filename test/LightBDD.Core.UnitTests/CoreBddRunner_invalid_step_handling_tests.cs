using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
#pragma warning disable 1998

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_invalid_step_handling_tests
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
        public void Invalid_step_descriptors_should_make_scenario_failing_immediately_without_execution()
        {
            var step1 = TestStep.Create(Step_that_should_not_run);
            var step2 = StepDescriptor.CreateInvalid(new Exception("reason1"));
            var step3 = StepDescriptor.CreateInvalid(new Exception("reason2"));
            var ex = Assert.Throws<AggregateException>(() => _runner.Test().TestScenario(step1, step2, step3));

            Assert.That(ex.InnerExceptions.Select(x => x.Message).ToArray(),
                Is.EqualTo(new[] { "Scenario steps initialization failed.", "reason1", "reason2" }));

            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Step that should not run", ExecutionStatus.NotRun),
                new StepResultExpectation(2, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 2: reason1"),
                new StepResultExpectation(3, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 3: reason2"));
        }

        [Test]
        public void Invalid_step_descriptors_should_make_composite_failing_immediately_without_execution()
        {
            TestCompositeStep Composite_step() => new(
                TestStep.Create(Step_that_should_not_run),
                StepDescriptor.CreateInvalid(new Exception("reason1")),
                StepDescriptor.CreateInvalid(new Exception("reason2")));

            var ex = Assert.Throws<AggregateException>(() => _runner.Test()
                .TestScenario(
                    TestStep.CreateComposite(Composite_step),
                    TestStep.Create(Step_that_should_not_run)));

            Assert.That(ex.InnerExceptions.Select(x => x.Message).ToArray(),
                Is.EqualTo(new[] { "Sub-steps initialization failed.", "reason1", "reason2" }));

            var mainSteps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            StepResultExpectation.AssertEqual(mainSteps[0].GetSubSteps(),
                new StepResultExpectation("1.",1, 3, "Step that should not run", ExecutionStatus.NotRun),
                new StepResultExpectation("1.",2, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 1.2: reason1"),
                new StepResultExpectation("1.",3, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 1.3: reason2"));
            Assert.That(mainSteps[1].Status,Is.EqualTo(ExecutionStatus.NotRun));
        }

        private static async Task Step_that_should_not_run()
        {
            throw new InvalidOperationException("Step should not run");
        }
    }
}