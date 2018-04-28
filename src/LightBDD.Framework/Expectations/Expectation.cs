using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    public abstract class Expectation<T> : ISelfFormattable
    {
        public abstract ExpectationResult Verify(T value, IValueFormattingService formattingService);

        public abstract string Format(IValueFormattingService formattingService);

        public override string ToString()
        {
            return Format(ValueFormattingServices.Current);
        }

        protected ExpectationResult FormatFailure(IValueFormattingService formattingService, string failureMessage, params string[] details)
        {
            return FormatFailure(formattingService, failureMessage, details.AsEnumerable());
        }

        protected ExpectationResult FormatFailure(IValueFormattingService formattingService, string failureMessage, IEnumerable<string> details)
        {
            var builder = new StringBuilder();
            builder.Append("expected: ").Append(Format(formattingService)).Append(", but ").Append(failureMessage);
            foreach (var line in details)
                builder.AppendLine().Append('\t').Append(line.Replace("\n", "\n\t"));
            return ExpectationResult.Failure(builder.ToString());
        }
    }
}