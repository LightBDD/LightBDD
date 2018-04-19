using System;
using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations.Helpers
{
    public class ExpectationScenario<T> : IExpectationScenario
    {
        public string Format { get; }

        public ExpectationScenario(string format, Func<IExpectationComposer, Expectation<T>> inferredExpectationFn, Func<IExpectationComposer<T>, Expectation<T>> explicitExpectationFn)
        {
            Format = format;
            Expectation = inferredExpectationFn(Expect.To);
            NegatedExpectation = inferredExpectationFn(Expect.To.Not);
            ExplicitExpectation = explicitExpectationFn(Expect.To.For<T>());
            ExplicitNegatedExpectation = explicitExpectationFn(Expect.To.For<T>().Not);
        }
        public IList<(T value, string failureMessage)> MatchingValues { get; } = new List<(T value, string failureMessage)>();
        public IList<(T value, string failureMessage)> NotMatchingValues { get; } = new List<(T value, string failureMessage)>();

        public ExpectationScenario<T> WithMatchingValue(T value) => WithMatchingValues(value);
        public ExpectationScenario<T> WithMatchingValues(params T[] values)
        {
            foreach (var value in values)
                MatchingValues.Add((value, $"expected: not {Format}, but it was"));
            return this;
        }

        public ExpectationScenario<T> WithNotMatchingValue(T value, string failureMessage)
        {
            NotMatchingValues.Add((value, failureMessage));
            return this;
        }

        public Expectation<T> ExplicitNegatedExpectation { get; }
        public Expectation<T> ExplicitExpectation { get; }
        public Expectation<T> NegatedExpectation { get; }
        public Expectation<T> Expectation { get; }

        public void AssertFormat()
        {
            AssertFormat(Expectation, Format);
            AssertFormat(ExplicitExpectation, Format);
            AssertFormat(NegatedExpectation, "not " + Format);
            AssertFormat(ExplicitNegatedExpectation, "not " + Format);
        }

        public void AssertExpectationMatchingValues()
        {
            foreach (var (value, _) in MatchingValues)
            {
                AssertSuccess(Expectation, value);
                AssertSuccess(ExplicitExpectation, value);
            }
        }

        public void AssertExpectationNotMatchingValues()
        {
            foreach (var (value, expectedMessage) in NotMatchingValues)
            {
                AssertFailure(Expectation, value, expectedMessage);
                AssertFailure(ExplicitExpectation, value, expectedMessage);
            }
        }

        public void AssertNegatedExpectationNotMatchingValues()
        {
            foreach (var (value, expectedMessage) in MatchingValues)
            {
                AssertFailure(NegatedExpectation, value, expectedMessage);
                AssertFailure(ExplicitNegatedExpectation, value, expectedMessage);
            }
        }

        public void AssertNegatedExpectationMatchingValues()
        {
            foreach (var (value, _) in NotMatchingValues)
            {
                AssertSuccess(NegatedExpectation, value);
                AssertSuccess(ExplicitNegatedExpectation, value);
            }
        }

        private void AssertSuccess(Expectation<T> expectation, T value)
        {
            var result = expectation.Verify(value, DefaultValueFormattingService.Instance);
            Assert.True(result, $"{value}");
            Assert.IsEmpty(result.Message, $"{value}");
        }

        private void AssertFailure(Expectation<T> expectation, T value, string expectedMessage)
        {
            var result = expectation.Verify(value, DefaultValueFormattingService.Instance);
            Assert.False(result, $"{value}");
            Assert.That(result.Message, Is.EqualTo(expectedMessage), $"{value}");
        }

        private void AssertFormat(Expectation<T> expectation, string format)
        {
            Assert.That(expectation.Format(DefaultValueFormattingService.Instance), Is.EqualTo(format));
            Assert.That(expectation.ToString(), Is.EqualTo(format));
        }
    }
}