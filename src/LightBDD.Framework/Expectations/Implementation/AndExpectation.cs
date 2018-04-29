using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class AndExpectation<T> : Expectation<T>
    {
        private readonly IExpectation<T>[] _expectations;

        public AndExpectation(params IExpectation<T>[] expectations)
        {
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
            return $"({string.Join(" and ", _expectations.Select(x => x.Format(formattingService)))})";
        }
    }
}