using System.Diagnostics;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    [DebuggerStepThrough]
    internal class CastExpectation<TDerived, TBase> : Expectation<TBase> where TDerived : TBase
    {
        private readonly IExpectation<TDerived> _expectation;

        public CastExpectation(IExpectation<TDerived> expectation)
        {
            _expectation = expectation;
        }

        public override ExpectationResult Verify(TBase value, IValueFormattingService formattingService)
        {
            if (value == null || value is TDerived)
                return _expectation.Verify((TDerived)value, formattingService);

            return ExpectationResult.Failure($"value of type '{value.GetType().Name}' cannot be cast to '{typeof(TDerived).Name}'");
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return _expectation.Format(formattingService);
        }
    }
}