using System;
using System.Linq;
using System.Threading;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_time_measurement_tests
    {
        private IBddRunner _runner;
        private static readonly TimeSpan UtcNowClockPrecision = TimeSpan.FromMilliseconds(30);
        private IFeatureRunner _feature;
        private IExecutionTimer _timer;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var repo = new TestableFeatureRunnerRepository();
            _timer = repo.Context.ExecutionTimer;
            _feature = repo.GetRunnerFor(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public void It_should_capture_execution_time_for_successful_scenario()
        {
            AssertScenarioExecutionTime(() => _runner.Test().TestScenario(Step_one, Step_two));
        }

        [Test]
        public void It_should_capture_execution_time_for_failed_scenario()
        {
            AssertScenarioExecutionTime(() => _runner.Test().TestScenario(Step_one, Step_throwing_exception, Step_two));
        }

        [Test]
        public void It_should_capture_execution_time_for_composite_steps()
        {
            AssertScenarioExecutionTime(() => _runner.Test().TestGroupScenario(Step_group_one, Step_group_two));
        }

        private void AssertScenarioExecutionTime(Action runScenario)
        {
            var start = _timer.GetTime();
            try { runScenario(); }
            catch { }

            var stop = _timer.GetTime();
            var executionTime = stop.GetExecutionTime(start);

            var result = _feature.GetFeatureResult().GetScenarios().Single();

            FormatTime("Measure time", executionTime);
            FormatTime("Scenario time", result.ExecutionTime);

            Assert.That(result.ExecutionTime, Is.Not.Null);
            Assert.That(result.ExecutionTime.Duration, Is.LessThanOrEqualTo(executionTime.Duration), "Scenario.ExecutionTime.Duration");
            Assert.That(result.ExecutionTime.Start, Is
                .GreaterThanOrEqualTo(start.Time)
                .And
                .LessThan(executionTime.End.Add(UtcNowClockPrecision)), "Scenario.ExecutionTime.Start");

            AssertStepsExecutionTimesAreDoneInOrder();
        }

        private static void FormatTime(string label, ExecutionTime time)
        {
            if (time == null)
                return;
            TestContext.Out.WriteLine("{0}: {1:HH\\:mm\\:ss.fff} + {2} -> {3:HH\\:mm\\:ss.fff}", label, time.Start, time.Duration, time.End);
        }

        private void AssertStepsExecutionTimesAreDoneInOrder()
        {
            var scenario = _feature.GetFeatureResult().GetScenarios().Single();
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