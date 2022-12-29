#nullable enable
using System;

namespace LightBDD.Framework.Expectations.Implementation;

internal static class NumericTypeHelper
{
    public static bool IsNumeric(object o)
    {
        return o is double or float or decimal or byte or sbyte or int or uint or long or ulong or short or ushort;
    }

    public static bool IsNumeric(Type type)
    {
        return type == typeof(double) 
               || type == typeof(float) 
               || type == typeof(decimal) 
               || type == typeof(byte)
               || type == typeof(sbyte)
               || type == typeof(int)
               || type == typeof(uint)
               || type == typeof(long)
               || type == typeof(ulong)
               || type == typeof(short)
               || type == typeof(short);
    }
}