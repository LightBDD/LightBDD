using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class TabularParameterRow : ITabularParameterRow
    {
        public TabularParameterRow(int rowId, IEnumerable<string> values)
            : this(
                rowId,
                TableRowType.Matching, values.Select(x => new ValueResult(x, x, ParameterVerificationStatus.NotApplicable, null)))
        {
        }

        public TabularParameterRow(int rowId, TableRowType type, IEnumerable<IValueResult> values)
        {
            Type = type;
            Values = values.ToArray();
            VerificationStatus = CollectVerificationStatus();
            Exception = CaptureException(rowId);
        }

        private Exception CaptureException(int rowId)
        {
            var errors = Values
                .Where(t => t.Exception != null)
                .Select(t => $"[{rowId}].{t.Exception.Message}")
                .ToArray();

            return errors.Any()
                ? new InvalidOperationException(string.Join("\n", errors))
                : null;
        }

        private ParameterVerificationStatus CollectVerificationStatus()
        {
            return Values.Any()
                ? Values.Max(v => MapToRowStatus(v.VerificationStatus))
                : ParameterVerificationStatus.NotApplicable;
        }

        private ParameterVerificationStatus MapToRowStatus(ParameterVerificationStatus status)
        {
            return status >= ParameterVerificationStatus.Failure
                ? ParameterVerificationStatus.Failure
                : status;
        }

        public TableRowType Type { get; }
        public IReadOnlyList<IValueResult> Values { get; }
        public Exception Exception { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
    }
}