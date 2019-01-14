using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class EveryItemExpectation<TValue> : Expectation<IEnumerable<TValue>>
    {
        private readonly IExpectation<TValue> _itemExpectation;

        public EveryItemExpectation(IExpectation<TValue> itemExpectation)
        {
            _itemExpectation = itemExpectation;
        }

        public override ExpectationResult Verify(IEnumerable<TValue> collection, IValueFormattingService formattingService)
        {
            List<string> errors = new List<string>();
            int i = 0;
            foreach (var item in collection ?? Enumerable.Empty<TValue>())
            {
                var result = _itemExpectation.Verify(item, formattingService);
                if (!result)
                    errors.Add($"[{i}]: {result.Message}");
                ++i;
            }
            if (collection != null && errors.Count == 0)
                return ExpectationResult.Success;
            return FormatFailure(formattingService, $"got: '{formattingService.FormatValue(collection)}'", errors);
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return "every item " + _itemExpectation.Format(formattingService);
        }
    }
}