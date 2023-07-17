using System;
using System.Linq;
using System.Threading.Tasks;
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
    //migrated
    public class CoreBddRunner_step_metadata_collection_tests : Steps
    {
        IScenarioResult ExecuteScenario(Action<ICoreScenarioBuilder> onRun) => TestableExecutionPipeline.Default.ExecuteScenario(this, onRun);

        [Test]
        public void It_should_capture_all_steps()
        {
            var steps = ExecuteScenario(x => x.Test().TestScenario(
                Given_step_one,
                When_step_two,
                Then_step_three))
                .GetSteps();

            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 3, "WHEN step two", ExecutionStatus.Passed),
                    new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_capture_failed_steps()
        {
            var steps = ExecuteScenario(x => x.Test().TestScenario(
                Given_step_one,
                When_step_two_throwing_exception,
                Then_step_three))
                .GetSteps();

            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 3, "WHEN step two throwing exception", ExecutionStatus.Failed, $"Step 2: {ExceptionReason}"),
                    new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.NotRun)
                );
        }

        [Test]
        public void It_should_capture_bypassed_steps()
        {
            var steps = ExecuteScenario(x => x.Test().TestScenario(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three)).GetSteps();

            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 3, "WHEN step two is bypassed", ExecutionStatus.Bypassed, $"Step 2: {BypassReason}"),
                    new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_infer_and_capture_default_step_types()
        {
            var steps = ExecuteScenario(x => x.Test().TestScenario(
                Setup_before_steps,
                Setup_before_steps,
                Given_step_one,
                Given_step_one,
                When_step_two,
                When_step_two,
                Then_step_three,
                Then_step_four,
                Then_step_five,
                Some_step
                )).GetSteps();

            StepResultExpectation.AssertEqual(steps,
                    new StepResultExpectation(1, 10, "SETUP before steps", ExecutionStatus.Passed),
                    new StepResultExpectation(2, 10, "AND before steps", ExecutionStatus.Passed),
                    new StepResultExpectation(3, 10, "GIVEN step one", ExecutionStatus.Passed),
                    new StepResultExpectation(4, 10, "AND step one", ExecutionStatus.Passed),
                    new StepResultExpectation(5, 10, "WHEN step two", ExecutionStatus.Passed),
                    new StepResultExpectation(6, 10, "AND step two", ExecutionStatus.Passed),
                    new StepResultExpectation(7, 10, "THEN step three", ExecutionStatus.Passed),
                    new StepResultExpectation(8, 10, "AND step four", ExecutionStatus.Passed),
                    new StepResultExpectation(9, 10, "AND step five", ExecutionStatus.Passed),
                    new StepResultExpectation(10, 10, "Some step", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_capture_predefined_step_types()
        {
            var steps = ExecuteScenario(x => x.Test().TestScenario(
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
                )).GetSteps();

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