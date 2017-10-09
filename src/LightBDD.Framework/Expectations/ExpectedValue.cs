using System;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Class that can be used as a step method parameter to hold an expected value that will be verified during step execution with <see cref="SetActual(System.Func{T},System.Func{T,T,bool})"/> method.
    /// The difference from standard assertion mechanism is that value quality or inequality would be displayed in step name itself and framework will perform verification of all the parameters before failing a step.
    /// </summary>
    public class ExpectedValue<T> : IVerifiableParameter
    {
        private Func<object, string> _formattingFunc = DefaultFormatValue;
        private string _formattedExpectedValue;
        private string _formattedActualValue = "<?>";
        private Exception _innerException;
        private T _actual;

        /// <summary>
        /// Returns expected value.
        /// </summary>
        public T Expected { get; }

        /// <summary>
        /// Returns true if actual value is set.
        /// </summary>
        public bool HasActual { get; private set; }

        /// <summary>
        /// Returns actual value or throws <see cref="InvalidOperationException"/> if it was not set.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if no actual value is set.</exception>
        public T GetActual()
        {
            if (_innerException != null)
                ExceptionDispatchInfo.Capture(_innerException).Throw();

            return HasActual ?
                _actual
                : throw new InvalidOperationException("Actual value is not set");
        }

        /// <summary>
        /// Returns true if actual value is provided and matches expected value.
        /// </summary>
        public bool IsMatching { get; private set; }

        private ExpectedValue(T expected)
        {
            Expected = expected;
            _formattedExpectedValue = _formattingFunc(expected);
        }

        /// <summary>
        /// Sets the actual value provided by <paramref name="actualFn"/> function to verify against expectation using <see cref="IEquatable{T}.Equals(T)"/> if <typeparamref name="T"/> implements it or <see cref="object.Equals(object)"/> if not.
        /// If <paramref name="actualFn"/> throws during execution or actual and expected values does not match, the method will still finish, allowing to set values of other parameters if present, but the executing step would eventually fail after return.
        /// </summary>
        /// <param name="actualFn">Function providing actual value.</param>
        public ExpectedValue<T> SetActual(Func<T> actualFn)
        {
            return SetActual(actualFn, AreEqual);
        }

        /// <summary>
        /// Sets the actual value provided by <paramref name="actualFn"/> function to verify against expectation using <see cref="IEquatable{T}.Equals(T)"/> if <typeparamref name="T"/> implements it or <see cref="object.Equals(object)"/> if not.
        /// If <paramref name="actualFn"/> throws during execution or actual and expected values does not match, the method will still finish, allowing to set values of other parameters if present, but the executing step would eventually fail after return.
        /// </summary>
        /// <param name="actualFn">Function providing actual value.</param>
        public Task<ExpectedValue<T>> SetActualAsync(Func<Task<T>> actualFn)
        {
            return SetActualAsync(actualFn, AreEqual);
        }

        /// <summary>
        /// Sets the actual value provided by <paramref name="actualFn"/> function to verify against expectation using <paramref name="matchingFn"/> function.
        /// If <paramref name="actualFn"/> throws during execution or actual and expected values does not match, the method will still finish, allowing to set values of other parameters if present, but the executing step would eventually fail after return.
        /// </summary>
        /// <param name="actualFn">Function providing actual value.</param>
        /// <param name="matchingFn">Function that should return true if the expectation (1st parameter) match actual value (2nd parameter).</param>
        /// <exception cref="InvalidOperationException">Thrown if actual value has been already set.</exception>
        public ExpectedValue<T> SetActual(Func<T> actualFn, Func<T, T, bool> matchingFn)
        {
            return SetActualAsync(() => Task.FromResult(actualFn()), matchingFn).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sets the actual value provided by <paramref name="actualFn"/> function to verify against expectation using <paramref name="matchingFn"/> function.
        /// If <paramref name="actualFn"/> throws during execution or actual and expected values does not match, the method will still finish, allowing to set values of other parameters if present, but the executing step would eventually fail after return.
        /// </summary>
        /// <param name="actualFn">Function providing actual value.</param>
        /// <param name="matchingFn">Function that should return true if the expectation (1st parameter) match actual value (2nd parameter).</param>
        /// <exception cref="InvalidOperationException">Thrown if actual value has been already set.</exception>
        public async Task<ExpectedValue<T>> SetActualAsync(Func<Task<T>> actualFn, Func<T, T, bool> matchingFn)
        {
            if (HasActual)
                throw new InvalidOperationException("Actual value has been already specified");

            try
            {
                var actual = await actualFn();
                var isEqual = matchingFn(Expected, actual);
                _formattedActualValue = _formattingFunc(actual);
                IsMatching = isEqual;
                _actual = actual;
            }
            catch (Exception e)
            {
                _innerException = e;
                _formattedActualValue = $"<{e.GetType().Name}>";
            }
            finally
            {
                HasActual = true;
            }
            return this;
        }

        private static bool AreEqual(T expected, T actual)
        {
            return expected is IEquatable<T> equatable
                ? equatable.Equals(actual)
                : Equals(expected, actual);
        }

        /// <summary>
        /// Implicit converter.
        /// </summary>
        public static implicit operator ExpectedValue<T>(T expected)
        {
            return new ExpectedValue<T>(expected);
        }

        void IVerifiableParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingFunc = formattingService.FormatValue;
            _formattedExpectedValue = _formattingFunc(Expected);
        }

        Exception IVerifiableParameter.GetValidationException()
        {
            return IsMatching
                ? null
                : new ArgumentException(ToString(), _innerException);
        }

        /// <summary>
        /// Returns current state of the <see cref="ExpectedValue{T}"/>.
        /// </summary>
        public override string ToString()
        {
            return Format();
        }

        private string Format()
        {
            if (!IsMatching)
                return $"expected {_formattedExpectedValue} (but was {_formattedActualValue})";

            return _formattedExpectedValue == _formattedActualValue
                ? _formattedExpectedValue
                : $"{_formattedExpectedValue} (matches {_formattedActualValue})";
        }

        private static string DefaultFormatValue(object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }
    }
}
