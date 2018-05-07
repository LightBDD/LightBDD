using System;
using System.Globalization;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.UnitTests.Formatting;
using NUnit.Framework;
using RandomTestValues;

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture]
    public class Expected_tests
    {
        [Test]
        public void It_should_initialize_instance_with_implicit_cast()
        {
            var value = RandomValue.Int();

            Expected<int> expected = value;
            Assert.That(expected.SetActual(value).Status, Is.EqualTo(ParameterVerificationStatus.Success));
            Assert.That(expected.Expectation.ToString(), Is.EqualTo($"equal '{value}'"));
        }

        [Test]
        public void It_should_initialize_instance_with_NotProvided_status()
        {
            Expected<int> expectation = RandomValue.Int();
            Assert.That(expectation.Status, Is.EqualTo(ParameterVerificationStatus.NotProvided));
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.GetActual());
            Assert.That(ex.Message, Is.EqualTo("Actual value is not set"));
        }

        [Test]
        public void It_should_initialize_instance_with_default_formatting()
        {
            var value = RandomValue.Int();
            Expected<int> expectation = value;

            Assert.That(expectation.ToString(), Is.EqualTo(string.Format(CultureInfo.InvariantCulture, "expected: equal '{0}'", value)));
        }

        [Test]
        public void SetActual_should_set_actual_value()
        {
            var actualValue = RandomValue.Int();
            Expected<int> expectation = RandomValue.Int();
            expectation.SetActual(actualValue);

            Assert.That(expectation.Status, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
            Assert.That(expectation.GetActual(), Is.EqualTo(actualValue));
        }

        [Test]
        [TestCase(55, 55, true, "55")]
        [TestCase(55, 32, false, "expected: equal '55', but got: '32'")]
        public void SetActual_should_verify_expectation_and_update_formatting(int expected, int actual, bool shouldMatch, string message)
        {
            Expected<int> expectation = expected;
            expectation.SetActual(() => actual);

            Assert.That(expectation.Status, Is.EqualTo(shouldMatch ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure));
            Assert.That(expectation.ToString(), Is.EqualTo(message));
        }

        [Test]
        [TestCase(55, 55, true, "55")]
        [TestCase(55, 32, false, "expected: equal '55', but got: '32'")]
        public async Task SetActualAsync_should_verify_expectation_and_update_formatting(int expected, int actual, bool shouldMatch, string message)
        {
            Expected<int> expectation = expected;
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
            var expected = RandomValue.Int();

            Expected<int> expectation = expected;
            expectation.SetActual(() => throw new DivideByZeroException());

            Assert.That(expectation.Status, Is.EqualTo(ParameterVerificationStatus.Exception));
            Assert.That(expectation.ToString(), Is.EqualTo($"expected: equal '{expected}', but got: '<{nameof(DivideByZeroException)}>'"));
            Assert.Throws<DivideByZeroException>(() => expectation.GetActual());
        }

        [Test]
        public async Task SetActualAsync_should_capture_exception_during_actual_value_evaluation_and_expose_it_with_GetActual()
        {
            var expected = RandomValue.Int();

            Expected<int> expectation = expected;
            await expectation.SetActualAsync(() => throw new DivideByZeroException());

            Assert.That(expectation.Status, Is.EqualTo(ParameterVerificationStatus.Exception));
            Assert.That(expectation.ToString(), Is.EqualTo($"expected: equal '{expected}', but got: '<{nameof(DivideByZeroException)}>'"));
            Assert.Throws<DivideByZeroException>(() => expectation.GetActual());
        }

        [Test]
        public void SetActual_should_be_allowed_once()
        {
            Expected<int> expectation = RandomValue.Int();
            expectation.SetActual(() => RandomValue.Int());
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(() => RandomValue.Int()));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
            ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(RandomValue.Int()));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
        }

        [Test]
        public void SetActualAsync_should_be_allowed_once()
        {
            Expected<int> expectation = RandomValue.Int();
            expectation.SetActual(() => RandomValue.Int());
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => expectation.SetActualAsync(() => Task.FromResult(RandomValue.Int())));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
        }

        [Test]
        public void SetValueFormattingService_should_allow_using_custom_formatter()
        {
            var expected = RandomValue.Int();
            var actualValue = RandomValue.Int();
            Expected<int> expectation = expected;
            ((IVerifiableParameter)expectation).SetValueFormattingService(new ValueFormattingServiceStub(CultureInfo.InvariantCulture, "--{0}--"));
            expectation.SetActual(() => actualValue);

            Assert.That(expectation.ToString(), Is.EqualTo($"expected: equal '--{expected}--', but got: '--{actualValue}--'"));
        }

        [Test]
        public void GetValidationException_should_return_exception_if_actual_was_not_set()
        {
            var expected = RandomValue.String();
            Expected<string> expectation = expected;
            var ex = GetException(expectation);
            Assert.That(ex, Is.InstanceOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo($"expected: equal '{expected}', but did not received anything"));
        }

        [Test]
        public void GetValidationException_should_return_exception_if_actual_value_did_not_match()
        {
            var expected = RandomValue.String();
            var actualValue = $"!{expected}";

            Expected<string> expectation = expected;
            expectation.SetActual(() => actualValue);

            var ex = GetException(expectation);
            Assert.That(ex, Is.InstanceOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo($"expected: equal '{expected}', but got: '{actualValue}'"));
        }

        [Test]
        public void GetValidationException_should_return_exception_if_SetActual_failed_with_exception()
        {
            var expected = RandomValue.String();

            Expected<string> expectation = expected;
            expectation.SetActual(() => throw new DivideByZeroException());

            var ex = GetException(expectation);
            Assert.That(ex, Is.InstanceOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo($"expected: equal '{expected}', but got: '<{nameof(DivideByZeroException)}>'"));
            Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
        }

        [Test]
        public void GetValidationException_should_return_null_if_actual_match_expected_value()
        {
            var expected = RandomValue.String();
            Expected<string> expectation = expected;
            expectation.SetActual(expected);

            var ex = GetException(expectation);
            Assert.That(ex, Is.Null);
        }

        private Exception GetException(Expected<string> expectation)
        {
            return ((IVerifiableParameter) expectation).Result.Exception;
        }
    }
}
