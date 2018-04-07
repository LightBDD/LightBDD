using System;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;

namespace LightBDD.Framework.Expectations
{
    public sealed class Expected<T> : IVerifiableParameter
    {
        private IValueFormattingService _formattingService;
        private string _actualText;
        private ExpectationResult _result;
        public Expectation<T> Expectation { get; }
        public T Actual { get; private set; }
        public Exception Exception { get; private set; }
        public bool IsValid => _result.IsValid;
        public ParameterVerificationStatus Status
        {
            get
            {
                if (Exception != null) return ParameterVerificationStatus.Exception;
                if (_actualText == null) return ParameterVerificationStatus.NotProvided;
                if (IsValid) return ParameterVerificationStatus.Success;
                return ParameterVerificationStatus.Failure;
            }
        }


        public Expected(Expectation<T> expectation)
        {
            Expectation = expectation;
        }

        public Expected<T> SetActual(T value)
        {
            Exception = null;
            Actual = value;
            _actualText = _formattingService.FormatValue(value);
            _result = Expectation.Verify(value,_formattingService);
            return this;
        }

        public Expected<T> SetActual(Func<T> valueFn)
        {
            try
            {
                return SetActual(valueFn());
            }
            catch (Exception e)
            {
                Exception = e;
                _actualText = "<exception>";
            }

            return this;
        }

        public async Task<Expected<T>> SetActualAsync(Func<Task<T>> valueFn)
        {
            try
            {
                return SetActual(await valueFn());
            }
            catch (Exception e)
            {
                Exception = e;
                _actualText = "<exception>";
            }

            return this;
        }

        public void SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        public Exception GetValidationException()
        {
            if (Status == ParameterVerificationStatus.NotProvided)
                return new InvalidOperationException(ToString() + ", but did not received anything");
            return IsValid ? null : new InvalidOperationException(_result.Message, Exception);
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
            if (IsValid)
                return $"{_actualText}";
            return $"expected: {Expectation.Format(_formattingService)}, but got: '{_actualText}'";
        }
    }
}