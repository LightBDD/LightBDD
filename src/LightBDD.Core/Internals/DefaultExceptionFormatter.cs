using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LightBDD.Core.Internals
{
    [DebuggerStepThrough]
    internal static class DefaultExceptionFormatter
    {
        public static string Format(Exception exception)
        {
            var builder = new StringBuilder();

            FormatExceptionDetails(exception, builder);

            foreach (var line in exception.StackTrace.Split('\n').Take(4))
                builder.AppendLine(line.Trim());

            return builder.ToString();
        }

        private static void FormatExceptionDetails(Exception exception, StringBuilder builder, int indent = 0)
        {
            var stringIndent = indent > 0 ? new string('\t', indent) : string.Empty;
            var innerExceptionIndicator = indent > 0 ? "---> " : string.Empty;
            builder.AppendLine($"{stringIndent}{innerExceptionIndicator}{exception.GetType().Format()} : {exception.Message.Replace(Environment.NewLine, Environment.NewLine + stringIndent)}");

            foreach (var inner in GetInnerExceptions(exception).ToArray())
                FormatExceptionDetails(inner, builder, indent + 1);
        }

        private static string Format(this Type type)
        {
            if (type == null) 
                return string.Empty;
            return $"{type.Namespace}.{type.Name}";
        }

        private static IEnumerable<Exception> GetInnerExceptions(Exception exception)
        {
            if (exception is AggregateException aggregate)
                return aggregate.InnerExceptions;
            return exception.InnerException != null
                ? Enumerable.Repeat(exception.InnerException, 1)
                : Enumerable.Empty<Exception>();
        }
    }
}