using System;
using LightBDD.Execution;
using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Execution
{
    [TestFixture]
    public class ParameterizedStepTests
    {
        private IProgressNotifier _progressNotifier;
        private Context _context;

        class Context
        {
            public void Step_one(string one)
            {
                StepOneValue = one;
            }

            public string StepOneValue { get; private set; }

            public void Step_with_ignore_assertion(string value)
            {
                Assert.Ignore("Just ignore");
            }

            public void Step_with_inconclusive_assertion(string s)
            {
                Assert.Inconclusive("Inconclusive");
            }

            public void Step_with_exception(string message)
            {
                throw new InvalidOperationException(message);
            }
        }

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _context = new Context();
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
            const string stepName = "Step_one abc";
            const int stepNumber = 1;
            const int totalStepCount = 100;

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_one((string)args[0]),
                new Func<Context, object>[] { ctx => "abc" },
                "Step_one {0}", stepNumber, Map);

            step.Invoke(_progressNotifier, totalStepCount);
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart(stepName, stepNumber, totalStepCount));
        }

        [Test]
        public void Should_call_step_with_arguments_and_mark_it_passed()
        {
            const int stepNumber = 1;
            const string argument = "abc";

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_one((string)args[0]),
                new Func<Context, object>[] { ctx => argument },
                "Step_one {0}", stepNumber, Map);

            step.Invoke(_progressNotifier, 100);
            Assert.That(_context.StepOneValue, Is.EqualTo(argument));

            Assert.That(step.GetResult().Name, Is.EqualTo("Step_one abc"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(step.GetResult().StatusDetails, Is.Null);
        }

        [Test]
        public void Should_assert_ignore_mark_step_ignored()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_with_ignore_assertion((string)args[0]),
                new Func<Context, object>[] { ctx => "abc" },
                "Step_with_ignore_assertion {0}", stepNumber, Map);

            var ex = Assert.Throws<IgnoreException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Name, Is.EqualTo("Step_with_ignore_assertion abc"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Should_assert_inconclusive_mark_step_ignored()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_with_inconclusive_assertion((string)args[0]),
                new Func<Context, object>[] { ctx => "abc" },
                "Step_with_inconclusive_assertion {0}", stepNumber, Map);

            var ex = Assert.Throws<InconclusiveException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Name, Is.EqualTo("Step_with_inconclusive_assertion abc"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Should_exception_mark_step_failed()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_with_exception((string)args[0]),
                new Func<Context, object>[] { ctx => "abc" },
                "Step_with_exception {0}", stepNumber, Map);

            var ex = Assert.Throws<InvalidOperationException>(() => step.Invoke(_progressNotifier, 100));

            Assert.That(step.GetResult().Name, Is.EqualTo("Step_with_exception abc"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
            Assert.That(ex.Message, Is.EqualTo("abc"));
        }

        [Test]
        public void Should_return_not_run_result_with_unknown_parameters()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_one((string)args[0]),
                new Func<Context, object>[] { ctx => "abc" },
                "Step_one {0}", stepNumber, Map);

            Assert.That(step.GetResult().Name, Is.EqualTo("Step_one <?>"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.NotRun));
            Assert.That(step.GetResult().StatusDetails, Is.Null);
        }

        [Test]
        public void Should_exception_mark_step_failed_with_unknown_parameters_if_parameter_evaluation_failed()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (ctx, args) => ctx.Step_one((string)args[0]),
                new Func<Context, object>[] { ctx => ThrowException() },
                "Step_one {0}", stepNumber, Map);

            var ex = Assert.Throws<InvalidOperationException>(() => step.Invoke(_progressNotifier, 100));

            Assert.That(step.GetResult().Name, Is.EqualTo("Step_one <?>"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Failed));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        private object ThrowException()
        {
            throw new InvalidOperationException();
        }
    }
}