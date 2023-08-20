using System;
using System.Globalization;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.UnitTests.Formatting;
using LightBDD.ScenarioHelpers;
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
            expectation.SetActual(Fake.Int);
            var ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(Fake.Int));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
            ex = Assert.Throws<InvalidOperationException>(() => expectation.SetActual(Fake.Int()));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
        }

        [Test]
        public void SetActualAsync_should_be_allowed_once()
        {
            Verifiable<int> expectation = Fake.Int();
            expectation.SetActual(Fake.Int);
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => expectation.SetActualAsync(() => Task.FromResult(Fake.Int())));
            Assert.That(ex.Message, Is.EqualTo("Actual value has been already specified"));
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
    }
}
