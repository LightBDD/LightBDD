using System.Diagnostics;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    [DebuggerStepThrough]
    internal class NotExpectation<T> : Expectation<T>
    {
        private readonly IExpectation<T> _expectation;

        public NotExpectation(IExpectation<T> expectation)
        {
            _expectation = expectation;
        }

        public override ExpectationResult Verify(T value, IValueFormattingService formattingService)
        {
            if (!_expectation.Verify(value, formattingService))
                return ExpectationResult.Success;
            return FormatFailure(formattingService, "it was");
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return "not " + _expectation.Format(formattingService);
        }
    }
}