using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type allowing to validate collection content against specified column expectations.
    /// </summary>
    /// <typeparam name="TRow">Row type.</typeparam>
    public class TableValidator<TRow> : VerifiableTable<TRow>
    {
        /// <summary>
        /// Constructor allowing to create validator instance.
        /// </summary>
        /// <param name="columns">Columns defintions.</param>
        protected internal TableValidator(IEnumerable<VerifiableTableColumn> columns)
            : base(columns)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<ITabularParameterRow> GetExpectedRowResults()
        {
            var columns = Columns
                .Select(c => new ValueResult(c.Expectation.Invoke(null).Format(FormattingService), ColumnValue.None.Format(FormattingService), ParameterVerificationStatus.NotProvided, null/*$"{c.Name}: Value not provided"*/))
                .ToArray();

            return new[]
            {
                new TabularParameterRow(0, TableRowType.Missing, columns)
            };
        }

        /// <inheritdoc />
        protected override IValueResult ToColumnValueResult(VerifiableTableColumn column, ColumnValue expected, ColumnValue actual)
        {
            var result = column.Expectation.Invoke(null).Verify(actual.Value, FormattingService);

            return new ValueResult(
                column.Expectation(null).Format(FormattingService),
                FormattingService.FormatValue(actual),
                result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                result ? null : $"{column.Name}: {result.Message}"
            );
        }

        /// <inheritdoc />
        protected override IEnumerable<RowData> MatchRows(IReadOnlyList<TRow> actual)
        {
            return actual.Select(a => new RowData(TableRowType.Matching, default, a));
        }
    }
}