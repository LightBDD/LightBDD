using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Parameters.Implementation;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing verifiable tabular step parameter.
    /// When used in step methods, the tabular representation of the parameter will be rendered in reports and progress notification, including verification details.<br/>
    /// The table rows and column values will be verified independently, and included in the reports, where any unsuccessful verification will make step to fail.
    ///
    /// Please see <see cref="VerifiableTable"/> type to learn how to create tables effectively.
    /// </summary>
    /// <typeparam name="TRow">Row type.</typeparam>
    [DebuggerStepThrough]
    public class VerifiableTable<TRow> : IComplexParameter, ISelfFormattable
    {
        internal IValueFormattingService _formattingService = ValueFormattingServices.Current;
        private TabularParameterDetails _details;
        /// <summary>
        /// Returns list of actual rows, or null if actual rows were not provided.
        /// </summary>
        public IReadOnlyList<TRow> ActualRows { get; private set; }
        /// <summary>
        /// Returns list of column definitions.
        /// </summary>
        public IReadOnlyList<VerifiableTableColumn> Columns { get; }
        IParameterDetails IComplexParameter.Details => Details;
        /// <summary>
        /// Returns table verification details.
        /// </summary>
        public ITabularParameterDetails Details => GetResultLazily();

        /// <summary>
        /// Constructor creating verifiable table with specified <paramref name="columns"/>.
        /// </summary>
        /// <param name="columns">Table columns.</param>
        public VerifiableTable(IEnumerable<VerifiableTableColumn> columns)
        {
            Columns = columns.OrderByDescending(x => x.IsKey).ToArray();
        }

        /// <summary>
        /// Sets the actual rows specified by <paramref name="actualRows"/> parameter and verifies them against expectations.
        /// </summary>
        /// <param name="actualRows">Actual rows.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualRows"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ActualRows"/> collection has been already set.</exception>
        public VerifiableTable<TRow> SetActual(IEnumerable<TRow> actualRows)
        {
            if (actualRows == null)
                throw new ArgumentNullException(nameof(actualRows));

            EnsureActualNotSet();
            return SetMatchedActual(MatchRows(actualRows.ToArray()));
        }

        /// <summary>
        /// Sets the actual rows specified by <paramref name="actualRowsProvider"/> parameter and verifies them against <see cref="ExpectedRows"/> collection.<br/>
        /// If evaluation of <paramref name="actualRowsProvider"/> throws, the exception will be included in the report, but won't be propagated out of this method.
        /// </summary>
        /// <param name="actualRowsProvider">Actual rows provider.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualRowsProvider"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ActualRows"/> collection has been already set.</exception>
        public async Task<VerifiableTable<TRow>> SetActualAsync(Func<Task<IEnumerable<TRow>>> actualRowsProvider)
        {
            if (actualRowsProvider == null)
                throw new ArgumentNullException(nameof(actualRowsProvider));

            EnsureActualNotSet();
            IEnumerable<TRow> actualCollection;

            try
            {
                actualCollection = await actualRowsProvider();
            }
            catch (Exception ex)
            {
                _details = new TabularParameterDetails(
                    GetColumns(),
                    GetExpectedRows(),
                    ParameterVerificationStatus.Exception,
                    new InvalidOperationException($"Failed to retrieve rows: {ex.Message}", ex));
                ActualRows = new TRow[0];
                return this;
            }
            return SetActual(actualCollection);
        }

        internal void EnsureActualNotSet()
        {
            if (ActualRows != null)
                throw new InvalidOperationException("Actual rows have been already specified");
        }

        internal VerifiableTable<TRow> SetMatchedActual(IEnumerable<RowMatch> results)
        {
            var matches = results.ToArray();
            _details = new TabularParameterDetails(GetColumns(), GetRows(matches));
            ActualRows = matches
                .Where(x => x.Type != TableRowType.Missing)
                .Select(x => x.Actual.Value)
                .ToArray();
            return this;
        }

        private IEnumerable<ITabularParameterRow> GetRows(IEnumerable<RowMatch> results)
        {
            return results.Select(GetRow);
        }

        private TabularParameterRow GetRow(RowMatch row, int index)
        {
            var values = Columns.Select(c =>
            {
                var expected = row.Expected != null ? c.GetValue(row.Expected) : ColumnValue.None;
                var actual = row.Actual.Exception == null && row.Actual.Value != null ? c.GetValue(row.Actual.Value) : ColumnValue.None;
                var result = VerifyExpectation(expected, actual, c.Expectation);
                return GetValueResult(expected, actual, result, c);
            });
            return new TabularParameterRow(index, row.Type, values, row.Actual.Exception);
        }

        internal virtual ValueResult GetValueResult(ColumnValue expected, ColumnValue actual, ExpectationResult result, VerifiableTableColumn column)
        {
            return new ValueResult(
                column.Expectation(null).Format(_formattingService),
                _formattingService.FormatValue(actual),
                result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                result ? null : $"{column.Name}: {result.Message}"
            );
        }

        internal virtual ExpectationResult VerifyExpectation(ColumnValue expected, ColumnValue actual, Func<object, IExpectation<object>> columnExpectation)
        {
            return columnExpectation.Invoke(expected.Value).Verify(actual.Value, _formattingService);
        }

        private IEnumerable<ITabularParameterColumn> GetColumns()
        {
            return Columns.Select(x => new TabularParameterColumn(x.Name, x.IsKey));
        }

        void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        private ITabularParameterDetails GetResultLazily()
        {
            if (_details != null)
                return _details;
            return _details = new TabularParameterDetails(GetColumns(), GetExpectedRows(), ParameterVerificationStatus.NotProvided);
        }

        internal virtual IEnumerable<ITabularParameterRow> GetExpectedRows()
        {
            var columns = Columns
                .Select(x => x.Expectation(null))
                .Select(x => new ValueResult(x.Format(_formattingService), string.Empty, ParameterVerificationStatus.NotApplicable, string.Empty))
                .ToArray();

            return new[]
            {
                new TabularParameterRow(0, TableRowType.Missing, columns)
            };
        }

        /// <summary>
        /// Returns inline representation of table.
        /// </summary>
        public string Format(IValueFormattingService formattingService)
        {
            return "<table>";
        }

        [DebuggerStepThrough]
        internal struct RowMatch
        {
            public TableRowType Type { get; }
            public TRow Expected { get; }
            public ActualRowValue Actual { get; }

            public RowMatch(TableRowType type, TRow expected, ActualRowValue actual)
            {
                Type = type;
                Expected = expected;
                Actual = actual;
            }
        }

        internal struct ActualRowValue
        {
            public ActualRowValue(TRow value)
            {
                Value = value;
                Exception = null;
            }

            public ActualRowValue(Exception exception)
            {
                Value = default(TRow);
                Exception = exception;
            }

            public TRow Value { get; }
            public Exception Exception { get; }

            public static implicit operator ActualRowValue(TRow row) => new ActualRowValue(row);
        }

        internal virtual IEnumerable<RowMatch> MatchRows(IReadOnlyList<TRow> actual)
        {
            return actual.Select(a => new RowMatch(TableRowType.Matching, default(TRow), a));
        }
    }

    /// <summary>
    /// Table extensions allowing to create <see cref="VerifiableTable{TRow}"/> from collections.
    /// </summary>
    [DebuggerStepThrough]
    public static class VerifiableTable
    {
        public static VerifiableTable<T> Create<T>(Action<IVerifiableTableBuilder<T>> tableDefinitionBuilder)
        {
            var builder = new VerifiableTableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build();
        }
    }
}