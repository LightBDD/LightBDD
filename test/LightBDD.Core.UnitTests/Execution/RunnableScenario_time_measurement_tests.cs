using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_time_measurement_tests
    {
        private static readonly TimeSpan UtcNowClockPrecision = TimeSpan.FromMilliseconds(30);

        [Test]
        public async Task It_should_capture_execution_time_for_successful_scenario()
        {
            await AssertScenarioExecutionTime(r => r.Test().TestScenario(Step_one, Step_two));
        }

        [Test]
        public async Task It_should_capture_execution_time_for_failed_scenario()
        {
            await AssertScenarioExecutionTime(r => r.Test().TestScenario(Step_one, Step_throwing_exception, Step_two));
        }

        [Test]
        public async Task It_should_capture_execution_time_for_composite_steps()
        {
            await AssertScenarioExecutionTime(r => r.Test().TestGroupScenario(Step_group_one, Step_group_two));
        }

        private async Task AssertScenarioExecutionTime(Func<ICoreScenarioStepsRunner, Task> runScenario)
        {
            var timer = DefaultExecutionTimer.StartNew();
            var start = timer.GetTime();
            var scenario = await TestableScenarioFactory.Default.RunScenario(runScenario);

            var stop = timer.GetTime();
            var executionTime = stop.GetExecutionTime(start);

            FormatTime("Measure time", executionTime);
            FormatTime("Scenario time", scenario.ExecutionTime);

            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.ExecutionTime.Duration, Is.LessThanOrEqualTo(executionTime.Duration), "Scenario.ExecutionTime.Duration");
            Assert.That(scenario.ExecutionTime.Start, Is
                .GreaterThanOrEqualTo(start.Time)
                .And
                .LessThan(executionTime.End.Add(UtcNowClockPrecision)), "Scenario.ExecutionTime.Start");

            AssertStepsExecutionTimesAreDoneInOrder(scenario);
        }

        private static void FormatTime(string label, ExecutionTime time)
        {
            if (time == null)
                return;
            TestContext.Out.WriteLine("{0}: {1:HH\\:mm\\:ss.fff} + {2} -> {3:HH\\:mm\\:ss.fff}", label, time.Start, time.Duration, time.End);
        }

        private void AssertStepsExecutionTimesAreDoneInOrder(IScenarioResult scenario)
        {
            var steps = scenario.GetSteps().ToArray();
            AssertStepTimes(steps, scenario.ExecutionTime);
        }

        private static void AssertStepTimes(IStepResult[] steps, ExecutionTime parentExecutionTime)
        {
            for (var i = 0; i < steps.Length; ++i)
            {
                var step = steps[i];

                FormatTime($"Step result [{step.Info}]", step.ExecutionTime);
                if (step.Status == ExecutionStatus.NotRun)
                {
                    Assert.That(step.ExecutionTime, Is.Null, $"Step[{step.Info}].ExecutionTime");
                    continue;
                }

                Assert.That(step.ExecutionTime, Is.Not.Null, $"Step[{step.Info}].ExecutionTime");

                if (i == 0)
                {
                    Assert.That(step.ExecutionTime.Start,
                        Is.GreaterThanOrEqualTo(parentExecutionTime.Start),
                        $"Step[{step.Info}].ExecutionTime.Start");
                }
                else
                {
                    Assert.That(step.ExecutionTime.Start,
                        Is.GreaterThanOrEqualTo(steps[i - 1].ExecutionTime.End - UtcNowClockPrecision),
                        $"Step[{step.Info}].ExecutionTime.Start");
                }

                if (i == steps.Length - 1)
                {
                    Assert.That(step.ExecutionTime.End,
                        Is.LessThanOrEqualTo(parentExecutionTime.End + UtcNowClockPrecision),
                        $"Step[{step.Info}].ExecutionTime.End");
                }

                AssertStepTimes(step.GetSubSteps().ToArray(), step.ExecutionTime);
            }
        }

        private static void Step_two()
        {
            Thread.Sleep(50);
        }

        private static void Step_one()
        {
            Thread.Sleep(50);
        }

        private static void Step_throwing_exception()
        {
            throw new NotImplementedException();
        }

        private TestCompositeStep Step_group_two()
        {
            return TestCompositeStep.Create(Step_one, Step_two, Step_two);
        }

        private TestCompositeStep Step_group_one()
        {
            return TestCompositeStep.Create(Step_one, Step_two);
        }
    }
}