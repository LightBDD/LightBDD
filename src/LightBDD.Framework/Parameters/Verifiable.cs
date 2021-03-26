using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results.Parameters;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Notification.Events;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type allowing to specify verifiable parameters for LightBDD steps, which outcome is inlined in the step name.
    /// It is initialized with expectation expression that has to be fulfilled upon validation in the step method body, with one of <see cref="SetActual(T)"/>, <see cref="SetActual(System.Func{T})"/> or <see cref="SetActualAsync"/> method call.
    /// In contrary to regular assertions, it is possible to evaluate all the verifiable parameters, as mentioned methods does not throw upon failed validation, but their outcome is collected after step method is finished.<br/>
    /// 
    /// Example:
    /// <example>
    /// void Then_user_should_have_login_and_email(Verifiable&lt;string&gt; login, Verifiable&lt;string&gt; email)
    /// {
    ///     login.SetActual(_user.Login);
    ///     email.SetActual(_user.Email);
    /// }
    /// <br/>
    /// _ => Then_user_should_have_login_and_email("bob123", Expect.To.MatchRegex("^\\w{6,12}@mymail\\.com$"))
    /// </example>
    /// </summary>
    /// <typeparam name="T">Type of the expected parameter</typeparam>
    public sealed class Verifiable<T> : IComplexParameter, ITraceableParameter
    {
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        private string _actualText;
        private T _actual;
        private ExpectationResult _result;
        private Exception _exception;
        private IParameterInfo _parameterInfo;
        private IProgressPublisher _progressPublisher;
        private int _setFlag = 0;
        private InlineParameterDetails _details;

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
        public Verifiable(IExpectation<T> expectation)
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
        public Verifiable<T> SetActual(T value)
        {
            AcquireSetFlag();
            try
            {
                NotifyStart();
                return SetActualValue(value);
            }
            catch (Exception e)
            {
                SetActualException(e);
            }
            finally
            {
                NotifyEnd();
            }

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
        public Verifiable<T> SetActual(Func<T> valueFn)
        {
            AcquireSetFlag();
            try
            {
                NotifyStart();
                return SetActualValue(valueFn());
            }
            catch (Exception e)
            {
                SetActualException(e);
            }
            finally
            {
                NotifyEnd();
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
        public async Task<Verifiable<T>> SetActualAsync(Func<Task<T>> valueFn)
        {
            AcquireSetFlag();
            try
            {
                NotifyStart();
                return SetActualValue(await valueFn());
            }
            catch (Exception e)
            {
                SetActualException(e);
            }
            finally
            {
                NotifyEnd();
            }

            return this;
        }

        void ITraceableParameter.InitializeParameterTrace(IParameterInfo parameterInfo, IProgressPublisher progressPublisher)
        {
            _parameterInfo = parameterInfo;
            _progressPublisher = progressPublisher;
            _progressPublisher?.Publish(time => new InlineParameterDiscovered(time, _parameterInfo, GetDetails()));
        }

        private void NotifyStart()
        {
            _progressPublisher?.Publish(time => new InlineParameterValidationStarting(time, _parameterInfo, GetDetails()));
        }

        private void NotifyEnd()
        {
            _progressPublisher?.Publish(time => new InlineParameterValidationFinished(time, _parameterInfo, GetDetails()));
        }

        private void AcquireSetFlag()
        {
            if (Interlocked.Exchange(ref _setFlag, 1) == 1)
                throw new InvalidOperationException("Actual value has been already specified or is being provided");
        }

        void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        IParameterDetails IComplexParameter.Details => GetDetails();

        private string GetValidationMessage()
        {
            if (Status == ParameterVerificationStatus.NotProvided)
                return ToString() + ", but did not received anything";
            var message = _result.IsValid ? null : _result.Message;
            return message ?? ToString();
        }

        private InlineParameterDetails GetDetails()
        {
            return _details ??= new InlineParameterDetails(Expectation.Format(_formattingService), _actualText, Status, GetValidationMessage());
        }

        /// <summary>
        /// Initializes the instance with expectation that the actual value should equal to <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected"></param>
        public static implicit operator Verifiable<T>(T expected)
        {
            return Expect.To.Equal(expected);
        }

        /// <summary>
        /// Initializes the instance with provided expectation.
        /// </summary>
        /// <param name="expectation"></param>
        public static implicit operator Verifiable<T>(Expectation<T> expectation)
        {
            return new Verifiable<T>(expectation);
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

        private Verifiable<T> SetActualValue(T value)
        {
            _exception = null;
            _actual = value;
            _actualText = _formattingService.FormatValue(value);
            _result = Expectation.Verify(value, _formattingService);
            _details = null;
            return this;
        }

        private void SetActualException(Exception e)
        {
            _exception = e;
            _actualText = $"<{e.GetType().Name}>";
            _details = null;
        }
    }
}