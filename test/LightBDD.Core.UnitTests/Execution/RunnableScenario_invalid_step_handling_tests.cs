using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

#pragma warning disable 1998

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_invalid_step_handling_tests
    {
        [Test]
        public async Task Invalid_step_descriptors_should_make_scenario_failing_immediately_without_execution()
        {
            var step1 = TestStep.Create(Step_that_should_not_run);
            var step2 = StepDescriptor.CreateInvalid(new Exception("reason1"));
            var step3 = StepDescriptor.CreateInvalid(new Exception("reason2"));
            var scenario = await TestableScenarioFactory.Default
                .CreateScenario(r => r.Test().TestScenario(step1, step2, step3))
                .RunAsync();

            //TODO: rework exception collection and handling
            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();
            Assert.That(ex.InnerExceptions.Select(x => x.Message).ToArray(),
                Is.EqualTo(new[] { "Step group initialization failed.", "reason1", "reason2" }));

            StepResultExpectation.AssertEqual(scenario.GetSteps(),
                new StepResultExpectation(1, 3, nameof(Step_that_should_not_run), ExecutionStatus.NotRun),
                new StepResultExpectation(2, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 2 Failed: System.Exception: reason1"),
                new StepResultExpectation(3, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 3 Failed: System.Exception: reason2"));
        }

        [Test]
        public async Task Invalid_step_descriptors_should_make_composite_failing_immediately_without_execution()
        {
            TestCompositeStep Composite_step() => new(
                TestStep.Create(Step_that_should_not_run),
                StepDescriptor.CreateInvalid(new Exception("reason1")),
                StepDescriptor.CreateInvalid(new Exception("reason2")));

            var scenario = await TestableScenarioFactory.Default
                .CreateScenario(r => r.Test()
                    .TestScenario(
                        TestStep.CreateComposite(Composite_step),
                        TestStep.Create(Step_that_should_not_run)))
                .RunAsync();

            //TODO: rework exception collection and handling
            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();
            Assert.That(ex.InnerExceptions.Select(x => x.Message).ToArray(),
                Is.EqualTo(new[] { "Step group initialization failed.", "reason1", "reason2" }));

            var mainSteps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(mainSteps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Step_that_should_not_run), ExecutionStatus.NotRun),
                new StepResultExpectation("1.", 2, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 1.2 Failed: System.Exception: reason1"),
                new StepResultExpectation("1.", 3, 3, "<INVALID STEP>", ExecutionStatus.Failed, "Step 1.3 Failed: System.Exception: reason2"));
            Assert.That(mainSteps[1].Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        private static async Task Step_that_should_not_run()
        {
            throw new InvalidOperationException("Step should not run");
        }
    }
}