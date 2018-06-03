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

        public TabularParameterRow(int rowId, TableRowType type, IEnumerable<IValueResult> values, Exception rowException = null)
        {
            Type = type;
            Values = values.ToArray();
            VerificationStatus = CollectVerificationStatus();
            Message = CaptureMessage(rowException, rowId);
        }

        private string CaptureMessage(Exception rowException, int rowId)
        {
            var errors = new List<string>();

            if (rowException != null)
                errors.Add($"[{rowId}]: Failed to retrieve row: {rowException.Message}");

            errors.AddRange(Values
                .Where(t => !string.IsNullOrWhiteSpace(t.Message))
                .Select(t => $"[{rowId}].{t.Message}"));

            return errors.Any()
                ? string.Join("\n", errors)
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
        public string Message { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
    }
}