using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class TabularParameterResult : ITabularParameterResult
    {
        public TabularParameterResult(IEnumerable<ITabularParameterColumn> columns,IEnumerable<ITabularParameterRow> rows, ParameterVerificationStatus verificationStatus)
            : this(columns, rows)
        {
            VerificationStatus = verificationStatus;
        }

        public TabularParameterResult(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows)
        {
            Columns = columns.ToArray();
            Rows = rows.ToArray();
            VerificationStatus = Rows.Any() ? Rows.Max(x => x.VerificationStatus) : ParameterVerificationStatus.NotApplicable;
            Exception = CollectException();
        }

        private Exception CollectException()
        {
            var message = string.Join("\n", Rows.Where(x => x.Exception != null).Select(x => x.Exception.Message));
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