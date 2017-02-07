using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Implementation;

namespace LightBDD.Framework.Reporting
{
    /// <summary>
    /// Summary file path formatter allowing to specify formattable paths with parameters like current date/time or time when test were executed.
    /// Formatter accepts string.Format() like parameters of {name:format} syntax, where <c>name</c> is a parameter name while <c>format</c> is string.Format() parameter format string (it is always required).
    /// </summary>
    public class ReportPathFormatter
    {
        private readonly List<KeyValuePair<string, Func<IFeatureResult[], object>>> _parameters = new List<KeyValuePair<string, Func<IFeatureResult[], object>>>();

        /// <summary>
        /// Creates default <see cref="ReportPathFormatter"/> that supports following format parameters:
        /// <list type="bullet">
        /// <item><description>CurrentDateTimeUtc - equivalent to DateTime.UtcNow (DateTime type)</description></item>
        /// <item><description>CurrentDateTime - equivalent to DateTime.Now (DateTime type)</description></item>
        /// <item><description>TestDateTimeUtc - utc date/time when test execution started (DateTime type)</description></item>
        /// <item><description>TestDateTime - local date/time when test execution started (DateTime type)</description></item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static ReportPathFormatter CreateDefault()
        {
            return new ReportPathFormatter()
                .Add("CurrentDateTimeUtc", r => DateTime.UtcNow)
                .Add("CurrentDateTime", r => DateTime.Now)
                .Add("TestDateTimeUtc", r => r.GetTestExecutionTimeSummary().Start.UtcDateTime)
                .Add("TestDateTime", r => r.GetTestExecutionTimeSummary().Start.LocalDateTime);
        }

        /// <summary>
        /// Adds new format parameter.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="parameterFunction">Parameter function</param>
        /// <returns>Parameter object.</returns>
        public ReportPathFormatter Add(string name, Func<IFeatureResult[], object> parameterFunction)
        {
            _parameters.Add(new KeyValuePair<string, Func<IFeatureResult[], object>>(name, parameterFunction));
            return this;
        }

        /// <summary>
        /// Converts <paramref name="formattablePath"/> to <see cref="ReportFormattablePath"/> by:
        /// * parameterizing path with configured parameter functions,
        /// * replacing starting '~' character with value of <see cref="AppContext.BaseDirectory"/>, if present,
        /// * resolving to full path if path is relative.
        /// </summary>
        /// <param name="formattablePath">Formattable path string</param>
        /// <returns>Formattable path.</returns>
        public ReportFormattablePath ToFormattablePath(string formattablePath)
        {
            List<Func<IFeatureResult[], object>> parameters = new List<Func<IFeatureResult[], object>>();
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