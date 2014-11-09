using System;
using System.Diagnostics;
using LightBDD.Execution;
using LightBDD.Execution.Parameters;
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
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_one {0}", stepNumber, Map);

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
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam(argument) },
                "CALL", "Step_one {0}", stepNumber, Map);

            step.Invoke(_progressNotifier, 100);
            Assert.That(_context.StepOneValue, Is.EqualTo(argument));

            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_one abc"));
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
                (type, ctx, args) => ctx.Step_with_ignore_assertion((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_with_ignore_assertion {0}", stepNumber, Map);

            var ex = Assert.Throws<IgnoreException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_with_ignore_assertion abc"));
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
                (type, ctx, args) => ctx.Step_with_inconclusive_assertion((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_with_inconclusive_assertion {0}", stepNumber, Map);

            var ex = Assert.Throws<InconclusiveException>(() => step.Invoke(_progressNotifier, 100));
            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_with_inconclusive_assertion abc"));
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
                (type, ctx, args) => ctx.Step_with_exception((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_with_exception {0}", stepNumber, Map);

            var ex = Assert.Throws<InvalidOperationException>(() => step.Invoke(_progressNotifier, 100));

            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_with_exception abc"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Failed));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
            Assert.That(ex.Message, Is.EqualTo("abc"));
        }

        [Test]
        public void Should_return_not_run_result_with_unknown_unsafe_parameters()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam("abc", false) },
                "CALL", "Step_one {0}", stepNumber, Map);

            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_one <?>"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.NotRun));
            Assert.That(step.GetResult().StatusDetails, Is.Null);
        }

        [Test]
        public void Should_return_not_run_result_with_safe_parameters()
        {
            const int stepNumber = 1;

            var step = new ParameterizedStep<Context>(
                _context,
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_one {0}", stepNumber, Map);

            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_one abc"));
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
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam(ThrowException, false) },
                "CALL", "Step_one {0}", stepNumber, Map);

            var ex = Assert.Throws<InvalidOperationException>(() => step.Invoke(_progressNotifier, 100));

            Assert.That(step.GetResult().Name, Is.EqualTo("CALL Step_one <?>"));
            Assert.That(step.GetResult().Number, Is.EqualTo(stepNumber));
            Assert.That(step.GetResult().Status, Is.EqualTo(ResultStatus.Failed));
            Assert.That(step.GetResult().StatusDetails, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Should_measure_step_execution_time()
        {
            var step = new ParameterizedStep<Context>(
                _context,
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_one {0}", 1, Map);

            var watch = new Stopwatch();
            var startTime = DateTimeOffset.UtcNow;
            watch.Start();
            step.Invoke(_progressNotifier, 100);
            watch.Stop();
            Assert.That(step.GetResult().ExecutionTime, Is.LessThanOrEqualTo(watch.Elapsed));
            Assert.That(step.GetResult().ExecutionStart, Is.GreaterThanOrEqualTo(startTime).And.LessThanOrEqualTo(startTime.AddSeconds(1)));
        }

        [Test]
        public void Should_measure_step_execution_time_for_steps_throwing_exception()
        {
            var step = new ParameterizedStep<Context>(
                _context,
                (type, ctx, args) => ctx.Step_with_exception((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_with_exception {0}", 1, Map);

            var watch = new Stopwatch();
            var startTime = DateTimeOffset.UtcNow;
            watch.Start();
            try { step.Invoke(_progressNotifier, 100); }
            catch { }
            watch.Stop();
            Assert.That(step.GetResult().ExecutionTime, Is.LessThanOrEqualTo(watch.Elapsed));
            Assert.That(step.GetResult().ExecutionStart, Is.GreaterThanOrEqualTo(startTime).And.LessThanOrEqualTo(startTime.AddSeconds(1)));
        }

        [Test]
        public void Should_notify_step_finish()
        {
            const int totalStepCount = 100;

            var step = new ParameterizedStep<Context>(
                _context,
                (type, ctx, args) => ctx.Step_one((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_one {0}", 1, Map);

            step.Invoke(_progressNotifier, totalStepCount);
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(step.GetResult(), totalStepCount));
        }

        [Test]
        public void Should_notify_step_finish_for_steps_throwing_exception()
        {
            const int totalStepCount = 100;

            var step = new ParameterizedStep<Context>(
                _context,
                (type, ctx, args) => ctx.Step_with_exception((string)args[0]),
                new[] { CreateParam("abc") },
                "CALL", "Step_with_exception {0}", 1, Map);

            try { step.Invoke(_progressNotifier, 100); }
            catch { }
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(step.GetResult(), totalStepCount));
        }

        private object ThrowException()
        {
            throw new InvalidOperationException();
        }

        private static IStepParameter<Context> CreateParam(object value, bool isSafelyEvaluable = true)
        {
            return CreateParam(() => value, isSafelyEvaluable);
        }

        private static IStepParameter<Context> CreateParam(Func<object> valueFn, bool isSafelyEvaluable = true)
        {
            return new TestableParameter(valueFn, isSafelyEvaluable);
        }

        class TestableParameter : IStepParameter<Context>
        {
            private readonly Func<object> _valueFn;
            private readonly bool _isSafelyEvaluable;

            public TestableParameter(Func<object> valueFn, bool isSafelyEvaluable = true)
            {
                _valueFn = valueFn;
                _isSafelyEvaluable = isSafelyEvaluable;
            }

            public object Evaluate(Context context)
            {
                return _valueFn();
            }

            public bool IsSafelyEvaluable()
            {
                return _isSafelyEvaluable;
            }
        }
    }
}