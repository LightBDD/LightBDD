using LightBDD.Core.Formatting.Values;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightBDD.Framework.Expectations.Implementation
{
    [DebuggerStepThrough]
    internal class OrExpectation<T> : Expectation<T>
    {
        private readonly string _prefix;
        private readonly IExpectation<T>[] _expectations;

        public OrExpectation(string prefix, params IExpectation<T>[] expectations)
        {
            _prefix = prefix;
            _expectations = expectations;
        }

        public override ExpectationResult Verify(T value, IValueFormattingService formattingService)
        {
            var details = new List<string>();
            foreach (var expectation in _expectations)
            {
                var result = expectation.Verify(value, formattingService);
                if (result)
                    return ExpectationResult.Success;
                details.Add(result.Message);
            }
            return FormatFailure(formattingService, $"got: '{formattingService.FormatValue(value)}'", details);
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return $"{_prefix}({string.Join(" or ", _expectations.Select(x => x.Format(formattingService)))})";
        }
    }
}