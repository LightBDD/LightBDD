using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LightBDD.Framework.Parameters.Implementation
{
    [DebuggerStepThrough]
    internal static class TableColumnProvider
    {
        public static IEnumerable<ColumnInfo> InferColumns<TRow>(TRow[] rows, bool addLengthToCollections = false)
        {
            var typeInfo = typeof(TRow).GetTypeInfo();

            if (IsObject(typeInfo))
                typeInfo = TrySpecifyType(rows);

            if (IsSimpleType(typeInfo))
                return AsSimpleColumn();
            if (IsExpando(typeInfo))
                return AsExpandoColumns(rows);
            if (IsCollection(typeInfo))
                return AsCollection(rows, addLengthToCollections);

            return AsPoco(typeInfo.AsType());
        }

        private static TypeInfo TrySpecifyType<TRow>(TRow[] rows)
        {
            Type type = null;
            foreach (var row in rows)
            {
                var rowType = row?.GetType();
                if (type != null && type != rowType)
                    return typeof(object).GetTypeInfo();
                type = rowType;
            }

            return (type ?? typeof(object)).GetTypeInfo();
        }

        private static bool IsObject(TypeInfo typeInfo)
        {
            return typeInfo.AsType() == typeof(object);
        }

        private static IEnumerable<ColumnInfo> AsPoco(Type type)
        {
            return GetProperties(type)
                .Select(property => new ColumnInfo(property.Name, r => r != null ? ColumnValue.From(property.GetValue(r)) : ColumnValue.None))
                .Concat(GetFields(type)
                    .Select(field => new ColumnInfo(field.Name, r => r != null ? ColumnValue.From(field.GetValue(r)) : ColumnValue.None)))
                .OrderBy(x => x.Name);
        }

        private static IEnumerable<ColumnInfo> AsCollection<TRow>(TRow[] rows, bool addLengthToCollections)
        {
            if (addLengthToCollections)
                yield return new ColumnInfo("Length", row => row != null ? ColumnValue.From(((IList)row).Count) : ColumnValue.None);

            var totalColumns = rows.Cast<IList>().Aggregate(0, (max, col) => Math.Max(max, col.Count));

            for (int i = 0; i < totalColumns; ++i)
            {
                var index = i;
                yield return new ColumnInfo($"[{i}]", row =>
                {
                    var collection = (IList)row;
                    return collection.Count > index ? ColumnValue.From(collection[index]) : ColumnValue.None;
                });
            }
        }

        private static bool IsCollection(TypeInfo typeInfo)
        {
            return typeof(IList).GetTypeInfo().IsAssignableFrom(typeInfo);
        }

        private static IOrderedEnumerable<ColumnInfo> AsExpandoColumns<TRow>(IEnumerable<TRow> rows)
        {
            return rows.Cast<IDictionary<string, object>>()
                .SelectMany(r => r.Keys)
                .Distinct()
                .Select(name => new ColumnInfo(name, row => ((IDictionary<string, object>)row).TryGetValue(name, out var value) ? ColumnValue.From(value) : ColumnValue.None))
                .OrderBy(x => x.Name);
        }

        private static IEnumerable<ColumnInfo> AsSimpleColumn()
        {
            return new[] { new ColumnInfo("Item", ColumnValue.From) };
        }

        private static bool IsExpando(TypeInfo typeInfo)
        {
            return typeof(IDictionary<string, object>).GetTypeInfo().IsAssignableFrom(typeInfo);
        }

        private static bool IsSimpleType(TypeInfo typeInfo)
        {
            return typeInfo.IsPrimitive
                   || typeof(string).GetTypeInfo().IsAssignableFrom(typeInfo)
                   || typeof(object) == typeInfo.AsType();
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