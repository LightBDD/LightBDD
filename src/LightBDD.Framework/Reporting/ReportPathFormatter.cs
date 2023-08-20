using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Reporting.Implementation;

namespace LightBDD.Framework.Reporting
{
    /// <summary>
    /// Summary file path formatter allowing to specify formattable paths with parameters like current date/time or time when test were executed.
    /// Formatter accepts string.Format() like parameters of {name:format} syntax, where <c>name</c> is a parameter name while <c>format</c> is string.Format() parameter format string (it is always required).
    /// </summary>
    //TODO: review and improve
    public class ReportPathFormatter
    {
        private readonly List<KeyValuePair<string, ReportFormattablePath.ParameterProviderFunc>> _parameters = new();

        /// <summary>
        /// Creates default <see cref="ReportPathFormatter"/> that supports following format parameters:
        /// <list type="bullet">
        /// <item><description>CurrentDateTimeUtc - equivalent to DateTime.UtcNow (DateTimeOffset type)</description></item>
        /// <item><description>CurrentDateTime - equivalent to DateTime.Now (DateTimeOffset type)</description></item>
        /// <item><description>TestDateTimeUtc - utc date/time when test execution started (DateTimeOffset type)</description></item>
        /// <item><description>TestDateTime - local date/time when test execution started (DateTimeOffset type)</description></item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static ReportPathFormatter CreateDefault()
        {
            return new ReportPathFormatter()
                .Add("CurrentDateTimeUtc", _ => DateTimeOffset.UtcNow)
                .Add("CurrentDateTime", _ => DateTimeOffset.Now)
                .Add("TestDateTimeUtc", r => r.ExecutionTime.Start.UtcDateTime)
                .Add("TestDateTime", r => r.ExecutionTime.Start.LocalDateTime);
        }

        /// <summary>
        /// Adds new format parameter.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="parameterFunction">Parameter function</param>
        /// <returns>Parameter object.</returns>
        public ReportPathFormatter Add(string name, ReportFormattablePath.ParameterProviderFunc parameterFunction)
        {
            _parameters.Add(new KeyValuePair<string, ReportFormattablePath.ParameterProviderFunc>(name, parameterFunction));
            return this;
        }

        /// <summary>
        /// Converts <paramref name="formattablePath"/> to <see cref="ReportFormattablePath"/> by:
        /// * parameterizing path with configured parameter functions,
        /// * replacing starting '~' character with value of <c>AppContext.BaseDirectory</c>, if present,
        /// * resolving to full path if path is relative.
        /// </summary>
        /// <param name="formattablePath">Formattable path string</param>
        /// <returns>Formattable path.</returns>
        public ReportFormattablePath ToFormattablePath(string formattablePath)
        {
            var parameters = new List<ReportFormattablePath.ParameterProviderFunc>();
            foreach (var pair in _parameters.OrderByDescending(p => p.Key))
            {
                var replaced = formattablePath.Replace("{" + pair.Key + ":", "{" + parameters.Count + ":");
                if (replaced == formattablePath)
                    continue;
                parameters.Add(pair.Value);
                formattablePath = replaced;
            }
            return new ReportFormattablePath(FilePathHelper.ResolveAbsolutePath(formattablePath), parameters);
        }
    }
}