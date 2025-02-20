﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
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
    /// This class is an abstract base that can be extended to provide specialized verifiable tables.
    /// </summary>
    /// <typeparam name="TRow">Row type.</typeparam>
    public abstract class VerifiableTable<TRow> : IComplexParameter, ISelfFormattable
    {
        private TabularParameterDetails _details;
        /// <summary>
        /// Returns verification details.
        /// </summary>
        public ITabularParameterDetails Details => GetResultLazily();
        IParameterDetails IComplexParameter.Details => Details;
        /// <summary>
        /// <see cref="IValueFormattingService"/> instance.
        /// </summary>
        protected IValueFormattingService FormattingService { get; private set; } = ValueFormattingServices.Current;

        /// <summary>
        /// Returns list of actual rows, or null if actual rows were not provided.
        /// </summary>
        public IReadOnlyList<TRow> ActualRows { get; private set; }

        /// <summary>
        /// Returns list of column definitions.
        /// </summary>
        public IReadOnlyList<VerifiableTableColumn> Columns { get; }

        /// <summary>
        /// Constructor creating verifiable table with specified <paramref name="columns"/>.
        /// </summary>
        /// <param name="columns">Table columns.</param>
        protected VerifiableTable(IEnumerable<VerifiableTableColumn> columns)
        {
            Columns = columns.OrderByDescending(x => x.IsKey)
                .DefaultIfEmpty(VerifiableTableColumn.FromColumnInfo(TableColumnProvider.ItemColumn))
                .ToArray();
        }

        /// <summary>
        /// Sets the actual rows specified by <paramref name="actualRows"/> parameter and verifies them against expectations.
        /// </summary>
        /// <param name="actualRows">Actual rows.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualRows"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ActualRows"/> collection has been already set.</exception>
        public void SetActual(IEnumerable<TRow> actualRows)
        {
            if (actualRows == null)
                throw new ArgumentNullException(nameof(actualRows));

            EnsureActualNotSet();
            SetRows(MatchRows(actualRows.ToArray()));
        }

        /// <summary>
        /// Clears actual state allowing to specify it once more.
        /// </summary>
        public void ResetActual()
        {
            ActualRows = null;
            _details = null;
        }

        /// <summary>
        /// Sets the actual rows specified by <paramref name="actualRowsProvider"/> parameter and verifies them against expectations.<br/>
        /// If evaluation of <paramref name="actualRowsProvider"/> throws, the exception will be included in the report, but won't be propagated out of this method.
        /// </summary>
        /// <param name="actualRowsProvider">Actual rows provider.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualRowsProvider"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ActualRows"/> collection has been already set.</exception>
        public async Task SetActualAsync(Func<Task<IEnumerable<TRow>>> actualRowsProvider)
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
                    GetColumnResults(),
                    GetExpectedRowResults(),
                    ParameterVerificationStatus.Exception,
                    new InvalidOperationException($"Failed to retrieve rows: {ex.Message}", ex));
                ActualRows = Array.Empty<TRow>();
                return;
            }
            SetActual(actualCollection);
        }

        /// <summary>
        /// Returns collection of expected rows.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<ITabularParameterRow> GetExpectedRowResults();

        /// <summary>
        /// Ensures that actual values are not set yet.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when actual value is already set.</exception>
        protected void EnsureActualNotSet()
        {
            if (ActualRows != null)
                throw new InvalidOperationException("Actual rows have been already specified");
        }

        /// <summary>
        /// Returns inline representation of table.
        /// </summary>
        public virtual string Format(IValueFormattingService formattingService)
        {
            return "<table>";
        }

        /// <summary>
        /// Sets specified <paramref name="rows"/> as the actual values and generates the details for it.
        /// </summary>
        /// <param name="rows">Rows to set.</param>
        protected void SetRows(IEnumerable<RowData> rows)
        {
            var dataRows = rows.ToArray();
            _details = new TabularParameterDetails(GetColumnResults(), dataRows.Select(ToRowResult));
            ActualRows = dataRows
                .Where(x => x.Type != TableRowType.Missing)
                .Select(x => x.Actual.Value)
                .ToArray();
        }

        /// <summary>
        /// Provides <see cref="IValueResult"/> representation of column value comparison.
        /// </summary>
        /// <param name="column">Column definition.</param>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value</param>
        protected abstract IValueResult ToColumnValueResult(VerifiableTableColumn column, ColumnValue expected, ColumnValue actual);

        /// <summary>
        /// Created collection of <see cref="RowData"/> for the given <paramref name="actual"/> collection.
        /// </summary>
        protected abstract IEnumerable<RowData> MatchRows(IReadOnlyList<TRow> actual);

        /// <summary>
        /// Row data containing expected value, actual value as well as row type.
        /// </summary>
        protected class RowData
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public RowData(TableRowType type, TRow expected, RowDataActualValue actual)
            {
                Type = type;
                Expected = expected;
                Actual = actual;
            }

            /// <summary>
            /// Row type.
            /// </summary>
            public TableRowType Type { get; }
            /// <summary>
            /// Expected row value (can be null).
            /// </summary>
            public TRow Expected { get; }
            /// <summary>
            /// Actual row value.
            /// </summary>
            public RowDataActualValue Actual { get; }
        }

        /// <summary>
        /// Type representing actual row value that can be row instance or exception.
        /// </summary>
        protected struct RowDataActualValue
        {
            /// <summary>
            /// Value representing no actual value.
            /// </summary>
            public static readonly RowDataActualValue None = new(default(TRow));

            /// <summary>
            /// Constructor setting row value.
            /// </summary>
            public RowDataActualValue(TRow value)
            {
                Value = value;
                Exception = null;
            }

            /// <summary>
            /// Constructor setting exception.
            /// </summary>
            public RowDataActualValue(Exception exception)
            {
                Value = default;
                Exception = exception;
            }

            /// <summary>
            /// Value.
            /// </summary>
            public TRow Value { get; }
            /// <summary>
            /// Exception.
            /// </summary>
            public Exception Exception { get; }

            /// <summary>
            /// Implicit operator converting row to actual row value.
            /// </summary>
            /// <param name="row"></param>
            public static implicit operator RowDataActualValue(TRow row) => new(row);
        }

        void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            FormattingService = formattingService;
        }

        private ITabularParameterDetails GetResultLazily()
        {
            if (_details != null)
                return _details;
            return _details = new TabularParameterDetails(GetColumnResults(), GetExpectedRowResults(), ParameterVerificationStatus.NotProvided);
        }

        private IEnumerable<ITabularParameterColumn> GetColumnResults()
        {
            return Columns.Select(x => new TabularParameterColumn(x.Name, x.IsKey));
        }

        private ITabularParameterRow ToRowResult(RowData row, int index)
        {
            var values = Columns.Select(c =>
            {
                var expected = row.Expected != null ? c.GetValue(row.Expected) : ColumnValue.None;
                var actual = row.Actual.Exception == null && row.Actual.Value != null ? c.GetValue(row.Actual.Value) : ColumnValue.None;
                return ToColumnValueResult(c, expected, actual);
            });
            return new TabularParameterRow(index, row.Type, values, row.Actual.Exception);
        }
    }
}