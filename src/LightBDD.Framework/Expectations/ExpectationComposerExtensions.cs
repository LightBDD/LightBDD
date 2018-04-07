using System;
using System.Text.RegularExpressions;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    public static class ExpectationComposerExtensions
    {
        public static Expectation<T> ComposeUnary<T>(this IExpectationComposer<T> composer, Func<IValueFormattingService, string> descriptionFn, Func<T, bool> predicateFn)
        {
            return composer.Compose(new UnaryExpectation<T>(descriptionFn, predicateFn));
        }

        public static Expectation<T> Equal<T>(this IExpectationComposer composer, T expected)
        {
            return composer.For<T>().Equal(expected);
        }

        public static Expectation<T> Equal<T>(this IExpectationComposer<T> composer, T expected)
        {
            return composer.ComposeUnary(
                formatter => $"equal '{formatter.FormatValue(expected)}'",
                x => Equals(x, expected));
        }

        public static Expectation<string> WildMatch(this IExpectationComposer builder, string pattern)
        {
            return builder.For<string>().WildMatch(pattern);
        }

        public static Expectation<string> WildMatch(this IExpectationComposer<string> builder, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*").Replace("\\#", "\\d") + "$";
            var regex = new Regex(regexPattern);
            return builder.ComposeUnary(formatter => $"match '{pattern}'", regex.IsMatch);
        }

        public static Expectation<string> Match(this IExpectationComposer builder, string pattern)
        {
            return builder.For<string>().Match(pattern);
        }

        public static Expectation<string> Match(this IExpectationComposer<string> builder, string pattern)
        {
            var regex = new Regex(pattern);
            return builder.ComposeUnary(formatter => $"match regex '{pattern}'", regex.IsMatch);
        }
    }
}