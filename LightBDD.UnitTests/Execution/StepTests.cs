using System;
using System.Diagnostics;
using LightBDD.Execution;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Execution
{
    [TestFixture]
    public class StepTests : SomeSteps
    {
        private IProgressNotifier _progressNotifier;

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
        }
        private ResultStatus Map(Type exception)
        {
            return exception == typeof(IgnoreException) || exception == typeof(InconclusiveException)
                ? ResultStatus.Ignored
                : ResultStatus.Failed;
        }

        [Test]
        public void Should_notify_step_start()
        {
            const string stepName = "step";
            const int stepNumber = 1;
            const int totalStepCount = 100;

            var step = new Step(Step_one, stepName, stepNumber, Map);
            step.Invoke(_progressNotifier, totalStepCount);
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart(stepName, stepNumber, totalStepCount));
        }

        [Test]
        public void Should_assert_ignore_mark_step_ignored()
        {
            var step = new Step(Step_with_ignore_assertion, "step", 1, Map);
            var ex = Assert.Throws<IgnoreException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Should_assert_inconclusive_mark_step_ignored()
        {
            var step = new Step(Step_with_inconclusive_assertion, "step", 1, Map);
            var ex = Assert.Throws<InconclusiveException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Should_exception_mark_step_failed()
        {
            var step = new Step(Step_throwing_exception, "step", 1, Map);
            var ex = Assert.Throws<InvalidOperationException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Failed));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Should_use_given_step_name()
        {
            var step = new Step(Step_one, "step", 1, Map);
            Assert.That(step.GetResult().Name, Is.EqualTo("step"));
        }

        [Test]
        public void Should_mark_step_not_run()
        {
            var step = new Step(Step_one, "step", 1, Map);
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.NotRun));
            Assert.That(step.GetResult().StatusDetails, Is.Null);
        }

        [Test]
        public void Should_passing_action_mark_step_passed()
        {
            var step = new Step(Step_one, "step", 1, Map);
            step.Invoke(_progressNotifier, 100);
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(step.GetResult().StatusDetails, Is.Null);
        }

        [Test]
        public void Should_measure_step_execution_time()
        {
            var step = new Step(Step_one, "step", 1, Map);
            var watch = new Stopwatch();
            var startTime = DateTimeOffset.UtcNow;
            watch.Start();
            step.Invoke(_progressNotifier, 100);
            watch.Stop();
            Assert.That(step.GetResult().ExecutionTime, Is.LessThanOrEqualTo(watch.Elapsed));
            Assert.That(step.GetResult().ExecutionStart, Is.GreaterThanOrEqualTo(startTime).And.LessThanOrEqualTo(startTime.Add(watch.Elapsed)));
        }

        [Test]
        public void Should_measure_step_execution_time_for_failed_steps()
        {
            var step = new Step(Step_throwing_exception, "step", 1, Map);
            var watch = new Stopwatch();
            var startTime = DateTimeOffset.UtcNow;
            watch.Start();

            try { step.Invoke(_progressNotifier, 100); }
            catch { }

            watch.Stop();
            Assert.That(step.GetResult().ExecutionTime, Is.LessThanOrEqualTo(watch.Elapsed));
            Assert.That(step.GetResult().ExecutionStart, Is.GreaterThanOrEqualTo(startTime).And.LessThanOrEqualTo(startTime.Add(watch.Elapsed)));
        }

        [Test]
        public void Should_notify_step_finish()
        {
            const string stepName = "step";
            const int stepNumber = 1;
            const int totalStepCount = 100;

            var step = new Step(Step_one, stepName, stepNumber, Map);
            step.Invoke(_progressNotifier, totalStepCount);
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(step.GetResult(), totalStepCount));
        }

        [Test]
        public void Should_notify_step_finish_for_failed_steps()
        {
            const string stepName = "step";
            const int stepNumber = 1;
            const int totalStepCount = 100;

            var step = new Step(Step_throwing_exception, stepName, stepNumber, Map);
            try { step.Invoke(_progressNotifier, 100); }
            catch { }
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(step.GetResult(), totalStepCount));
        }
    }
}