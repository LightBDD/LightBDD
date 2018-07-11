using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    [DebuggerStepThrough]
    internal class EqualCollection<T> : Expectation<IEnumerable<T>>
    {
        private readonly T[] _expected;

        public EqualCollection(IEnumerable<T> expected)
        {
            _expected = expected.ToArray();
        }

        public override ExpectationResult Verify(IEnumerable<T> collection, IValueFormattingService formattingService)
        {
            if (collection == null)
                return FormatFailure(formattingService, $"got: '{formattingService.FormatValue(null)}'");
            var details = new List<string>();
            var i = 0;
            var actual = (collection).ToArray();

            if (_expected.Length != actual.Length)
                details.Add($"expected collection of {_expected.Length} item(s), but got one of {actual.Length} item(s)");

            foreach (var item in actual)
            {
                if (_expected.Length > i)
                {
                    if (!Equals(_expected[i], item))
                        details.Add($"[{i}]: expected: '{formattingService.FormatValue(_expected[i])}', but got: '{formattingService.FormatValue(item)}'");
                }
                else
                    details.Add($"[{i}]: surplus: '{formattingService.FormatValue(item)}'");

                ++i;
            }
            for (; i < _expected.Length; ++i)
                details.Add($"[{i}]: missing: '{formattingService.FormatValue(_expected[i])}'");

            return details.Any()
                ? FormatFailure(formattingService, $"got: '{formattingService.FormatValue(actual)}'", details)
                : ExpectationResult.Success;
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return $"equals collection '{formattingService.FormatValue(_expected)}'";
        }
    }
}