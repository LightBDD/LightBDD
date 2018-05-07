using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    public class VerifiableTable<TRow> : IVerifiableParameter, ISelfFormattable
    {
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        public IReadOnlyList<TRow> Expected { get; }
        public IReadOnlyList<TRow> Actual { get; private set; }
        public IReadOnlyList<VerifiableTableColumn<TRow>> Columns { get; }

        public VerifiableTable(IEnumerable<TRow> expected, params VerifiableTableColumn<TRow>[] columns)
        {
            Expected = expected.ToArray();
            Columns = columns;
        }

        public VerifiableTable<TRow> SetActual(IEnumerable<TRow> actual)
        {
            return SetMatchedActual(MatchRows(Expected, actual.ToArray()));
        }

        private IEnumerable<RowMatch> MatchRows(IReadOnlyList<TRow> expected, IReadOnlyList<TRow> actual)
        {
            if (!Columns.Any(c => c.IsKey))
                return expected
                    .Zip(actual, (e, a) => new RowMatch(TableRowType.Matching, e, a))
                    .Concat(expected.Skip(actual.Count).Select(e => new RowMatch(TableRowType.Missing, e, default(TRow))))
                    .Concat(actual.Skip(expected.Count).Select(a => new RowMatch(TableRowType.Surplus, default(TRow), a)));

            var result = new List<RowMatch>(expected.Count);

            var keySelector = Columns.Where(x => x.IsKey).Select(x => x.GetValue).ToArray();
            int[] GetHashes(TRow row) => keySelector.Select(s => s.Invoke(row).Value?.GetHashCode() ?? 0).ToArray();

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
            Result = new TabularParameterResult(GetColumns(), GetRows(matches));
            Actual = matches.Where(x => x.Type != TableRowType.Missing).Select(x => x.Actual).ToArray();
            return this;
        }

        private IEnumerable<ITabularParameterRow> GetRows(IEnumerable<RowMatch> results)
        {
            return results.Select(GetRow);
        }

        private TabularParameterRow GetRow(RowMatch row)
        {
            var values = Columns.Select(c =>
            {
                var expected = c.GetValue(row.Expected);
                var actual = c.GetValue(row.Actual);
                var result = c.Verify(row.Expected, row.Actual, _formattingService);
                return new ValueResult(
                    _formattingService.FormatValue(expected),
                    _formattingService.FormatValue(actual),
                    result ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                    result ? null : new InvalidOperationException(result.Message)
                );
            });
            return new TabularParameterRow(row.Type, values);
        }

        private IEnumerable<ITabularParameterColumn> GetColumns()
        {
            return Columns.Select(x => new TabularParameterColumn(x.Name, x.IsKey));
        }
        void IVerifiableParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        public IParameterVerificationResult Result { get; private set; }=new TabularParameterResult(Enumerable.Empty<ITabularParameterColumn>(),Enumerable.Empty<ITabularParameterRow>());

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