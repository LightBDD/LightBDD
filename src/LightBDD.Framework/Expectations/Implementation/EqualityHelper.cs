#nullable enable

using System;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal static class EqualityHelper
    {
        public static bool AreEqual(object? a, object? b)
        {
            if (a is null || b is null || a.GetType() == b.GetType())
                return Equals(a, b);

            if (NumericTypeHelper.IsNumeric(a) && NumericTypeHelper.IsNumeric(b))
                return AreNumericsEqual(a, b);
            return Equals(a, b);
        }

        private static bool AreNumericsEqual(object a, object b)
        {
            try
            {
                if (a is double || b is double)
                    return Equals(Convert.ToDouble(a), Convert.ToDouble(b));
                if (a is float || b is float)
                    return Equals(Convert.ToSingle(a), Convert.ToSingle(b));
                if (a is decimal || b is decimal)
                    return Equals(Convert.ToDecimal(a), Convert.ToDecimal(b));
                if (a is ulong || b is ulong)
                    return Equals(Convert.ToUInt64(a), Convert.ToUInt64(b));
                if (a is long || b is long)
                    return Equals(Convert.ToInt64(a), Convert.ToInt64(b));
                if (a is uint || b is uint)
                    return Equals(Convert.ToUInt32(a), Convert.ToUInt32(b));
                return Equals(Convert.ToInt32(a), Convert.ToInt32(b));
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }
}
