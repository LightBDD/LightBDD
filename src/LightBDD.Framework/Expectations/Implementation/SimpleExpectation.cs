using System;
using System.Diagnostics;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    [DebuggerStepThrough]
    internal class SimpleExpectation<T> : Expectation<T>
    {
        private readonly Func<IValueFormattingService, string> _descriptionFn;
        private readonly Func<T, bool> _predicateFn;

        public SimpleExpectation(Func<IValueFormattingService, string> descriptionFn, Func<T, bool> predicateFn)
        {
            _descriptionFn = descriptionFn;
            _predicateFn = predicateFn;
        }

        public override ExpectationResult Verify(T value, IValueFormattingService formattingService)
        {
            if (_predicateFn(value))
                return ExpectationResult.Success;
            return FormatFailure(formattingService, $"got: '{formattingService.FormatValue(value)}'");
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return _descriptionFn(formattingService);
        }
    }
}