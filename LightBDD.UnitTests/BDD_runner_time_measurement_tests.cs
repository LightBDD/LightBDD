using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    public class BDD_runner_time_measurement_tests
    {
        private AbstractBDDRunner _subject;
        private static readonly TimeSpan _utcNowClockPrecision = TimeSpan.FromMilliseconds(15);

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
        }

        #endregion

        [Test]
        public void Should_capture_execution_time_for_simplified_steps()
        {
            AssertScenarioExecutionTime(() => _subject.RunScenario(Step_one, Step_two));
        }

        [Test]
        public void Should_capture_execution_time_for_failed_scenario_with_simplified_steps()
        {
            AssertScenarioExecutionTime(() => _subject.RunScenario(Step_one, Step_throwing_exception, Step_two));
        }

        [Test]
        public void Should_capture_execution_time_for_parameterized_steps()
        {
            AssertScenarioExecutionTime(() => _subject.RunScenario(
                when => Step_one(),
                then => Step_two()));
        }

        [Test]
        public void Should_capture_execution_time_for_failed_scenario_with_parameterized_steps()
        {
            AssertScenarioExecutionTime(() => _subject.RunScenario(
                given => Step_one(),
                when => Step_throwing_exception(),
                then => Step_two()));
        }

        private void AssertScenarioExecutionTime(Action runScenario)
        {
            var watch = new Stopwatch();

            var startTime = DateTimeOffset.UtcNow;
            watch.Start();
            try { runScenario(); }
            catch { }
            watch.Stop();

            var result = _subject.Result.Scenarios.Single();

            FormatTime("Measure time", startTime, watch.Elapsed);
            FormatTime("Scenario time", result.ExecutionStart, result.ExecutionTime);

            Assert.That(result.ExecutionTime, Is.LessThanOrEqualTo(watch.Elapsed));
            Assert.That(result.ExecutionStart, Is
                .GreaterThanOrEqualTo(startTime)
                .And
                .LessThan(startTime.Add(watch.Elapsed).Add(_utcNowClockPrecision)));

            AssertStepsExecutionTimesAreDoneInOrder();
        }

        private static void FormatTime(string label, DateTimeOffset? start, TimeSpan? elapsed)
        {
            Console.WriteLine("{0}: {1:HH\\:mm\\:ss.fff} + {2} -> {3:HH\\:mm\\:ss.fff}", label, start, elapsed,start+elapsed);
        }

        private void AssertStepsExecutionTimesAreDoneInOrder()
        {
            var scenario = _subject.Result.Scenarios.Single();
            var steps = scenario.Steps.ToArray();
            for (int i = 0; i < steps.Length; ++i)
            {
                FormatTime("Step result", steps[i].ExecutionStart, steps[i].ExecutionTime);
                if (steps[i].Status == ResultStatus.NotRun)
                {
                    Assert.That(steps[i].ExecutionStart, Is.Null);
                    Assert.That(steps[i].ExecutionTime, Is.Null);
                    continue;
                }

                if (i == 0)
                    Assert.That(steps[i].ExecutionStart, Is.GreaterThanOrEqualTo(scenario.ExecutionStart));
                else
                    Assert.That(steps[i].ExecutionStart, Is.GreaterThanOrEqualTo(steps[i - 1].ExecutionStart + steps[i - 1].ExecutionTime - _utcNowClockPrecision));

                if (i == steps.Length - 1)
                    Assert.That(steps[i].ExecutionStart + steps[i].ExecutionTime, Is.LessThanOrEqualTo(scenario.ExecutionStart + scenario.ExecutionTime + _utcNowClockPrecision));
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