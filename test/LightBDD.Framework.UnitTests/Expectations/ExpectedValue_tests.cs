using System;
using System.Globalization;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Formatting;
using NUnit.Framework;
using RandomTestValues;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class ExpectedValue_tests
    {
        [Test]
        public void It_should_initialize_instance_with_implicit_cast()
        {
            var expectedValue = RandomValue.Int();

            ExpectedValue<int> expectation = expectedValue;
            Assert.That(expectation.Expected, Is.EqualTo(expectedValue));
        }

        [Test]
        public void It_should_initialize_instance_without_actual_value()
        {
            ExpectedValue<int> expectation = RandomValue.Int();
            Assert.That(expectation.HasActual, Is.False);
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.GetActual());
            Assert.That(ex.Message, Is.EqualTo("Actual value is not set"));
        }

        [Test]
        public void It_should_initialize_instance_with_default_formatting()
        {
            var expectedValue = RandomValue.Int();
            ExpectedValue<int> expectation = expectedValue;

            Assert.That(expectation.ToString(),
                Is.EqualTo(string.Format(CultureInfo.InvariantCulture, "expected {0} (but was <?>)", expectedValue)));
        }

        [Test]
        public void SetActual_should_set_actual_value()
        {
            var expectedValue = RandomValue.Int();
            var actualValue = RandomValue.Int();
            ExpectedValue<int> expectation = expectedValue;
            expectation.SetActual(() => actualValue);

            Assert.That(expectation.HasActual, Is.True);
            Assert.That(expectation.GetActual(), Is.EqualTo(actualValue));
        }

        [Test]
        [TestCase(55, 55, true, "55")]
        [TestCase(55, 32, false, "expected 55 (but was 32)")]
        public void SetActual_should_verify_expectation_and_update_formatting(int expected, int actual, bool shouldMatch, string message)
        {
            ExpectedValue<int> expectation = expected;
            expectation.SetActual(() => actual);

            Assert.That(expectation.IsMatching, Is.EqualTo(shouldMatch));
            Assert.That(expectation.ToString(), Is.EqualTo(message));
        }

        [Test]
        [TestCase(55, 55, true, "55")]
        [TestCase(55, 32, false, "expected 55 (but was 32)")]
        public async Task SetActualAsync_should_verify_expectation_and_update_formatting(int expected, int actual, bool shouldMatch, string message)
        {
            ExpectedValue<int> expectation = expected;
            await expectation.SetActualAsync(async () =>
            {
                await Task.Yield();
                return actual;
            });

            Assert.That(expectation.IsMatching, Is.EqualTo(shouldMatch));
            Assert.That(expectation.ToString(), Is.EqualTo(message));
        }

        [Test]
        public void SetActual_should_verify_expectation_and_update_formatting_with_matching_but_not_the_same_values()
        {
            var expectedValue = RandomValue.Int();
            var actualValue = RandomValue.Int();

            ExpectedValue<int> expectation = expectedValue;
            expectation.SetActual(() => actualValue, (e, a) => true);

            Assert.That(expectation.IsMatching, Is.True);
            Assert.That(expectation.ToString(), Is.EqualTo($"{expectedValue} (matches {actualValue})"));
        }

        [Test]
        public void SetActual_should_capture_exception_during_actual_value_evaluation_and_expose_it_with_GetActual()
        {
            var expectedValue = RandomValue.Int();

            ExpectedValue<int> expectation = expectedValue;
            expectation.SetActual(() => throw new DivideByZeroException());

            Assert.That(expectation.HasActual, Is.True);
            Assert.That(expectation.ToString(), Is.EqualTo($"expected {expectedValue} (but was <{nameof(DivideByZeroException)}>)"));
            Assert.Throws<DivideByZeroException>(() => expectation.GetActual());
        }

        [Test]
        public void SetActual_should_capture_exception_during_values_comparison_and_expose_it_with_GetActual()
        {
            var expectedValue = RandomValue.Int();
            var actualValue = RandomValue.Int();

            ExpectedValue<int> expectation = expectedValue;
            expectation.SetActual(() => actualValue, (e, a) => throw new DivideByZeroException());

            Assert.That(expectation.HasActual, Is.True);
            Assert.That(expectation.ToString(), Is.EqualTo($"expected {expectedValue} (but was <{nameof(DivideByZeroException)}>)"));
            Assert.Throws<DivideByZeroException>(() => expectation.GetActual());
        }

        [Test]
        public void SetActual_should_be_allowed_once()
        {
            ExpectedValue<int> expectation = RandomValue.Int();
            expectation.SetActual(() => RandomValue.Int());
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(() => RandomValue.Int()));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
        }

        [Test]
        public void SetValueFormattingService_should_update_formatting_of_expected_and_future_actual_value()
        {
            var expectedValue = RandomValue.Int();
            var actualValue = RandomValue.Int();
            ExpectedValue<int> expectation = expectedValue;
            ((IVerifiableParameter)expectation).SetValueFormattingService(new ValueFormattingServiceStub(CultureInfo.InvariantCulture, "--{0}--"));
            expectation.SetActual(() => actualValue);

            Assert.That(expectation.ToString(), Is.EqualTo($"expected --{expectedValue}-- (but was --{actualValue}--)"));
        }

        [Test]
        public void GetValidationException_should_return_exception_if_actual_was_not_set()
        {
            var expectedValue = RandomValue.String();
            ExpectedValue<string> expectation = expectedValue;
            var ex = ((IVerifiableParameter)expectation).GetValidationException();
            Assert.That(ex, Is.InstanceOf<ArgumentException>());
            Assert.That(ex.Message, Is.EqualTo($"expected {expectedValue} (but was <?>)"));
        }

        [Test]
        public void GetValidationException_should_return_exception_if_actual_value_did_not_match()
        {
            var expectedValue = RandomValue.String();
            var actualValue = $"!{expectedValue}";

            ExpectedValue<string> expectation = expectedValue;
            expectation.SetActual(() => actualValue);

            var ex = ((IVerifiableParameter)expectation).GetValidationException();
            Assert.That(ex, Is.InstanceOf<ArgumentException>());
            Assert.That(ex.Message, Is.EqualTo($"expected {expectedValue} (but was {actualValue})"));
        }

        [Test]
        public void GetValidationException_should_return_exception_if_SetActual_failed_with_exception()
        {
            var expectedValue = RandomValue.String();

            ExpectedValue<string> expectation = expectedValue;
            expectation.SetActual(() => throw new DivideByZeroException());

            var ex = ((IVerifiableParameter)expectation).GetValidationException();
            Assert.That(ex, Is.InstanceOf<ArgumentException>());
            Assert.That(ex.Message, Is.EqualTo($"expected {expectedValue} (but was <{nameof(DivideByZeroException)}>)"));
            Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
        }

        [Test]
        public void GetValidationException_should_return_null_if_actual_match_expected_value()
        {
            ExpectedValue<string> expectation = RandomValue.String();
            expectation.SetActual(RandomValue.String, (e, a) => true);

            var ex = ((IVerifiableParameter)expectation).GetValidationException();
            Assert.That(ex, Is.Null);
        }
    }
}
