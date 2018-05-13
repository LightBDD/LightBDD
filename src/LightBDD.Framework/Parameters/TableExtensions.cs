using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    public static class TableExtensions
    {
        public static Table<T> AsTable<T>(this IEnumerable<T> items)
        {
            var rows = items.ToArray();
            var columns = TableColumnProvider.InferColumns(rows).Select(i => new TableColumn(i.Name, false, i.GetValue));
            return new Table<T>(rows, columns);
        }

        public static Table<KeyValuePair<TKey, TValue>> AsTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new TableColumn(i.Name, false, 
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value)));
            var columns = new[] { new TableColumn("Key", true, x => ColumnValue.From(((KeyValuePair<TKey, TValue>)x).Key)) }
                .Concat(valueColumns);
            return new Table<KeyValuePair<TKey, TValue>>(rows, columns);
        }

        public static VerifiableTable<T> AsVerifiableTable<T>(this IEnumerable<T> items)
        {
            var rows = items.ToArray();

            var columns = TableColumnProvider.InferColumns(rows, true).Select(i => new VerifiableTableColumn(i.Name, false, i.GetValue, expectedColumnValue => Expect.To.Equal(expectedColumnValue)));
            return new VerifiableTable<T>(rows, columns);
        }

        public static VerifiableTable<KeyValuePair<TKey, TValue>> AsVerifiableTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new VerifiableTableColumn(i.Name, false, 
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value), expectedColumnValue => Expect.To.Equal(expectedColumnValue)));
            var columns = new[] { new VerifiableTableColumn("Key", true, x => ColumnValue.From(((KeyValuePair<TKey, TValue>)x).Key), expectedColumnValue => Expect.To.Equal(expectedColumnValue)) }
                .Concat(valueColumns);
            return new VerifiableTable<KeyValuePair<TKey, TValue>>(rows, columns);
        }
    }
}