using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class TabularParameterRow : ITabularParameterRow
    {
        public TabularParameterRow(IEnumerable<string> values):this(
            TableRowType.Matching,
            values.Select(x => new ValueResult(x, x, ParameterVerificationStatus.NotApplicable, null)))
        {
        }

        public TabularParameterRow(TableRowType type, IEnumerable<IValueResult> values)
        {
            Type = type;
            Values = values.ToArray();
        }

        public TableRowType Type { get; }
        public IEnumerable<IValueResult> Values { get; }
    }
}