using System;
using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations.Helpers
{
    public class ExpectationScenario<T> : IExpectationScenario
    {
        public string Format { get; }
        public string NegatedFormat { get; }

        public ExpectationScenario(string format, Func<IExpectationComposer, IExpectation<T>> expectationFn)
        {
            Format = format;
            NegatedFormat = "not " + format;
            Expectation = expectationFn(Expect.To);
            NegatedExpectation = expectationFn(Expect.To.Not);
        }
        public IList<(T value, string failureMessage)> MatchingValues { get; } = new List<(T value, string failureMessage)>();
        public IList<(T value, string failureMessage)> NotMatchingValues { get; } = new List<(T value, string failureMessage)>();

        public ExpectationScenario<T> WithMatchingValue(T value) => WithMatchingValues(value);
        public ExpectationScenario<T> WithMatchingValues(params T[] values)
        {
            foreach (var value in values)
                MatchingValues.Add((value, $"expected: {NegatedFormat}, but it was"));
            return this;
        }

        public ExpectationScenario<T> WithNotMatchingValue(T value, string failureMessage)
        {
            NotMatchingValues.Add((value, failureMessage));
            return this;
        }

        public IExpectation<T> NegatedExpectation { get; }
        public IExpectation<T> Expectation { get; }

        public void AssertFormat()
        {
            AssertFormat(Expectation, Format);
            AssertFormat(NegatedExpectation, NegatedFormat);
        }

        public void AssertExpectationMatchingValues()
        {
            foreach (var (value, _) in MatchingValues)
                AssertSuccess(Expectation, value);
        }

        public void AssertExpectationNotMatchingValues()
        {
            foreach (var (value, expectedMessage) in NotMatchingValues)
                AssertFailure(Expectation, value, expectedMessage);
        }

        public void AssertNegatedExpectationNotMatchingValues()
        {
            foreach (var (value, expectedMessage) in MatchingValues)
                AssertFailure(NegatedExpectation, value, expectedMessage);
        }

        public void AssertNegatedExpectationMatchingValues()
        {
            foreach (var (value, _) in NotMatchingValues)
                AssertSuccess(NegatedExpectation, value);
        }

        private void AssertSuccess(IExpectation<T> expectation, T value)
        {
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.True(result, $"{expectation}: {value}");
            Assert.IsEmpty(result.Message, $"{expectation}: {value}");
        }

        private void AssertFailure(IExpectation<T> expectation, T value, string expectedMessage)
        {
            var result = expectation.Verify(value, ValueFormattingServices.Current);
            Assert.False(result, $"{expectation}: {value}");
            Assert.That(result.Message.Replace("\r", ""), Is.EqualTo(expectedMessage.Replace("\r", "")), $"{expectation}: {value}");
        }

        private void AssertFormat(IExpectation<T> expectation, string format)
        {
            Assert.That(expectation.Format(ValueFormattingServices.Current), Is.EqualTo(format));
            Assert.That(expectation.ToString(), Is.EqualTo(format));
        }
    }
}