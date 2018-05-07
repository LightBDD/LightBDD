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

            var columns = new List<TableColumn<T>>();
            var typeInfo = typeof(T).GetTypeInfo();
            if (typeInfo.IsPrimitive || typeof(string).GetTypeInfo().IsAssignableFrom(typeInfo))
                columns.Add(new TableColumn<T>("Item", false, x => ColumnValue.From(x)));
            else if (typeof(IDictionary<string, object>).GetTypeInfo().IsAssignableFrom(typeInfo))
                columns.AddRange(
                    rows.Cast<IDictionary<string, object>>()
                    .SelectMany(r => r.Keys)
                    .Distinct()
                    .Select(name => new TableColumn<T>(name, false, row => ((IDictionary<string, object>)row).TryGetValue(name, out var value) ? ColumnValue.From(value) : ColumnValue.None)));
            else if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                columns.AddRange(
                    Enumerable.Range(0, rows.Cast<IList>().Aggregate(0, (max, col) => Math.Max(max, col.Count))).Select(i => new TableColumn<T>($"[{i}]", false, row =>
                              {
                                  var collection = (IList)row;
                                  return collection.Count > i ? ColumnValue.From(collection[i]) : ColumnValue.None;
                              })));
            }
            else
            {
                columns.AddRange(GetProperties(typeof(T)).Where(x => x.CanRead).Select(property => new TableColumn<T>(property.Name, false, r => r != null ? ColumnValue.From(property.GetValue(r)) : ColumnValue.None))
                    .Concat(GetFields(typeof(T)).Where(x => x.IsPublic && !x.IsStatic).Select(field => new TableColumn<T>(field.Name, false, r => r != null ? ColumnValue.From(field.GetValue(r)) : ColumnValue.None))));
            }

            return new Table<T>(rows, columns);
        }

        private static IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetRuntimeFields().Where(x => x.IsPublic && !x.IsStatic);
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetRuntimeProperties().Where(x => x.CanRead && x.GetMethod.IsPublic && !x.GetMethod.IsStatic);
        }

        public static VerifiableTable<KeyValuePair<TKey, TValue>> AsVerifiableTable<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary) where TValue : class
        {
            var columns = new List<VerifiableTableColumn<KeyValuePair<TKey, TValue>>>
            {
                new VerifiableTableColumn<KeyValuePair<TKey, TValue>>("Key", true, r => ColumnValue.From(r.Key),
                    (e, a, f) => Expect.To.Equal(e.Key).Verify(a.Key, f))
            };

            foreach (var property in typeof(TValue).GetTypeInfo().DeclaredProperties)
            {
                columns.Add(new VerifiableTableColumn<KeyValuePair<TKey, TValue>>(property.Name, false, r => r.Value != null ? ColumnValue.From(property.GetValue(r.Value)) : ColumnValue.None,
                    (e, a, f) => Expect.To.Equal(e.Value != null ? property.GetValue(e.Value) : null).Verify(a.Value != null ? property.GetValue(a.Value) : null, f)));
            }
            return new VerifiableTable<KeyValuePair<TKey, TValue>>(dictionary, columns.ToArray());
        }
    }
}