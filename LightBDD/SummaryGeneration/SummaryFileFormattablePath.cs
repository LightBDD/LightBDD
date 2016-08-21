using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LightBDD.Results;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// Summary file formattable path.
    /// </summary>
    public class SummaryFileFormattablePath
    {
        private readonly string _formattablePath;
        private readonly Func<IFeatureResult[], object>[] _parameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formattablePath">Formattable path string with String.Format() syntax.</param>
        /// <param name="parameterProviders">Parameter providing functions.</param>
        public SummaryFileFormattablePath(string formattablePath, IEnumerable<Func<IFeatureResult[], object>> parameterProviders)
        {
            _formattablePath = formattablePath;
            _parameters = parameterProviders.ToArray();
        }

        /// <summary>
        /// Resolves path by applying all the formats and resolving to a full path.
        /// </summary>
        /// <param name="results">Feature results used in format.</param>
        /// <returns></returns>
        public string Resolve(IFeatureResult[] results)
        {
            var formattedPath = string.Format(CultureInfo.InvariantCulture, _formattablePath, _parameters.Select(p => p.Invoke(results)).ToArray());
            return Path.GetFullPath(formattedPath);
        }
    }
}