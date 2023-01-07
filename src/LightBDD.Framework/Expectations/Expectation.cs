#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Base class that should be used for all <see cref="IExpectation{T}"/> implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Expectation<T> : IExpectation<T>, IGeneralExpectationConverter
    {
        /// <inheritdoc />
        public abstract ExpectationResult Verify(T value, IValueFormattingService formattingService);

        /// <inheritdoc />
        public abstract string Format(IValueFormattingService formattingService);

        /// <inheritdoc />
        public override string ToString()
        {
            return Format(ValueFormattingServices.Current);
        }

        /// <summary>
        /// A helper methods used to format failure message for the expectation.
        /// It allows to format a default format failure message and add details in new line, shifted with tabulator character
        /// </summary>
        /// <param name="formattingService">Formatting service.</param>
        /// <param name="failureMessage">Failure message</param>
        /// <param name="details">Failure details that will be added to the message in new line, shifted with tabulator character.</param>
        /// <returns>Expectation result.</returns>
        protected ExpectationResult FormatFailure(IValueFormattingService formattingService, string failureMessage, params string[] details)
        {
            return FormatFailure(formattingService, failureMessage, details.AsEnumerable());
        }

        /// <summary>
        /// A helper methods used to format failure message for the expectation.
        /// It allows to format a default format failure message and add details in new line, shifted with tabulator character
        /// </summary>
        /// <param name="formattingService">Formatting service.</param>
        /// <param name="failureMessage">Failure message</param>
        /// <param name="details">Failure details that will be added to the message in new line, shifted with tabulator character.</param>
        /// <returns>Expectation result.</returns>
        protected ExpectationResult FormatFailure(IValueFormattingService formattingService, string failureMessage, IEnumerable<string> details)
        {
            var builder = new StringBuilder();
            builder.Append("expected: ").Append(Format(formattingService)).Append(", but ").Append(failureMessage);
            foreach (var line in details)
                builder.AppendLine().Append('\t').Append(line.Replace(Environment.NewLine, Environment.NewLine + "\t"));
            return ExpectationResult.Failure(builder.ToString());
        }

        /// <inheritdoc />
        public Expectation<object?> ToGeneralExpectation()
        {
            if (this is Expectation<object?> self)
                return self;
            return this.CastFrom(Expect.Type<object?>());
        }
    }
}