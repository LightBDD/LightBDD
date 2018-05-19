using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    public class VerifiableTable<TRow> : IVerifiableParameter, ISelfFormattable
    {
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        public IReadOnlyList<TRow> Expected { get; }
        public IReadOnlyList<TRow> Actual { get; private set; }
        public IReadOnlyList<VerifiableTableColumn> Columns { get; }

        public VerifiableTable(IEnumerable<TRow> expected, IEnumerable<VerifiableTableColumn> columns)
        {
            Expected = expected.ToArray();
            Columns = columns.OrderByDescending(x => x.IsKey).ToArray();
        }

        public VerifiableTable<TRow> SetActual(IEnumerable<TRow> actual)
        {
            return SetMatchedActual(MatchRows(Expected, actual.ToArray()));
        }

        private IEnumerable<RowMatch> MatchRows(IReadOnlyList<TRow> expected, IReadOnlyList<TRow> actual)
        {
            if (!Columns.Any(c => c.IsKey))
            {
                return expected
                    .Zip(actual, (e, a) => new RowMatch(TableRowType.Matching, e, a))
                    .Concat(expected.Skip(actual.Count).Select(e => new RowMatch(TableRowType.Missing, e, default(TRow))))
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

        public async Task<VerifiableTable<TRow>> SetActualAsync(Func<Task<IEnumerable<TRow>>> actualFn)
        {
            return SetActual(await actualFn());
        }

        public VerifiableTable<TRow> SetActual(Func<TRow, TRow> provideActualFn)
        {
            return SetMatchedActual(Expected.Select(e => new RowMatch(TableRowType.Matching, e, provideActualFn(e))));
        }

        public async Task<VerifiableTable<TRow>> SetActualAsync(Func<TRow, Task<TRow>> provideActualFn)
        {
            var results = await Task.WhenAll(Expected.Select(provideActualFn));
            return SetMatchedActual(Expected.Zip(results, (e, a) => new RowMatch(TableRowType.Matching, e, a)));
        }

        private VerifiableTable<TRow> SetMatchedActual(IEnumerable<RowMatch> results)
        {
            var matches = results.ToArray();
            _result = new TabularParameterResult(GetColumns(), GetRows(matches));
            Actual = matches.Where(x => x.Type != TableRowType.Missing).Select(x => x.Actual).ToArray();
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
                var actual = row.Actual != null ? c.GetValue(row.Actual) : ColumnValue.None;
                var result = VerifyExpectation(expected, actual, c.Expectation);
                return new ValueResult(
                    _formattingService.FormatValue(expected),
                    _formattingService.FormatValue(actual),
                    result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                    result ? null : new InvalidOperationException($"{c.Name}: {result.Message}")
                );
            });
            return new TabularParameterRow(index, row.Type, values);
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
        void IVerifiableParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        IParameterVerificationResult IVerifiableParameter.Result => Result;
        private TabularParameterResult _result;
        public ITabularParameterResult Result => GetResultLazily();

        private ITabularParameterResult GetResultLazily()
        {
            if (_result != null) return _result;
            return _result = new TabularParameterResult(GetColumns(), Expected.Select(ToMissingRow), ParameterVerificationStatus.NotProvided);
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
                    new InvalidOperationException($"{c.Name}: Value not provided")
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
            public TRow Actual { get; }

            public RowMatch(TableRowType type, TRow expected, TRow actual)
            {
                Type = type;
                Expected = expected;
                Actual = actual;
            }
        }
    }
}