using System.Text.RegularExpressions;
using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    public static class ExpectationExtensions
    {
        public static Expected<T> Equal<T>(this IExpectationComposer composer, T expected)
        {
            return composer.For<T>().Equal(expected);
        }

        public static Expected<T> Equal<T>(this IExpectationComposer<T> composer, T expected)
        {
            return composer.Create(new Expectation<T>($"equal '{expected}'", x => Equals(x, expected)));
        }

        public static Expected<string> WildMatch(this IExpectationComposer builder, string pattern)
        {
            return builder.For<string>().WildMatch(pattern);
        }

        public static Expected<string> Match(this IExpectationComposer builder, string pattern)
        {
            return builder.For<string>().Match(pattern);
        }
        public static Expected<string> WildMatch(this IExpectationComposer<string> builder, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*").Replace("\\#", "\\d") + "$";
            var regex = new Regex(regexPattern);
            return builder.Create(new Expectation<string>($"match '{pattern}'", regex.IsMatch));
        }

        public static Expected<string> Match(this IExpectationComposer<string> builder, string pattern)
        {
            var regex = new Regex(pattern);
            return builder.Create(new Expectation<string>($"match regex '{pattern}'", regex.IsMatch));
        }
    }
}