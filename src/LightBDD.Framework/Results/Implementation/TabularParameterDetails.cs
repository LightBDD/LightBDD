using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Tabular;

namespace LightBDD.Framework.Results.Implementation
{
    [DebuggerStepThrough]
    internal class TabularParameterDetails : ITabularParameterDetails
    {
        public TabularParameterDetails(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows, ParameterVerificationStatus verificationStatus, Exception tableException = null)
            : this(columns, rows, tableException)
        {
            VerificationStatus = verificationStatus;
        }

        public TabularParameterDetails(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows, Exception tableException = null)
        {
            Columns = columns.ToArray();
            Rows = rows.ToArray();
            VerificationStatus = Rows.Any() ? Rows.Max(x => x.VerificationStatus) : ParameterVerificationStatus.NotApplicable;
            VerificationMessage = CollectMessages(tableException);
        }

        private string CollectMessages(Exception tableException)
        {
            var messages = Enumerable.Repeat(tableException?.Message, 1)
                .Concat(Rows.Select(x => x.VerificationMessage))
                .Where(msg => !string.IsNullOrWhiteSpace(msg));

            var message = string.Join("\n", messages);
            return string.IsNullOrWhiteSpace(message)
                ? null
                : message;
        }

        public string VerificationMessage { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public IReadOnlyList<ITabularParameterColumn> Columns { get; }
        public IReadOnlyList<ITabularParameterRow> Rows { get; }
    }
}