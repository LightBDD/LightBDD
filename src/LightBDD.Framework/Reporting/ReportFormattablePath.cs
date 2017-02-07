using System;
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
        private readonly Func<IFeatureResult[], object>[] _parameters;
        /// <summary>
        /// Returns formattable path specified in constructor.
        /// </summary>
        public string FormattablePath { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formattablePath">Formattable path string with String.Format() syntax.</param>
        /// <param name="parameterProviders">Parameter providing functions.</param>
        public ReportFormattablePath(string formattablePath, IEnumerable<Func<IFeatureResult[], object>> parameterProviders)
        {
            FormattablePath = formattablePath;
            _parameters = parameterProviders.ToArray();
        }

        /// <summary>
        /// Resolves path by applying all the formats and resolving to a full path.
        /// </summary>
        /// <param name="results">Feature results used in format.</param>
        /// <returns></returns>
        public string Resolve(IFeatureResult[] results)
        {
            var formattedPath = string.Format(CultureInfo.InvariantCulture, FormattablePath, _parameters.Select(p => p.Invoke(results)).ToArray());
            return Path.GetFullPath(formattedPath);
        }
    }
}