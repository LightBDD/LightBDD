using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    [DebuggerStepThrough]
    internal class EquivalentCollection<T> : Expectation<IEnumerable<T>>
    {
        private readonly T[] _expected;

        public EquivalentCollection(IEnumerable<T> expected)
        {
            _expected = expected.ToArray();
        }

        public override ExpectationResult Verify(IEnumerable<T> collection, IValueFormattingService formattingService)
        {
            if (collection == null)
                return FormatFailure(formattingService, $"got: '{formattingService.FormatValue(null)}'");

            var details = new List<string>();
            var actual = collection.ToArray();
            var remainingActual = actual.ToList();
            if (_expected.Length != actual.Length)
                details.Add($"expected collection of {_expected.Length} item(s), but got one of {actual.Length} item(s)");

            foreach (var value in _expected)
            {
                var index = remainingActual.IndexOf(value);
                if (index >= 0)
                    remainingActual.RemoveAt(index);
                else details.Add($"missing: '{formattingService.FormatValue(value)}'");
            }

            foreach (var value in remainingActual)
                details.Add($"surplus: '{formattingService.FormatValue(value)}'");

            return details.Any()
                ? FormatFailure(formattingService, $"got: '{formattingService.FormatValue(actual)}'", details)
                : ExpectationResult.Success;
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return $"equivalent collection '{formattingService.FormatValue(_expected)}'";
        }
    }
}