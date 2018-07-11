using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LightBDD.Framework.Parameters.Implementation
{
    [DebuggerStepThrough]
    internal static class Reflector
    {
        public static string GetMemberName<T>(Expression<T> columnExpression)
        {
            if (columnExpression.Body is MemberExpression memberExpression)
                return memberExpression.Member.Name;
            throw new InvalidOperationException($"Expected {nameof(columnExpression)} to be member expression, got: {columnExpression}");
        }
    }
}