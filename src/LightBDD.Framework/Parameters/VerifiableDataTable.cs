using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing verifiable tabular step parameter.
    /// When used in step methods, the tabular representation of the parameter will be rendered in reports and progress notification, including verification details.<br/>
    /// The table rows and column values will be verified independently, and included in the reports, where any unsuccessful verification will make step to fail.
    ///
    /// Please see <see cref="Table"/> type to learn how to create tables effectively.
    /// </summary>
    /// <typeparam name="TRow">Row type.</typeparam>
    public class VerifiableDataTable<TRow> : VerifiableTable<TRow>
    {
        /// <summary>
        /// Returns list of expected rows.
        /// </summary>
        public IReadOnlyList<TRow> ExpectedRows { get; }

        /// <summary>
        /// Constructor creating verifiable table with specified <paramref name="columns"/> and <paramref name="expectedRows"/>.
        /// </summary>
        /// <param name="columns">Table columns.</param>
        /// <param name="expectedRows">Table rows.</param>
        protected internal VerifiableDataTable(IEnumerable<VerifiableTableColumn> columns, IEnumerable<TRow> expectedRows)
            : base(columns)
        {
            ExpectedRows = expectedRows.ToArray();
        }

        /// <summary>
        /// Sets the actual rows by calling <paramref name="actualRowLookup"/> for each expected row and verifies them against <see cref="ExpectedRows"/> collection.<br/>
        /// If evaluation of <paramref name="actualRowLookup"/> throws, the exception will be included in the report, but won't be propagated out of this method.
        /// </summary>
        /// <param name="actualRowLookup">Actual row lookup function that will be called for each expected row.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualRowLookup"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="VerifiableTable{TRow}.ActualRows"/> collection has been already set.</exception>
        public void SetActual(Func<TRow, TRow> actualRowLookup)
        {
            if (actualRowLookup == null)
                throw new ArgumentNullException(nameof(actualRowLookup));

            SetActualRows(() => ExpectedRows.Select(expected => LookupRow(expected, actualRowLookup)));
        }

        /// <summary>
        /// Sets the actual rows by calling <paramref name="actualRowLookup"/> for each expected row and verifies them against <see cref="ExpectedRows"/> collection.<br/>
        /// If evaluation of <paramref name="actualRowLookup"/> throws, the exception will be included in the report, but won't be propagated out of this method.<br/>
        ///
        /// Please note that async <paramref name="actualRowLookup"/> calls will be executed concurrently (<see cref="Task.WhenAll(System.Collections.Generic.IEnumerable{System.Threading.Tasks.Task})"/> is used).
        /// </summary>
        /// <param name="actualRowLookup">Actual row lookup function that will be called for each expected row.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualRowLookup"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="VerifiableTable{TRow}.ActualRows"/> collection has been already set.</exception>
        public async Task SetActualAsync(Func<TRow, Task<TRow>> actualRowLookup)
        {
            if (actualRowLookup == null)
                throw new ArgumentNullException(nameof(actualRowLookup));

            await SetActualRowsAsync(() => LookupRows(actualRowLookup));
        }

        private async Task<IEnumerable<RowData>> LookupRows(Func<TRow, Task<TRow>> actualRowLookup)
        {
            return await Task.WhenAll(ExpectedRows.Select(expected => LookupRow(expected, actualRowLookup)));
        }

        private static RowData LookupRow(TRow expected, Func<TRow, TRow> actualRowLookup)
        {
            return new RowData(TableRowType.Matching, expected, Evaluate(actualRowLookup, expected));
        }

        private async Task<RowData> LookupRow(TRow expected, Func<TRow, Task<TRow>> actualRowLookup)
        {
            return new RowData(TableRowType.Matching, expected, await EvaluateAsync(actualRowLookup, expected));
        }

        private static RowDataActualValue Evaluate(Func<TRow, TRow> provideActualFn, TRow row)
        {
            try
            {
                return provideActualFn(row);
            }
            catch (Exception ex)
            {
                return new RowDataActualValue(ex);
            }
        }

        private async Task<RowDataActualValue> EvaluateAsync(Func<TRow, Task<TRow>> provideActualFn, TRow row)
        {
            try
            {
                return await provideActualFn(row);
            }
            catch (Exception ex)
            {
                return new RowDataActualValue(ex);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<ITabularParameterRow> GetExpectedRowResults()
        {
            return ExpectedRows.Select(ToMissingRow);
        }

        private ITabularParameterRow ToMissingRow(TRow row, int index)
        {
            var values = Columns.Select(c =>
            {
                var expected = c.GetValue(row);
                return new ValueResult(
                    FormattingService.FormatValue(expected),
                    FormattingService.FormatValue(ColumnValue.None),
                    ParameterVerificationStatus.NotProvided,
                    $"{c.Name}: Value not provided"
                );
            });
            return new TabularParameterRow(index, TableRowType.Missing, values);
        }

        /// <inheritdoc />
        protected override IValueResult ToColumnValueResult(VerifiableTableColumn column, ColumnValue expected, ColumnValue actual)
        {
            var result = VerifyExpectation(column, expected, actual);

            return new ValueResult(
                FormattingService.FormatValue(expected),
                FormattingService.FormatValue(actual),
                result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                result ? null : $"{column.Name}: {result.Message}"
            );
        }

        private ExpectationResult VerifyExpectation(VerifiableTableColumn column, ColumnValue expected, ColumnValue actual)
        {
            if (expected.HasValue && !actual.HasValue)
                return ExpectationResult.Failure("missing value");
            if (!expected.HasValue && actual.HasValue)
                return ExpectationResult.Failure("unexpected value");
            return column.Expectation.Invoke(expected.Value).Verify(actual.Value, FormattingService);
        }

        /// <inheritdoc />
        protected override IEnumerable<RowData> MatchRows(IReadOnlyList<TRow> actual)
        {
            if (!Columns.Any(c => c.IsKey))
            {
                return ExpectedRows
                    .Zip(actual, (e, a) => new RowData(TableRowType.Matching, e, a))
                    .Concat(ExpectedRows.Skip(actual.Count).Select(e => new RowData(TableRowType.Missing, e, RowDataActualValue.None)))
                    .Concat(actual.Skip(ExpectedRows.Count).Select(a => new RowData(TableRowType.Surplus, default, a)));
            }

            var result = new List<RowData>(ExpectedRows.Count);
            var keySelector = Columns.Where(x => x.IsKey).Select(x => x.GetValue).ToArray();

            var remaining = actual.Select(x => new KeyValuePair<int[], TRow>(GetHashes(keySelector, x), x)).ToList();
            foreach (var e in ExpectedRows)
            {
                var eHash = GetHashes(keySelector, e);
                var index = remaining.FindIndex(r => r.Key.SequenceEqual(eHash));
                if (index >= 0)
                {
                    result.Add(new RowData(TableRowType.Matching, e, remaining[index].Value));
                    remaining.RemoveAt(index);
                }
                else
                    result.Add(new RowData(TableRowType.Missing, e, RowDataActualValue.None));
            }

            foreach (var r in remaining)
                result.Add(new RowData(TableRowType.Surplus, default, r.Value));

            return result;
        }

        private static int[] GetHashes(Func<object, ColumnValue>[] keySelector, TRow row)
        {
            return keySelector.Select(s => s.Invoke(row).Value?.GetHashCode() ?? 0).ToArray();
        }
    }
}