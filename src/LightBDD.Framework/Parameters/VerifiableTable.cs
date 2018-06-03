﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    public class VerifiableTable<TRow> : IComplexParameter, ISelfFormattable
    {
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        private TabularParameterDetails _details;
        public IReadOnlyList<TRow> Expected { get; }
        public IReadOnlyList<TRow> Actual { get; private set; }
        public IReadOnlyList<VerifiableTableColumn> Columns { get; }
        IParameterDetails IComplexParameter.Details => Details;
        public ITabularParameterDetails Details => GetResultLazily();

        public VerifiableTable(IEnumerable<TRow> expected, IEnumerable<VerifiableTableColumn> columns)
        {
            Expected = expected.ToArray();
            Columns = columns.OrderByDescending(x => x.IsKey).ToArray();
        }

        public VerifiableTable<TRow> SetActual(IEnumerable<TRow> actualCollection)
        {
            if (actualCollection == null)
                throw new ArgumentNullException(nameof(actualCollection));

            EnsureActualNotSet();
            return SetMatchedActual(MatchRows(Expected, actualCollection.ToArray()));
        }

        public async Task<VerifiableTable<TRow>> SetActualAsync(Func<Task<IEnumerable<TRow>>> actualCollectionFn)
        {
            if (actualCollectionFn == null)
                throw new ArgumentNullException(nameof(actualCollectionFn));

            EnsureActualNotSet();
            IEnumerable<TRow> actualCollection;

            try
            {
                actualCollection = await actualCollectionFn();
            }
            catch (Exception ex)
            {
                _details = new TabularParameterDetails(
                    GetColumns(),
                    Expected.Select(ToMissingRow),
                    ParameterVerificationStatus.Exception,
                    new InvalidOperationException($"Failed to retrieve rows: {ex.Message}", ex));
                Actual = new TRow[0];
                return this;
            }
            return SetActual(actualCollection);
        }

        public VerifiableTable<TRow> SetActual(Func<TRow, TRow> lookupFn)
        {
            if (lookupFn == null)
                throw new ArgumentNullException(nameof(lookupFn));

            EnsureActualNotSet();
            return SetMatchedActual(Expected.Select(e => new RowMatch(TableRowType.Matching, e, Evaluate(lookupFn, e))));
        }

        public async Task<VerifiableTable<TRow>> SetActualAsync(Func<TRow, Task<TRow>> lookupFn)
        {
            if (lookupFn == null)
                throw new ArgumentNullException(nameof(lookupFn));

            EnsureActualNotSet();
            var results = await Task.WhenAll(Expected.Select(e => EvaluateAsync(lookupFn, e)));
            return SetMatchedActual(Expected.Zip(results, (e, a) => new RowMatch(TableRowType.Matching, e, a)));
        }

        private void EnsureActualNotSet()
        {
            if (Actual != null)
                throw new InvalidOperationException("Actual values have been already specified");
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

        private VerifiableTable<TRow> SetMatchedActual(IEnumerable<RowMatch> results)
        {
            var matches = results.ToArray();
            _details = new TabularParameterDetails(GetColumns(), GetRows(matches));
            Actual = matches
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
                return new ValueResult(
                    _formattingService.FormatValue(expected),
                    _formattingService.FormatValue(actual),
                    result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                    result ? null : $"{c.Name}: {result.Message}"
                );
            });
            return new TabularParameterRow(index, row.Type, values, row.Actual.Exception);
        }

        private ExpectationResult VerifyExpectation(ColumnValue expected, ColumnValue actual, Func<object, IExpectation<object>> columnExpectation)
        {
            if (expected.HasValue && !actual.HasValue)
                return ExpectationResult.Failure("missing value");
            if (!expected.HasValue && actual.HasValue)
                return ExpectationResult.Failure("unexpected value");
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
            return _details = new TabularParameterDetails(GetColumns(), Expected.Select(ToMissingRow), ParameterVerificationStatus.NotProvided);
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

        /// <summary>
        /// Returns inline representation of table
        /// </summary>
        public string Format(IValueFormattingService formattingService)
        {
            return "<table>";
        }

        private struct RowMatch
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

        private struct ActualRowValue
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

        private IEnumerable<RowMatch> MatchRows(IReadOnlyList<TRow> expected, IReadOnlyList<TRow> actual)
        {
            if (!Columns.Any(c => c.IsKey))
            {
                return expected
                    .Zip(actual, (e, a) => new RowMatch(TableRowType.Matching, e, a))
                    .Concat(expected.Skip(actual.Count).Select(e => new RowMatch(TableRowType.Missing, e, default(ActualRowValue))))
                    .Concat(actual.Skip(expected.Count).Select(a => new RowMatch(TableRowType.Surplus, default(TRow), a)));
            }

            var result = new List<RowMatch>(expected.Count);

            var keySelector = Columns.Where(x => x.IsKey).Select(x => x.GetValue).ToArray();
            int[] GetHashes(TRow row)
            {
                return keySelector.Select(s => s.Invoke(row).Value?.GetHashCode() ?? 0).ToArray();
            }

            var remaining = actual.Select(x => new KeyValuePair<int[], TRow>(GetHashes(x), x)).ToList();
            foreach (var e in expected)
            {
                var eHash = GetHashes(e);
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
    }
}