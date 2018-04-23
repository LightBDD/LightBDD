using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class AndExpectation<T> : Expectation<T>
    {
        private readonly Expectation<T> _left;
        private readonly Expectation<T> _right;

        public AndExpectation(Expectation<T> left, Expectation<T> right)
        {
            _left = left;
            _right = right;
        }

        public override ExpectationResult Verify(T value, IValueFormattingService formattingService)
        {
            var lvalue = _left.Verify(value, formattingService);
            var rvalue = _right.Verify(value, formattingService);
            if (lvalue && rvalue)
                return ExpectationResult.Success;

            var details = new List<string>();
            if (!lvalue)
                details.Add("left: " + lvalue.Message);
            if (!rvalue)
                details.Add("right: " + rvalue.Message);

            return FormatFailure(formattingService, $"got: '{formattingService.FormatValue(value)}'", details);
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return $"({_left.Format(formattingService)} and {_right.Format(formattingService)})";
        }
    }
}