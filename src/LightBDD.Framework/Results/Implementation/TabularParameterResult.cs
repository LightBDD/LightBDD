using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class TabularParameterResult : ITabularParameterResult
    {
        public TabularParameterResult(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows)
        {
            Columns = columns.ToArray();
            Rows = rows.ToArray();
        }

        public Exception Exception { get; } = null;
        public ParameterVerificationStatus VerificationStatus { get; } = ParameterVerificationStatus.NotApplicable;
        public IEnumerable<ITabularParameterColumn> Columns { get; }
        public IEnumerable<ITabularParameterRow> Rows { get; }
    }
}