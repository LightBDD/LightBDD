using LightBDD.Core.Formatting.Values;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class AndExpectation<T> : Expectation<T>
    {
        private readonly string _prefix;
        private readonly IExpectation<T>[] _expectations;

        public AndExpectation(string prefix, params IExpectation<T>[] expectations)
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
                if (!result)
                    details.Add(result.Message);
            }

            return !details.Any()
                ? ExpectationResult.Success
                : FormatFailure(formattingService, $"got: '{formattingService.FormatValue(value)}'", details);
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return $"{_prefix}({string.Join(" and ", _expectations.Select(x => x.Format(formattingService)))})";
        }
    }
}