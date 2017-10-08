using System;
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
        private IValueFormattingService _formattingService;
        private readonly T _expected;
        private string _formattedExpectedValue;
        private string _formattedActualValue = "<?>";
        private bool _isEqual;
        private Exception _innerException;

        private ExpectedValue(T expected)
        {
            _expected = expected;
        }

        /// <summary>
        /// Sets the actual value provided by <paramref name="actualFn"/> function to verify against expectation using <see cref="IEquatable{T}.Equals(T)"/> if <typeparamref name="T"/> implements it or <see cref="object.Equals(object)"/> if not.
        /// If <paramref name="actualFn"/> throws during execution or actual and expected values does not match, the method will still finish, allowing to set values of other parameters if present, but the executing step would eventually fail after return.
        /// </summary>
        /// <param name="actualFn">Function providing actual value.</param>
        public void SetActual(Func<T> actualFn)
        {
            SetActual(actualFn, AreEqual);
        }

        /// <summary>
        /// Sets the actual value provided by <paramref name="actualFn"/> function to verify against expectation using <paramref name="matchingFn"/> function.
        /// If <paramref name="actualFn"/> throws during execution or actual and expected values does not match, the method will still finish, allowing to set values of other parameters if present, but the executing step would eventually fail after return.
        /// </summary>
        /// <param name="actualFn">Function providing actual value.</param>
        /// <param name="matchingFn">Function that should return true if actual value (1st parameter) match the expectation (2nd parameter).</param>
        public void SetActual(Func<T> actualFn, Func<T, T, bool> matchingFn)
        {
            //TODO: throw if already used
            try
            {
                var actual = actualFn();
                _isEqual = matchingFn(actual, _expected);
                _formattedActualValue = _formattingService.FormatValue(actual);
            }
            catch (Exception e)
            {
                _innerException = e;
                _formattedActualValue = $"<{e.GetType().Name}>";
            }
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
            _formattingService = formattingService;
            _formattedExpectedValue = _formattingService.FormatValue(_expected);
        }

        Exception IVerifiableParameter.GetValidationException()
        {
            return _isEqual
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
            if (!_isEqual)
                return $"expected {_formattedExpectedValue} (but was {_formattedActualValue})";

            return _formattedExpectedValue == _formattedActualValue
                ? _formattedExpectedValue
                : $"{_formattedExpectedValue} (matches {_formattedActualValue})";
        }
    }
}
