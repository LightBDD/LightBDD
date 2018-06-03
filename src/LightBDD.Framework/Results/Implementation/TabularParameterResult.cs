using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class TabularParameterResult : ITabularParameterResult
    {
        public TabularParameterResult(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows, ParameterVerificationStatus verificationStatus, Exception tableException = null)
            : this(columns, rows, tableException)
        {
            VerificationStatus = verificationStatus;
        }

        public TabularParameterResult(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows, Exception tableException = null)
        {
            Columns = columns.ToArray();
            Rows = rows.ToArray();
            VerificationStatus = Rows.Any() ? Rows.Max(x => x.VerificationStatus) : ParameterVerificationStatus.NotApplicable;
            Message = CollectMessages(tableException);
        }

        private string CollectMessages(Exception tableException)
        {
            var messages = Enumerable.Repeat(tableException?.Message, 1)
                .Concat(Rows.Select(x => x.Message))
                .Where(msg => !string.IsNullOrWhiteSpace(msg));

            var message = string.Join("\n", messages);
            return string.IsNullOrWhiteSpace(message)
                ? null
                : message;
        }

        public string Message { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public IReadOnlyList<ITabularParameterColumn> Columns { get; }
        public IReadOnlyList<ITabularParameterRow> Rows { get; }
    }
}