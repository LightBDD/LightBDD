using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    public static class ExpectationComposerExtensions
    {
        public static Expectation<T> ComposeUnary<T>(this IExpectationComposer composer, Func<IValueFormattingService, string> descriptionFn, Func<T, bool> predicateFn)
        {
            return composer.Compose(new UnaryExpectation<T>(descriptionFn, predicateFn));
        }

        public static Expectation<T> Equal<T>(this IExpectationComposer composer, T expected)
        {
            return composer.ComposeUnary<T>(
                formatter => $"equal '{formatter.FormatValue(expected)}'",
                x => Equals(x, expected));
        }

        public static Expectation<T> In<T>(this IExpectationComposer composer, params T[] expectedCollection)
        {
            return composer.ComposeUnary<T>(
                formatter => $"in '{formatter.FormatValue(expectedCollection)}'",
                expectedCollection.Contains);
        }

        public static Expectation<IEnumerable<T>> Contains<T>(this IExpectationComposer composer, T value)
        {
            return composer.ComposeUnary<IEnumerable<T>>(
                formatter => $"contains '{formatter.FormatValue(value)}'",
                x => x != null && x.Contains(value));
        }

        public static Expectation<IEnumerable<T>> Any<T>(this IExpectationComposer composer, Func<IExpectationComposer, IExpectation<T>> expectationBuilder)
        {
            return composer.Compose(new AnyExpectation<T>(expectationBuilder.Invoke(Expect.To)));
        }

        public static Expectation<IEnumerable<T>> All<T>(this IExpectationComposer composer, Func<IExpectationComposer, IExpectation<T>> expectationBuilder)
        {
            return composer.Compose(new AllExpectation<T>(expectationBuilder.Invoke(Expect.To)));
        }

        public static Expectation<string> MatchWild(this IExpectationComposer builder, string pattern)
        {
            return MatchWild(builder, pattern, RegexOptions.None, $"matching '{pattern}'");
        }

        public static Expectation<string> MatchWildIgnoreCase(this IExpectationComposer builder, string pattern)
        {
            return MatchWild(builder, pattern, RegexOptions.IgnoreCase, $"matching any case '{pattern}'");
        }

        private static Expectation<string> MatchWild(IExpectationComposer builder, string pattern, RegexOptions options, string format)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*").Replace("\\#", "\\d") + "$";
            var regex = new Regex(regexPattern, options);
            return builder.ComposeUnary<string>(formatter => format, value => value != null && regex.IsMatch(value));
        }

        public static Expectation<string> MatchRegex(this IExpectationComposer builder, string pattern)
        {
            return MatchRegex(builder, pattern, RegexOptions.None, $"matching regex '{pattern}'");
        }

        public static Expectation<string> MatchRegexIgnoreCase(this IExpectationComposer builder, string pattern)
        {
            return MatchRegex(builder, pattern, RegexOptions.IgnoreCase, $"matching regex any case '{pattern}'");
        }

        private static Expectation<string> MatchRegex(IExpectationComposer builder, string pattern, RegexOptions options, string format)
        {
            var regex = new Regex(pattern, options);
            return builder.ComposeUnary(formatter => format, (string value) => value != null && regex.IsMatch(value));
        }

        public static Expectation<object> Null(this IExpectationComposer builder)
        {
            return builder.ComposeUnary<object>(formatter => "null", x => x == null);
        }

        public static Expectation<T> Between<T>(this IExpectationComposer builder, T a, T b) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            return builder.ComposeUnary<T>(formatter => $"between '{formatter.FormatValue(a)}' and '{formatter.FormatValue(b)}'", x => x != null && Math.Abs(x.CompareTo(a) + x.CompareTo(b)) < 2);
        }

        public static Expectation<T> GreaterThan<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary<T>(formatter => $"greater than '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) > 0);
        }

        public static Expectation<T> LessThan<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary<T>(formatter => $"less than '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) < 0);
        }

        public static Expectation<T> GreaterOrEqual<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary<T>(formatter => $"greater or equal '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) >= 0);
        }

        public static Expectation<T> LessOrEqual<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary<T>(formatter => $"less or equal '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) <= 0);
        }

        public static Expectation<T> And<T>(this Expectation<T> left, Func<IExpectationComposer, Expectation<T>> right)
        {
            return new AndExpectation<T>(left, right(Expect.To));
        }

        public static Expectation<T> Or<T>(this Expectation<T> left, Func<IExpectationComposer, Expectation<T>> right)
        {
            return new OrExpectation<T>(left, right(Expect.To));
        }
    }
}