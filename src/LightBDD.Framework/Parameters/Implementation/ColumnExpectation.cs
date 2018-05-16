using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class ColumnExpectation<T> : Expectation<object>
    {
        private readonly IExpectation<T> _expectation;

        public ColumnExpectation(IExpectation<T> expectation)
        {
            _expectation = expectation;
        }

        public override ExpectationResult Verify(object value, IValueFormattingService formattingService)
        {
            return _expectation.Verify((T)value, formattingService);
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return _expectation.Format(formattingService);
        }
    }
}