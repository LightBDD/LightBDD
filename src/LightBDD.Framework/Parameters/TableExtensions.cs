using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters
{
    public static class TableExtensions
    {
        public static Table<T> AsTable<T>(this IEnumerable<T> items)
        {
            var rows = items.ToArray();
            return new Table<T>(rows, InferColumns(rows, (name, isKey, getValue) => new TableColumn<T>(name, isKey, getValue)));
        }

        public static VerifiableTable<T> AsVerifiableTable<T>(this IEnumerable<T> items)
        {
            var rows = items.ToArray();

            VerifiableTableColumn<T> CreateColumn(string name, bool isKey, Func<T, ColumnValue> getValue)
            {
                return new VerifiableTableColumn<T>(name, isKey, getValue,
                    (e, a, f) => Expect.To.Equal(getValue(e).Value).Verify(getValue(a), f));
            }

            return new VerifiableTable<T>(rows, InferColumns(rows, CreateColumn, true));
        }

        private static IEnumerable<TColumn> InferColumns<TRow, TColumn>(
            TRow[] rows,
            Func<string, bool, Func<TRow, ColumnValue>, TColumn> createColumn,
            bool addLengthToCollections = false) where TColumn : TableColumn<TRow>
        {
            var columns = new List<TColumn>();
            var typeInfo = typeof(TRow).GetTypeInfo();

            if (typeInfo.IsPrimitive
                || typeof(string).GetTypeInfo().IsAssignableFrom(typeInfo)
                || typeof(object) == typeof(TRow))
            {
                columns.Add(createColumn("Item", false, x => ColumnValue.From(x)));
            }
            else if (typeof(IDictionary<string, object>).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                columns.AddRange(
                    rows.Cast<IDictionary<string, object>>()
                        .SelectMany(r => r.Keys)
                        .Distinct()
                        .Select(name => createColumn(name, false, row => ((IDictionary<string, object>)row).TryGetValue(name, out var value) ? ColumnValue.From(value) : ColumnValue.None))
                        .OrderBy(x => x.Name)
                    );
            }
            else if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                if (addLengthToCollections)
                    columns.Add(createColumn("Length", false, row => row != null ? ColumnValue.From(((IList)row).Count) : ColumnValue.None));
                columns.AddRange(
                    Enumerable.Range(0, rows.Cast<IList>().Aggregate(0, (max, col) => Math.Max(max, col.Count))).Select(i => createColumn($"[{i}]", false, row =>
                    {
                        var collection = (IList)row;
                        return collection.Count > i ? ColumnValue.From(collection[i]) : ColumnValue.None;
                    })));
            }
            else
            {
                columns.AddRange(
                    GetProperties(typeof(TRow)).Select(property => createColumn(property.Name, false, r => r != null ? ColumnValue.From(property.GetValue(r)) : ColumnValue.None))
                    .Concat(GetFields(typeof(TRow)).Select(field => createColumn(field.Name, false, r => r != null ? ColumnValue.From(field.GetValue(r)) : ColumnValue.None)))
                    .OrderBy(x => x.Name));
            }

            return columns;
        }

        private static IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetRuntimeFields().Where(x => x.IsPublic && !x.IsStatic);
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetRuntimeProperties().Where(x => x.CanRead && x.GetMethod.IsPublic && !x.GetMethod.IsStatic);
        }
    }
}