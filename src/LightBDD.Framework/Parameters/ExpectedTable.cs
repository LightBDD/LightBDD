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
    public class ExpectedTable<TRow> : VerifiableTable<TRow>
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
        public ExpectedTable(IEnumerable<VerifiableTableColumn> columns, IEnumerable<TRow> expectedRows) : base(columns)
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
        public ExpectedTable<TRow> SetActual(Func<TRow, TRow> actualRowLookup)
        {
            if (actualRowLookup == null)
                throw new ArgumentNullException(nameof(actualRowLookup));

            EnsureActualNotSet();
            SetMatchedActual(ExpectedRows.Select(e => new RowMatch(TableRowType.Matching, e, Evaluate(actualRowLookup, e))));
            return this;
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
        public async Task<ExpectedTable<TRow>> SetActualAsync(Func<TRow, Task<TRow>> actualRowLookup)
        {
            if (actualRowLookup == null)
                throw new ArgumentNullException(nameof(actualRowLookup));

            EnsureActualNotSet();
            var results = await Task.WhenAll(ExpectedRows.Select(e => EvaluateAsync(actualRowLookup, e)));
            SetMatchedActual(ExpectedRows.Zip(results, (e, a) => new RowMatch(TableRowType.Matching, e, a)));
            return this;
        }
        private static ActualRowValue Evaluate(Func<TRow, TRow> provideActualFn, TRow row)
        {
            try
            {
                return provideActualFn(row);
            }
            catch (Exception ex)
            {
                return new ActualRowValue(ex);
            }
        }

        private async Task<ActualRowValue> EvaluateAsync(Func<TRow, Task<TRow>> provideActualFn, TRow row)
        {
            try
            {
                return await provideActualFn(row);
            }
            catch (Exception ex)
            {
                return new ActualRowValue(ex);
            }
        }

        internal override ExpectationResult VerifyExpectation(ColumnValue expected, ColumnValue actual, Func<object, IExpectation<object>> columnExpectation)
        {
            if (expected.HasValue && !actual.HasValue)
                return ExpectationResult.Failure("missing value");
            if (!expected.HasValue && actual.HasValue)
                return ExpectationResult.Failure("unexpected value");
            return columnExpectation.Invoke(expected.Value).Verify(actual.Value, _formattingService);
        }

        internal override IEnumerable<ITabularParameterRow> GetExpectedRows()
        {
            return ExpectedRows.Select(ToMissingRow);
        }

        private ITabularParameterRow ToMissingRow(TRow row, int index)
        {
            var values = Columns.Select(c =>
            {
                var expected = c.GetValue(row);
                return new ValueResult(
                    _formattingService.FormatValue(expected),
                    _formattingService.FormatValue(ColumnValue.None),
                    ParameterVerificationStatus.NotProvided,
                    $"{c.Name}: Value not provided"
                );
            });
            return new TabularParameterRow(index, TableRowType.Missing, values);
        }


        internal override IEnumerable<RowMatch> MatchRows(IReadOnlyList<TRow> actual)
        {
            if (!Columns.Any(c => c.IsKey))
            {
                return ExpectedRows
                    .Zip(actual, (e, a) => new RowMatch(TableRowType.Matching, e, a))
                    .Concat(ExpectedRows.Skip(actual.Count).Select(e => new RowMatch(TableRowType.Missing, e, default(ActualRowValue))))
                    .Concat(actual.Skip(ExpectedRows.Count).Select(a => new RowMatch(TableRowType.Surplus, default(TRow), a)));
            }

            var result = new List<RowMatch>(ExpectedRows.Count);
            var keySelector = Columns.Where(x => x.IsKey).Select(x => x.GetValue).ToArray();

            var remaining = actual.Select(x => new KeyValuePair<int[], TRow>(GetHashes(keySelector, x), x)).ToList();
            foreach (var e in ExpectedRows)
            {
                var eHash = GetHashes(keySelector, e);
                var index = remaining.FindIndex(r => r.Key.SequenceEqual(eHash));
                if (index >= 0)
                {
                    result.Add(new RowMatch(TableRowType.Matching, e, remaining[index].Value));
                    remaining.RemoveAt(index);
                }
                else
                    result.Add(new RowMatch(TableRowType.Missing, e, default(TRow)));
            }

            foreach (var r in remaining)
                result.Add(new RowMatch(TableRowType.Surplus, default(TRow), r.Value));

            return result;
        }

        internal override ValueResult GetValueResult(ColumnValue expected, ColumnValue actual, ExpectationResult result, VerifiableTableColumn column)
        {
            return new ValueResult(
                _formattingService.FormatValue(expected),
                _formattingService.FormatValue(actual),
                result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                result ? null : $"{column.Name}: {result.Message}"
            );
        }

        private static int[] GetHashes(Func<object, ColumnValue>[] keySelector, TRow row)
        {
            return keySelector.Select(s => s.Invoke(row).Value?.GetHashCode() ?? 0).ToArray();
        }
    }

    /// <summary>
    /// Table extensions allowing to create <see cref="ExpectedTable{TRow}"/> from collections.
    /// </summary>
    [DebuggerStepThrough]
    public static class ExpectedTable
    {
        /// <summary>
        /// Returns <see cref="ExpectedTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will have no key columns (the row verification will be index based).<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static ExpectedTable<T> ToVerifiableTable<T>(this IEnumerable<T> items)
        {
            return CreateFor(items.ToArray());
        }

        /// <summary>
        /// Returns <see cref="ExpectedTable{TRow}"/> defined by <paramref name="tableDefinitionBuilder"/> and rows specified by <paramref name="items"/> collection.<br/>
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="tableDefinitionBuilder">Table definition builder.</param>
        /// <returns>Verifiable table</returns>
        public static ExpectedTable<T> ToVerifiableTable<T>(this IEnumerable<T> items,
            Action<IExpectedTableBuilder<T>> tableDefinitionBuilder)
        {
            var builder = new ExpectedTableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="ExpectedTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will have no key columns (the row verification will be index based).<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static ExpectedTable<T> CreateFor<T>(params T[] items)
        {
            var columns = TableColumnProvider.InferColumns(items, true).Select(VerifiableTableColumn.FromColumnInfo);
            return new ExpectedTable<T>(columns, items);
        }

        /// <summary>
        /// Returns <see cref="ExpectedTable{TRow}"/> defined by <paramref name="tableDefinitionBuilder"/> and rows specified by <paramref name="items"/> collection.<br/>
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="tableDefinitionBuilder">Table definition builder.</param>
        /// <returns>Verifiable table</returns>
        public static ExpectedTable<T> CreateFor<T>(Action<IExpectedTableBuilder<T>> tableDefinitionBuilder,
            params T[] items)
        {
            var builder = new ExpectedTableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="ExpectedTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will use Key column to compare rows.<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static ExpectedTable<KeyValuePair<TKey, TValue>> ToVerifiableTable<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new VerifiableTableColumn(i.Name,
                false,
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value), Expect.To.Equal));
            var columns = new[]
                {
                    new VerifiableTableColumn("Key", true, x => ColumnValue.From(((KeyValuePair<TKey, TValue>) x).Key),
                        Expect.To.Equal)
                }
                .Concat(valueColumns);
            return new ExpectedTable<KeyValuePair<TKey, TValue>>(columns, rows);
        }
    }
}