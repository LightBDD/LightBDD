using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Results;
using LightBDD.SummaryGeneration.Implementation;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// Summary file path formatter allowing to specify formattable paths with parameters like current date/time or time when test were executed.
    /// Formatter accepts string.Format() like parameters of {name:format} syntax, where <c>name</c> is a parameter name while <c>format</c> is string.Format() parameter format string (it is always required).
    /// </summary>
    public class SummaryFilePathFormatter
    {
        private readonly List<KeyValuePair<string, Func<IFeatureResult[], object>>> _parameters = new List<KeyValuePair<string, Func<IFeatureResult[], object>>>();

        /// <summary>
        /// Creates default SummaryFilePathFormatter that supports following format parameters:
        /// <list type="bullet">
        /// <item><description>CurrentDateTimeUtc - equivalent to DateTime.UtcNow (DateTime type)</description></item>
        /// <item><description>CurrentDateTime - equivalent to DateTime.Now (DateTime type)</description></item>
        /// <item><description>TestDateTimeUtc - utc date/time when test execution started (DateTime type)</description></item>
        /// <item><description>TestDateTime - local date/time when test execution started (DateTime type)</description></item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static SummaryFilePathFormatter CreateDefault()
        {
            return new SummaryFilePathFormatter()
                .Add("CurrentDateTimeUtc", r => DateTime.UtcNow)
                .Add("CurrentDateTime", r => DateTime.Now)
                .Add("TestDateTimeUtc", r => r.GetTestExecutionStartTime().UtcDateTime)
                .Add("TestDateTime", r => r.GetTestExecutionStartTime().LocalDateTime);
        }

        /// <summary>
        /// Adds new format parameter.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="parameterFunction">Parameter function</param>
        /// <returns>Parameter object.</returns>
        public SummaryFilePathFormatter Add(string name, Func<IFeatureResult[], object> parameterFunction)
        {
            _parameters.Add(new KeyValuePair<string, Func<IFeatureResult[], object>>(name, parameterFunction));
            return this;
        }

        /// <summary>
        /// Converts <c>formattablePath</c> to <c>SummaryFileFormattablePath</c> by:
        /// * parameterizing path with configured parameter functions,
        /// * replacing starting '~' character with value of CurrentDomain.BaseDirectory, if present,
        /// * resolving to full path if path is relative.
        /// </summary>
        /// <param name="formattablePath">Formattable path string</param>
        /// <returns>Formattable path.</returns>
        public SummaryFileFormattablePath ToFormattablePath(string formattablePath)
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
            return new SummaryFileFormattablePath(FilePathHelper.ResolveAbsolutePath(formattablePath), parameters);
        }
    }
}