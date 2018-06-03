using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type allowing to specify verifiable parameters for LightBDD steps, which outcome is inlined in the step name.
    /// It is initialized with expectation expression that has to be fulfilled upon validation in the step method body, with one of <see cref="SetActual(T)"/>, <see cref="SetActual(System.Func{T})"/> or <see cref="SetActualAsync"/> method call.
    /// In contrary to regular assertions, it is possible to evaluate all the verifiable parameters, as mentioned methods does not throw upon failed validation, but their outcome is collected after step method is finished.<br/>
    /// Example:
    /// <example>
    /// void Then_user_should_have_login_and_email(Expected&lt;string&gt; login, Expected&lt;string&gt; email)
    /// {
    ///     login.SetActual(_user.Login);
    ///     email.SetActual(_user.Email);
    /// }
    /// <br/>
    /// _ => Then_user_should_have_login_and_email("bob123", Expect.To.MatchRegex("^\\w{6,12}@mymail\\.com$"))
    /// </example>
    /// </summary>
    /// <typeparam name="T">Type of the expected parameter</typeparam>
    //TODO: consider renaming, consider moving to root
    public sealed class Expected<T> : IComplexParameter
    {
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        private string _actualText;
        private T _actual;
        private ExpectationResult _result;
        private Exception _exception;

        /// <summary>
        /// Specified expectation.
        /// </summary>
        public IExpectation<T> Expectation { get; }

        /// <summary>
        /// Returns actual value if set.
        /// Throws exception if value is not provided or when setting the actual value failed.
        /// </summary>
        /// <returns>Actual value</returns>
        /// <exception cref="InvalidOperationException">Throws when value was not set.</exception>
        /// <exception>Rethrows the exception captured by <see cref="SetActual(Func{T})"/> or <see cref="SetActualAsync"/> methods.</exception>
        public T GetActual()
        {
            if (Status == ParameterVerificationStatus.NotProvided)
                throw new InvalidOperationException("Actual value is not set");
            if (_exception != null)
                ExceptionDispatchInfo.Capture(_exception).Throw();
            return _actual;
        }

        /// <summary>
        /// Returns the status of the expected parameter. 
        /// </summary>
        public ParameterVerificationStatus Status
        {
            get
            {
                if (_exception != null) return ParameterVerificationStatus.Exception;
                if (_actualText == null) return ParameterVerificationStatus.NotProvided;
                if (_result.IsValid) return ParameterVerificationStatus.Success;
                return ParameterVerificationStatus.Failure;
            }
        }

        /// <summary>
        /// Initializes the instance with the provided expectation.
        /// </summary>
        /// <param name="expectation">Expectation.</param>
        public Expected(IExpectation<T> expectation)
        {
            Expectation = expectation;
        }

        /// <summary>
        /// Sets the actual value and performs the validation against the expectation, updating <see cref="Status"/> property.
        /// The value specified by <paramref name="value"/> parameter can be retrieved by <see cref="GetActual"/> method.
        ///
        /// If actual value is already set, an exception is thrown.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown when actual value is already set.</exception>
        public Expected<T> SetActual(T value)
        {
            if (Status != ParameterVerificationStatus.NotProvided)
                throw new InvalidOperationException("Actual value has been already specified");
            _exception = null;
            _actual = value;
            _actualText = _formattingService.FormatValue(value);
            _result = Expectation.Verify(value, _formattingService);
            return this;
        }

        /// <summary>
        /// Sets the actual value and performs the validation against the expectation, updating <see cref="Status"/> property.
        /// The <paramref name="valueFn"/> function is evaluated and it's value can be retrieved later by <see cref="GetActual"/> method.
        /// If <paramref name="valueFn"/> function throws, the exception is captured and <see cref="Status"/> is updated respectively (the <see cref="SetActual(Func{T})"/> does not throw).
        ///
        /// If actual value is already set, an exception is thrown.
        /// </summary>
        /// <param name="valueFn">Function providing actual value.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown when actual value is already set.</exception>
        public Expected<T> SetActual(Func<T> valueFn)
        {
            if (Status != ParameterVerificationStatus.NotProvided)
                throw new InvalidOperationException("Actual value has been already specified");
            try
            {
                return SetActual(valueFn());
            }
            catch (Exception e)
            {
                _exception = e;
                _actualText = $"<{e.GetType().Name}>";
            }

            return this;
        }

        /// <summary>
        /// Sets the actual value and performs the validation against the expectation, updating <see cref="Status"/> property.
        /// The <paramref name="valueFn"/> function is evaluated and it's value can be retrieved later by <see cref="GetActual"/> method.
        /// If <paramref name="valueFn"/> function throws, the exception is captured and <see cref="Status"/> is updated respectively (the <see cref="SetActualAsync"/> does not throw).
        ///
        /// If actual value is already set, an exception is thrown.
        /// </summary>
        /// <param name="valueFn">Function providing actual value.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown when actual value is already set.</exception>
        public async Task<Expected<T>> SetActualAsync(Func<Task<T>> valueFn)
        {
            if (Status != ParameterVerificationStatus.NotProvided)
                throw new InvalidOperationException("Actual value has been already specified");
            try
            {
                return SetActual(await valueFn());
            }
            catch (Exception e)
            {
                _exception = e;
                _actualText = $"<{e.GetType().Name}>";
            }

            return this;
        }

        void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        IParameterDetails IComplexParameter.Details => new InlineParameterDetails(Expectation.Format(_formattingService), _actualText, Status, GetValidationMessage());

        private string GetValidationMessage()
        {
            if (Status == ParameterVerificationStatus.NotProvided)
                return ToString() + ", but did not received anything";
            var message = _result.IsValid ? null : _result.Message;
            return message ?? ToString();
        }

        /// <summary>
        /// Initializes the instance with expectation that the actual value should equal to <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected"></param>
        public static implicit operator Expected<T>(T expected)
        {
            return Expect.To.Equal(expected);
        }

        /// <summary>
        /// Initializes the instance with provided expectation.
        /// </summary>
        /// <param name="expectation"></param>
        public static implicit operator Expected<T>(Expectation<T> expectation)
        {
            return new Expected<T>(expectation);
        }

        /// <summary>
        /// Returns the text reflecting actual state.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_actualText == null)
                return $"expected: {Expectation.Format(_formattingService)}";
            if (_result.IsValid)
                return $"{_actualText}";
            return $"expected: {Expectation.Format(_formattingService)}, but got: '{_actualText}'";
        }
    }
}