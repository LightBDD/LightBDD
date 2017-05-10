using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    //TODO: add time measurement for step groups
    [TestFixture]
    public class CoreBddRunner_time_measurement_tests
    {
        private IBddRunner _runner;
        private static readonly TimeSpan UtcNowClockPrecision = TimeSpan.FromMilliseconds(30);
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
        public void It_should_capture_execution_time_for_successful_scenario()
        {
            AssertScenarioExecutionTime(() => _runner.Test().TestScenario(Step_one, Step_two));
        }

        [Test]
        public void It_should_capture_execution_time_for_failed_scenario()
        {
            AssertScenarioExecutionTime(() => _runner.Test().TestScenario(Step_one, Step_throwing_exception, Step_two));
        }

        private void AssertScenarioExecutionTime(Action runScenario)
        {
            var startTime = DateTimeOffset.UtcNow;
            var watch = Stopwatch.StartNew();
            try { runScenario(); }
            catch { }
            watch.Stop();

            var result = _feature.GetFeatureResult().GetScenarios().Single();

            FormatTime("Measure time", new ExecutionTime(startTime, watch.Elapsed));
            FormatTime("Scenario time", result.ExecutionTime);

            Assert.That(result.ExecutionTime, Is.Not.Null);
            Assert.That(result.ExecutionTime.Duration, Is.LessThanOrEqualTo(watch.Elapsed), "Scenario.ExecutionTime.Duration");
            Assert.That(result.ExecutionTime.Start, Is
                .GreaterThanOrEqualTo(startTime)
                .And
                .LessThan(startTime.Add(watch.Elapsed).Add(UtcNowClockPrecision)), "Scenario.ExecutionTime.Start");

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
            for (int i = 0; i < steps.Length; ++i)
            {
                FormatTime("Step result", steps[i].ExecutionTime);
                if (steps[i].Status == ExecutionStatus.NotRun)
                {
                    Assert.That(steps[i].ExecutionTime, Is.Null, $"Step[{i}].ExecutionTime");
                    continue;
                }

                Assert.That(steps[i].ExecutionTime, Is.Not.Null, $"Step[{i}].ExecutionTime");

                if (i == 0)
                    Assert.That(steps[i].ExecutionTime.Start, Is.GreaterThanOrEqualTo(scenario.ExecutionTime.Start), $"Step[{i}].ExecutionTime.Start");
                else
                    Assert.That(steps[i].ExecutionTime.Start, Is.GreaterThanOrEqualTo(steps[i - 1].ExecutionTime.End - UtcNowClockPrecision), $"Step[{i}].ExecutionTime.Start");

                if (i == steps.Length - 1)
                    Assert.That(steps[i].ExecutionTime.End, Is.LessThanOrEqualTo(scenario.ExecutionTime.End + UtcNowClockPrecision), $"Step[{i}].ExecutionTime.End");
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
    }
}