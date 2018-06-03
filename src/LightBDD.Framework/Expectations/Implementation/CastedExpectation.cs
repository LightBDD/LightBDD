using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class CastedExpectation<T, TBase> : Expectation<TBase> where T : TBase
    {
        private readonly IExpectation<T> _expectation;

        public CastedExpectation(IExpectation<T> expectation)
        {
            _expectation = expectation;
        }

        public override ExpectationResult Verify(TBase value, IValueFormattingService formattingService)
        {
            if (value == null || value is T)
                return _expectation.Verify((T)value, formattingService);

            return ExpectationResult.Failure($"value of type '{value.GetType().Name}' cannot be casted to '{typeof(T).Name}'");
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return _expectation.Format(formattingService);
        }
    }
}