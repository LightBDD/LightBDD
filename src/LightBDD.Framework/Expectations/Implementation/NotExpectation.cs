using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class NotExpectation<T> : IExpectation<T>
    {
        private readonly IExpectation<T> _expectation;

        public NotExpectation(IExpectation<T> expectation)
        {
            _expectation = expectation;
        }

        public string Description => "not " + _expectation.Description;
        public bool IsValid(T value)
        {
            return !_expectation.IsValid(value);
        }

        public string Format(IValueFormattingService formattingService)
        {
            return "not " + _expectation.Format(formattingService);
        }
    }
}