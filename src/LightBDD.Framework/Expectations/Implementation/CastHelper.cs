#nullable enable
using System;

namespace LightBDD.Framework.Expectations.Implementation;

internal static class CastHelper
{
    public static bool IsAssignableTo<T>(object? value) => value is T || value is null && IsNullAssignableTo<T>();
    private static bool IsNullAssignableTo<T>() => !typeof(T).IsValueType || (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>));

    public static bool TryConvertWithoutPrecisionLoss<T>(object value, out T o)
    {
        o = (T)Convert.ChangeType(value, typeof(T));
        var castBack = Convert.ChangeType(o, value.GetType());
        return Equals(value, castBack);
    }
}