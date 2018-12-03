using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LightBDD.Core.Formatting.ExceptionFormatting
{
    /// <summary>
    /// Default implementation of exception formatter.
    /// </summary>
    [DebuggerStepThrough]
    public class DefaultExceptionFormatter
    {
        private readonly List<Regex> _excludeMembers = new List<Regex>();
        private int _stackTraceLinesLimit = 8;

        /// <summary>
        /// Constructor initializing formatter with stack trace lines limit equal 8 and default member exclusions.
        /// </summary>
        public DefaultExceptionFormatter()
        {
            WithMembersExcludedFromStackTrace(
                "System.Runtime.ExceptionServices.ExceptionDispatchInfo.*",
                "System.Runtime.CompilerServices.TaskAwaiter.*");
        }

        /// <summary>
        /// Formats the exception by returning exception type name and message, followed by inner exception chain, followed by call stack information.
        /// The stack trace information can be shortened by filtering out members (any call entries starting with specified text) configured with <see cref="WithMembersExcludedFromStackTrace"/>() and number of printed lines configured with <see cref="WithStackTraceLinesLimit"/>().
        /// </summary>
        /// <param name="exception">Exception to format</param>
        /// <returns>Formatted exception details.</returns>
        public string Format(Exception exception)
        {
            var builder = new StringBuilder();

            FormatExceptionDetails(exception, builder);
            string last = null;
            var added = 0;
            foreach (var line in exception.StackTrace.Split('\n').Where(ShouldPrintLine))
            {
                var current = line.Trim();
                if (current.StartsWith("--- End of") && current == last)
                    continue;

                last = current;
                builder.AppendLine(current);

                if (++added >= _stackTraceLinesLimit)
                    break;
            }

            return builder.ToString();
        }

        private bool ShouldPrintLine(string line)
        {
            foreach (var excludeMember in _excludeMembers)
            {
                if (excludeMember.IsMatch(line))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds <paramref name="membersPatterns"/> regex patterns to a list of members that should be excluded from printing when stack trace is rendered.
        /// Each pattern will be used to filter out stack trace lines starting with <c>at ...</c> where <c>at</c> prefix should not be added to the pattern.
        /// 
        /// The method has additive effect and can be used many times.
        /// </summary>
        /// <param name="membersPatterns">Patterns to be used to filter out stack trace lines starting with <c>at ...</c> prefix</param>
        /// <returns>Self.</returns>
        public DefaultExceptionFormatter WithMembersExcludedFromStackTrace(params string[] membersPatterns)
        {
            _excludeMembers.AddRange(membersPatterns.Select(p => new Regex("^\\s*at\\s*" + p, RegexOptions.Compiled)));
            return this;
        }

        /// <summary>
        /// Clears all stack trace members exclusions (the default exclusions will be cleared as well) and removes the line limit.
        /// </summary>
        /// <returns>Self</returns>
        public DefaultExceptionFormatter WithAllMembersIncludedOnStackTrace()
        {
            _excludeMembers.Clear();
            _stackTraceLinesLimit = int.MaxValue;
            return this;
        }

        /// <summary>
        /// Specifies limit of the printed stack trace lines.
        /// By default it is set to 4 lines per exception.
        /// </summary>
        /// <param name="linesLimit">Maximum number of lines that will be printed</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="linesLimit"/> is less than 1.</exception>
        public DefaultExceptionFormatter WithStackTraceLinesLimit(int linesLimit)
        {
            if (linesLimit <= 0)
                throw new ArgumentException("StackTrace line limit cannot be less than 1", nameof(linesLimit));
            _stackTraceLinesLimit = linesLimit;
            return this;
        }

        private static void FormatExceptionDetails(Exception exception, StringBuilder builder, int indent = 0)
        {
            var stringIndent = indent > 0 ? new string('\t', indent) : string.Empty;
            var innerExceptionIndicator = indent > 0 ? "---> " : string.Empty;
            builder.AppendLine($"{stringIndent}{innerExceptionIndicator}{FormatType(exception.GetType())} : {exception.Message.Replace(Environment.NewLine, Environment.NewLine + stringIndent)}");

            foreach (var inner in GetInnerExceptions(exception).ToArray())
                FormatExceptionDetails(inner, builder, indent + 1);
        }

        private static string FormatType(Type type)
        {
            return type == null
                ? string.Empty
                : $"{type.Namespace}.{type.Name}";
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