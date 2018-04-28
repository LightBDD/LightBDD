using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    public sealed class Expected<T> : IVerifiableParameter
    {
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        private Exception _exception;
        private string _actualText;
        private T _actual;
        private ExpectationResult _result;
        public Expectation<T> Expectation { get; }

        public T GetActual()
        {
            if (Status == ParameterVerificationStatus.NotProvided)
                throw new InvalidOperationException("Actual value is not set");
            if (_exception != null)
                ExceptionDispatchInfo.Capture(_exception).Throw();
            return _actual;
        }

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

        public Expected(Expectation<T> expectation)
        {
            Expectation = expectation;
        }

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

        void IVerifiableParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        Exception IVerifiableParameter.GetValidationException()
        {
            if (Status == ParameterVerificationStatus.NotProvided)
                return new InvalidOperationException(ToString() + ", but did not received anything");
            return _result.IsValid ? null : new InvalidOperationException(_result.Message ?? ToString(), _exception);
        }

        public static implicit operator Expected<T>(T expected)
        {
            return Expect.To.Equal(expected);
        }

        public static implicit operator Expected<T>(Expectation<T> expectation)
        {
            return new Expected<T>(expectation);
        }

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