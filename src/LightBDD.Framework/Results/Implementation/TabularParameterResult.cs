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
            Exception = CollectExceptions(tableException);
        }

        private Exception CollectExceptions(Exception tableException)
        {
            var exceptions = Enumerable.Repeat(tableException, 1)
                .Concat(Rows.Select(x => x.Exception))
                .Where(exception => exception != null)
                .Select(x => x.Message);

            var message = string.Join("\n", exceptions);
            return string.IsNullOrWhiteSpace(message)
                ? null
                : new InvalidOperationException(message);
        }

        public Exception Exception { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public IReadOnlyList<ITabularParameterColumn> Columns { get; }
        public IReadOnlyList<ITabularParameterRow> Rows { get; }
    }
}