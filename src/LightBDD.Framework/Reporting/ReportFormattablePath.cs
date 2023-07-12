using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Reporting
{
    /// <summary>
    /// Report file formattable path.
    /// </summary>
    public class ReportFormattablePath
    {
        /// <summary>
        /// Parameter provider function.
        /// </summary>
        public delegate object ParameterProviderFunc(ITestRunResult result);

        private readonly ParameterProviderFunc[] _parameters;
        /// <summary>
        /// Returns formattable path specified in constructor.
        /// </summary>
        public string FormattablePath { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formattablePath">Formattable path string with String.Format() syntax.</param>
        /// <param name="parameterProviders">Parameter providing functions.</param>
        public ReportFormattablePath(string formattablePath, IEnumerable<ParameterProviderFunc> parameterProviders)
        {
            FormattablePath = formattablePath;
            _parameters = parameterProviders.ToArray();
        }

        /// <summary>
        /// Resolves path by applying all the formats and resolving to a full path.
        /// </summary>
        /// <param name="result">Test run result used in format.</param>
        /// <returns></returns>
        public string Resolve(ITestRunResult result)
        {
            var formattedPath = string.Format(CultureInfo.InvariantCulture, FormattablePath, _parameters.Select(p => p.Invoke(result)).ToArray());
            return Path.GetFullPath(formattedPath);
        }
    }
}