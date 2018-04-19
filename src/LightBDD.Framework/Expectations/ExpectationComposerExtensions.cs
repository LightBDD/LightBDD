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

        public static Expectation<string> MatchWild(this IExpectationComposer builder, string pattern)
        {
            return builder.For<string>().MatchWild(pattern);
        }

        public static Expectation<string> MatchWild(this IExpectationComposer<string> builder, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*").Replace("\\#", "\\d") + "$";
            var regex = new Regex(regexPattern);
            return builder.ComposeUnary(formatter => $"matching '{pattern}'", value => value != null && regex.IsMatch(value));
        }

        public static Expectation<string> MatchRegex(this IExpectationComposer builder, string pattern)
        {
            return builder.For<string>().MatchRegex(pattern);
        }

        public static Expectation<string> MatchRegex(this IExpectationComposer<string> builder, string pattern)
        {
            var regex = new Regex(pattern);
            return builder.ComposeUnary(formatter => $"matching regex '{pattern}'", value => value != null && regex.IsMatch(value));
        }

        public static Expectation<T> Null<T>(this IExpectationComposer builder) where T : class
        {
            return builder.For<T>().Null();
        }

        public static Expectation<T> Null<T>(this IExpectationComposer<T> builder) where T : class
        {
            return builder.ComposeUnary(formatter => "null", x => x == null);
        }

        public static Expectation<T> Between<T>(this IExpectationComposer builder, T a, T b) where T : IComparable<T>
        {
            return builder.For<T>().Between(a, b);
        }

        public static Expectation<T> Between<T>(this IExpectationComposer<T> builder, T a, T b) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            return builder.ComposeUnary(formatter => $"between '{formatter.FormatValue(a)}' and '{formatter.FormatValue(b)}'", x => x != null && Math.Abs(x.CompareTo(a) + x.CompareTo(b)) < 2);
        }

        public static Expectation<T> GreaterThan<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            return builder.For<T>().GreaterThan(a);
        }

        public static Expectation<T> GreaterThan<T>(this IExpectationComposer<T> builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary(formatter => $"greater than '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) > 0);
        }

        public static Expectation<T> LessThan<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            return builder.For<T>().LessThan(a);
        }

        public static Expectation<T> LessThan<T>(this IExpectationComposer<T> builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary(formatter => $"less than '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) < 0);
        }

        public static Expectation<T> GreaterOrEqual<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            return builder.For<T>().GreaterOrEqual(a);
        }

        public static Expectation<T> GreaterOrEqual<T>(this IExpectationComposer<T> builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary(formatter => $"greater or equal '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) >= 0);
        }

        public static Expectation<T> LessOrEqual<T>(this IExpectationComposer builder, T a) where T : IComparable<T>
        {
            return builder.For<T>().LessOrEqual(a);
        }

        public static Expectation<T> LessOrEqual<T>(this IExpectationComposer<T> builder, T a) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            return builder.ComposeUnary(formatter => $"less or equal '{formatter.FormatValue(a)}'", x => x != null && x.CompareTo(a) <= 0);
        }
    }
}