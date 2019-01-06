using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations.Implementation;
using LightBDD.Framework.Parameters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Extensions offering methods for defining expectations.
    /// </summary>
    public static class ExpectationExtensions
    {
        /// <summary>
        /// Helper method creating simple expectation based on <paramref name="predicateFn"/> and <paramref name="descriptionFn"/>.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="descriptionFn">Expectation description</param>
        /// <param name="predicateFn">Expectation predicate</param>
        public static Expectation<T> ComposeSimple<T>(this IExpectationComposer composer, Func<IValueFormattingService, string> descriptionFn, Func<T, bool> predicateFn)
        {
            return composer.Compose(new SimpleExpectation<T>(descriptionFn, predicateFn));
        }

        /// <summary>
        /// Creates expectation for values to be equal to <paramref name="expected"/> value.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="expected">Expected value</param>
        public static Expectation<T> Equal<T>(this IExpectationComposer composer, T expected)
        {
            return composer.ComposeSimple<T>(
                formatter => $"equals '{formatter.FormatValue(expected)}'",
                x => Equals(x, expected));
        }

        /// <summary>
        /// Creates expectation for values to be equal one of values in <paramref name="expectedCollection"/>.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="expectedCollection">Collection of expected values</param>
        public static Expectation<T> BeIn<T>(this IExpectationComposer composer, params T[] expectedCollection)
        {
            return composer.ComposeSimple<T>(
                formatter => $"in '{formatter.FormatValue(expectedCollection)}'",
                expectedCollection.Contains);
        }

        /// <summary>
        /// Creates expectation for collections to contain value specified by <paramref name="value"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="value">Expected value</param>
        public static Expectation<IEnumerable<T>> Contain<T>(this IExpectationComposer composer, T value)
        {
            return composer.ComposeSimple<IEnumerable<T>>(
                formatter => $"contains '{formatter.FormatValue(value)}'",
                x => x != null && x.Contains(value));
        }

        /// <summary>
        /// Creates expectation for collections to equal sequence specified by <paramref name="collection"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="collection">Expected collection</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        public static Expectation<IEnumerable<T>> EqualCollection<T>(this IExpectationComposer composer, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            return composer.Compose(new EqualCollection<T>(collection));
        }

        /// <summary>
        /// Creates expectation for collections to equal sequence specified by <paramref name="collection"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="collection">Expected collection</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        public static Expectation<IEnumerable<T>> EqualCollection<T>(this IExpectationComposer composer, params T[] collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            return composer.EqualCollection(collection.AsEnumerable());
        }

        /// <summary>
        /// Creates expectation for collections to equivalent sequence specified by <paramref name="collection"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="collection">Expected collection</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        public static Expectation<IEnumerable<T>> EquivalentCollection<T>(this IExpectationComposer composer, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            return composer.Compose(new EquivalentCollection<T>(collection));
        }

        /// <summary>
        /// Creates expectation for collections to equivalent sequence specified by <paramref name="collection"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="collection">Expected collection</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        public static Expectation<IEnumerable<T>> EquivalentCollection<T>(this IExpectationComposer composer, params T[] collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            return composer.EquivalentCollection(collection.AsEnumerable());
        }

        /// <summary>
        /// Creates expectation for collections with at least one item fulfilling expectation specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<IEnumerable<T>> AnyItem<T>(this IExpectationComposer composer, Func<IExpectationComposer, IExpectation<T>> expectationBuilder)
        {
            return composer.Compose(new AnyItemExpectation<T>(expectationBuilder.Invoke(Expect.To)));
        }

        /// <summary>
        /// Creates expectation for collections with every item fulfilling expectation specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<IEnumerable<T>> EveryItem<T>(this IExpectationComposer composer, Func<IExpectationComposer, IExpectation<T>> expectationBuilder)
        {
            return composer.Compose(new EveryItemExpectation<T>(expectationBuilder.Invoke(Expect.To)));
        }

        /// <summary>
        /// Creates expectation for strings to match pattern specified by <paramref name="pattern"/> parameter.
        ///
        /// The <paramref name="pattern"/> may have special characters:
        /// <list type="bullet">
        /// <item><description>* - 0-more characters</description></item>
        /// <item><description>? - 1 character</description></item>
        /// <item><description># - 1 digit</description></item>
        /// </list>
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> BeLike(this IExpectationComposer composer, string pattern)
        {
            return BeLike(composer, pattern, RegexOptions.None, $"like '{pattern}'");
        }

        /// <summary>
        /// Creates expectation for strings to match pattern specified by <paramref name="pattern"/> parameter, where character case is ignored.
        ///
        /// The <paramref name="pattern"/> may have special characters:
        /// <list type="bullet">
        /// <item><description>* - 0-more characters</description></item>
        /// <item><description>? - 1 character</description></item>
        /// <item><description># - 1 digit</description></item>
        /// </list>
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> BeLikeIgnoreCase(this IExpectationComposer composer, string pattern)
        {
            return BeLike(composer, pattern, RegexOptions.IgnoreCase, $"like '{pattern}' ignore case");
        }

        private static Expectation<string> BeLike(IExpectationComposer composer, string pattern, RegexOptions options, string format)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*").Replace("\\#", "\\d") + "$";
            var regex = new Regex(regexPattern, options);
            return composer.ComposeSimple<string>(formatter => format, value => value != null && regex.IsMatch(value));
        }

        /// <summary>
        /// Creates expectation for strings to match regex pattern specified by <paramref name="pattern"/> parameter.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> Match(this IExpectationComposer composer, string pattern)
        {
            return Match(composer, pattern, RegexOptions.None, $"matches '{pattern}'");
        }

        /// <summary>
        /// Creates expectation for strings to match regex pattern specified by <paramref name="pattern"/> parameter, where character case is ignored.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> MatchIgnoreCase(this IExpectationComposer composer, string pattern)
        {
            return Match(composer, pattern, RegexOptions.IgnoreCase, $"matches '{pattern}' ignore case");
        }

        private static Expectation<string> Match(IExpectationComposer composer, string pattern, RegexOptions options, string format)
        {
            var regex = new Regex($"^{pattern}$", options);
            return composer.ComposeSimple(formatter => format, (string value) => value != null && regex.IsMatch(value));
        }

        /// <summary>
        /// Creates expectation for values to be null.
        /// </summary>
        /// <param name="builder"></param>
        public static Expectation<object> BeNull(this IExpectationComposer builder)
        {
            return builder.ComposeSimple<object>(formatter => "null", x => x == null);
        }

        /// <summary>
        /// Creates expectation for values to be null.
        /// </summary>
        /// <param name="builder"></param>
        public static Expectation<T> BeNull<T>(this IExpectationComposer builder) where T : class
        {
            return builder.ComposeSimple<T>(formatter => "null", x => x == null);
        }

        /// <summary>
        /// Creates expectation for collections to be empty.
        /// </summary>
        /// <param name="builder"></param>
        public static Expectation<IEnumerable> BeEmpty(this IExpectationComposer builder)
        {
            return builder.ComposeSimple<IEnumerable>(formatter => "empty", x => x != null && !x.Cast<object>().Any());
        }

        /// <summary>
        /// Creates expectation for collections to be empty.
        /// </summary>
        /// <param name="builder"></param>
        public static Expectation<T> BeEmpty<T>(this IExpectationComposer builder) where T : IEnumerable
        {
            return builder.ComposeSimple<T>(formatter => "empty", x => x != null && !x.Cast<object>().Any());
        }

        /// <summary>
        /// Creates expectation for comparable types to be between values specified by <paramref name="a"/> and <paramref name="b"/> parameters, where parameter values are not included.
        /// The <paramref name="a"/> parameter value may be greater or lower than value of <paramref name="b"/> - both scenarios are supported.
        /// None of the provided parameters can be null.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="a">Parameter A.</param>
        /// <param name="b">Parameter B.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="a"/> or <paramref name="b"/> is null.</exception>
        public static Expectation<T> BeBetween<T>(this IExpectationComposer composer, T a, T b) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            return composer.ComposeSimple<T>(formatter => $"between '{formatter.FormatValue(a)}' and '{formatter.FormatValue(b)}'", x => x != null && Math.Abs(x.CompareTo(a) + x.CompareTo(b)) < 2);
        }

        /// <summary>
        /// Creates expectation for comparable types to be greater than value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> BeGreaterThan<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"greater than '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) > 0);
        }

        /// <summary>
        /// Creates expectation for comparable types to be less than value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> BeLessThan<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"less than '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) < 0);
        }

        /// <summary>
        /// Creates expectation for comparable types to be greater or equal value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> BeGreaterOrEqual<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"greater or equal '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) >= 0);
        }

        /// <summary>
        /// Creates expectation for comparable types to be less or equal value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer">Composer</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> BeLessOrEqual<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"less or equal '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) <= 0);
        }

        /// <summary>
        /// Creates expectation for values to fulfill all expectations specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<T> BeAllTrue<T>(this IExpectationComposer composer, params Func<IExpectationComposer, IExpectation<T>>[] expectationBuilder)
        {
            return composer.Compose(new AndExpectation<T>("all true ", expectationBuilder.Select(x => x.Invoke(Expect.To)).ToArray()));
        }

        /// <summary>
        /// Creates expectation for values to fulfill any expectation specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="composer">Composer</param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<T> BeAnyTrue<T>(this IExpectationComposer composer, params Func<IExpectationComposer, IExpectation<T>>[] expectationBuilder)
        {
            return composer.Compose(new OrExpectation<T>("any true ", expectationBuilder.Select(x => x.Invoke(Expect.To)).ToArray()));
        }

        /// <summary>
        /// Combines the existing expectation with one specified by <paramref name="andExpectation"/> parameter where both have to be fulfilled by values.
        /// </summary>
        public static Expectation<T> And<T>(this IExpectation<T> expectation, Func<IExpectationComposer, IExpectation<T>> andExpectation)
        {
            return new AndExpectation<T>(string.Empty, expectation, andExpectation(Expect.To));
        }

        /// <summary>
        /// Combines the existing expectation with one specified by <paramref name="orExpectation"/> parameter where at least one has to be fulfilled by values.
        /// </summary>
        public static Expectation<T> Or<T>(this IExpectation<T> expectation, Func<IExpectationComposer, IExpectation<T>> orExpectation)
        {
            return new OrExpectation<T>(string.Empty, expectation, orExpectation(Expect.To));
        }

        /// <summary>
        /// Creates a base type expectation for given expectation, that internally will cast <typeparamref name="TBase"/> to <typeparamref name="TDerived"/> during evaluation.
        ///
        /// Example usage: <code>Expect.To.MatchRegex("[0-9]+").CastFrom(Expect.Type&lt;object&gt;)</code>
        /// </summary>
        /// <typeparam name="TDerived">Derived type.</typeparam>
        /// <typeparam name="TBase">Base type.</typeparam>
        /// <param name="expectation">Expectation.</param>
        /// <param name="baseTypeRef">Base type.</param>
        /// <returns>Base type expectation.</returns>
        public static Expectation<TBase> CastFrom<TDerived, TBase>(this IExpectation<TDerived> expectation, TypeRef<TBase> baseTypeRef) where TDerived : TBase
        {
            return new CastExpectation<TDerived, TBase>(expectation);
        }

        /// <summary>
        /// Creates <see cref="Verifiable{T}"/> out of the expectation.
        /// </summary>
        /// <typeparam name="T">Expectation value type</typeparam>
        /// <param name="expectation">Expectation</param>
        /// <returns>Verifiable.</returns>
        public static Verifiable<T> ToVerifiable<T>(this IExpectation<T> expectation)
        {
            return new Verifiable<T>(expectation);
        }
    }
}