using System;
using System.Globalization;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.UnitTests.Formatting;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture]
    public class Verifiable_tests
    {
        [Test]
        public void It_should_initialize_instance_with_implicit_cast()
        {
            var value = Fake.Int();

            Verifiable<int> verifiable = value;
            Assert.That(verifiable.SetActual(value).Status, Is.EqualTo(ParameterVerificationStatus.Success));
            Assert.That(verifiable.Expectation.ToString(), Is.EqualTo($"equals '{value}'"));
        }

        [Test]
        public void It_should_initialize_instance_with_NotProvided_status()
        {
            Verifiable<int> expectation = Fake.Int();
            Assert.That(expectation.Status, Is.EqualTo(ParameterVerificationStatus.NotProvided));
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.GetActual());
            Assert.That(ex.Message, Is.EqualTo("Actual value is not set"));
        }

        [Test]
        public void It_should_initialize_instance_with_default_formatting()
        {
            var value = Fake.Int();
            Verifiable<int> expectation = value;

            Assert.That(expectation.ToString(), Is.EqualTo(string.Format(CultureInfo.InvariantCulture, "expected: equals '{0}'", value)));
        }

        [Test]
        public void SetActual_should_set_actual_value()
        {
            var actualValue = Fake.Int();
            Verifiable<int> expectation = Fake.Int();
            expectation.SetActual(actualValue);

            Assert.That(expectation.Status, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
            Assert.That(expectation.GetActual(), Is.EqualTo(actualValue));
        }

        [Test]
        [TestCase(55, 55, true, "55")]
        [TestCase(55, 32, false, "expected: equals '55', but got: '32'")]
        public void SetActual_should_verify_expectation_and_update_formatting(int expected, int actual, bool shouldMatch, string message)
        {
            Verifiable<int> expectation = expected;
            expectation.SetActual(() => actual);

            Assert.That(expectation.Status, Is.EqualTo(shouldMatch ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure));
            Assert.That(expectation.ToString(), Is.EqualTo(message));
        }

        [Test]
        [TestCase(55, 55, true, "55")]
        [TestCase(55, 32, false, "expected: equals '55', but got: '32'")]
        public async Task SetActualAsync_should_verify_expectation_and_update_formatting(int expected, int actual, bool shouldMatch, string message)
        {
            Verifiable<int> expectation = expected;
            await expectation.SetActualAsync(async () =>
            {
                await Task.Yield();
                return actual;
            });

            Assert.That(expectation.Status, Is.EqualTo(shouldMatch ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure));
            Assert.That(expectation.ToString(), Is.EqualTo(message));
        }

        [Test]
        public void SetActual_should_capture_exception_during_actual_value_evaluation_and_expose_it_with_GetActual()
        {
            var expected = Fake.Int();

            Verifiable<int> expectation = expected;
            expectation.SetActual(() => throw new DivideByZeroException());

            Assert.That(expectation.Status, Is.EqualTo(ParameterVerificationStatus.Exception));
            Assert.That(expectation.ToString(), Is.EqualTo($"expected: equals '{expected}', but got: '<{nameof(DivideByZeroException)}>'"));
            Assert.Throws<DivideByZeroException>(() => expectation.GetActual());
        }

        [Test]
        public async Task SetActualAsync_should_capture_exception_during_actual_value_evaluation_and_expose_it_with_GetActual()
        {
            var expected = Fake.Int();

            Verifiable<int> expectation = expected;
            await expectation.SetActualAsync(() => throw new DivideByZeroException());

            Assert.That(expectation.Status, Is.EqualTo(ParameterVerificationStatus.Exception));
            Assert.That(expectation.ToString(), Is.EqualTo($"expected: equals '{expected}', but got: '<{nameof(DivideByZeroException)}>'"));
            Assert.Throws<DivideByZeroException>(() => expectation.GetActual());
        }

        [Test]
        public void SetActual_should_be_allowed_once()
        {
            Verifiable<int> expectation = Fake.Int();
            expectation.SetActual(() => Fake.Int());
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(() => Fake.Int()));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified or is being provided"));
            ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(Fake.Int()));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified or is being provided"));
        }

        [Test]
        public void SetActualAsync_should_be_allowed_once()
        {
            Verifiable<int> expectation = Fake.Int();
            expectation.SetActual(() => Fake.Int());
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => expectation.SetActualAsync(() => Task.FromResult(Fake.Int())));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified or is being provided"));
        }

        [Test]
        public void SetValueFormattingService_should_allow_using_custom_formatter()
        {
            var expected = Fake.Int();
            var actualValue = expected + 1;
            Verifiable<int> expectation = expected;
            ((IComplexParameter)expectation).SetValueFormattingService(new ValueFormattingServiceStub(CultureInfo.InvariantCulture, "--{0}--"));
            expectation.SetActual(() => actualValue);

            Assert.That(expectation.ToString(), Is.EqualTo($"expected: equals '--{expected}--', but got: '--{actualValue}--'"));
        }

        [Test]
        public void SetActual_traces_progress_when_InitializeParameterTrace_used()
        {
            var value = 5;
            Verifiable<int> expectation = value;
            var publisher = new CapturingProgressPublisher();
            ((ITraceableParameter)expectation).InitializeParameterTrace(TestResults.CreateParameterInfo("p"), publisher);

            expectation.SetActual(value);

            publisher.AssertLogs(
                "InlineParameterDiscovered|Param=p|Status=NotProvided|E=equals '5'|V=",
                "InlineParameterValidationStarting|Param=p|Status=NotProvided|E=equals '5'|V=",
                "InlineParameterValidationFinished|Param=p|Status=Success|E=equals '5'|V=5");
        }

        [Test]
        public void SetActual_with_function_traces_progress_when_InitializeParameterTrace_used()
        {
            var value = 3;
            Verifiable<int> expectation = value;
            var publisher = new CapturingProgressPublisher();
            ((ITraceableParameter)expectation).InitializeParameterTrace(TestResults.CreateParameterInfo("c"), publisher);

            expectation.SetActual(() => value);

            publisher.AssertLogs(
                "InlineParameterDiscovered|Param=c|Status=NotProvided|E=equals '3'|V=",
                "InlineParameterValidationStarting|Param=c|Status=NotProvided|E=equals '3'|V=",
                "InlineParameterValidationFinished|Param=c|Status=Success|E=equals '3'|V=3");
        }

        [Test]
        public async Task SetActualAsync_traces_progress_when_InitializeParameterTrace_used()
        {
            var value = 8;
            Verifiable<int> expectation = value;
            var publisher = new CapturingProgressPublisher();
            ((ITraceableParameter)expectation).InitializeParameterTrace(TestResults.CreateParameterInfo("i"), publisher);

            await expectation.SetActualAsync(() => Task.FromResult(value));

            publisher.AssertLogs(
                "InlineParameterDiscovered|Param=i|Status=NotProvided|E=equals '8'|V=",
                "InlineParameterValidationStarting|Param=i|Status=NotProvided|E=equals '8'|V=",
                "InlineParameterValidationFinished|Param=i|Status=Success|E=equals '8'|V=8");
        }

        [Test]
        public async Task SetActualAsync_traces_progress_for_exceptions()
        {
            var value = 8;
            Verifiable<int> expectation = value;
            var publisher = new CapturingProgressPublisher();
            ((ITraceableParameter)expectation).InitializeParameterTrace(TestResults.CreateParameterInfo("i"), publisher);

            await expectation.SetActualAsync(async () => throw new InvalidOperationException("test"));

            publisher.AssertLogs(
                "InlineParameterDiscovered|Param=i|Status=NotProvided|E=equals '8'|V=",
                "InlineParameterValidationStarting|Param=i|Status=NotProvided|E=equals '8'|V=",
                "InlineParameterValidationFinished|Param=i|Status=Exception|E=equals '8'|V=<InvalidOperationException>");
        }

        [Test]
        public void InitializeParameterTrace_should_publish_discovery_event()
        {
            Verifiable<int> expectation = 5;
            var publisher = new CapturingProgressPublisher();
            ((ITraceableParameter)expectation).InitializeParameterTrace(TestResults.CreateParameterInfo("p"), publisher);
            publisher.AssertLogs("InlineParameterDiscovered|Param=p|Status=NotProvided|E=equals '5'|V=");
        }
    }
}
