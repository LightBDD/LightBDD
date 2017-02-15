using System;
using System.Reflection;

namespace LightBDD.XUnit
{
    /// <summary>
    /// XUnit ExceptionExtensions copy (https://github.com/xunit/xunit/blob/master/src/common/ExceptionExtensions.cs)
    /// </summary>
    internal static class ExceptionExtensions
    {
        public static Exception Unwrap(this Exception ex)
        {
            while (true)
            {
                var tiex = ex as TargetInvocationException;
                if (tiex == null)
                    return ex;

                ex = tiex.InnerException;
            }
        }
    }
}